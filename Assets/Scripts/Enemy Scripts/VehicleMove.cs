using UnityEngine;

// Controls enemy vehicle movement, AI behavior, and lane changing
public class VehicleMove : MonoBehaviour
{
    [Header("Base Settings")]
    public float baseSpeed = 15f;
    public int direction = 1; // 1 = opposite direction, -1 = same direction
    public float deleteZ = -50f;

    [Header("AI Settings")]
    public float detectDistance = 15f;
    public float slowDownFactor = 0.5f;
    public float laneWidth = 4f;
    public float laneChangeSpeed = 4f;
    public float laneChangeCooldown = 1f;
    public int maxBounces = 2; // Maximum number of bounces allowed

    private float _currentSpeed;
    private Transform _transform;
    private float _targetX;
    private bool _isChangingLane;
    private float _lastLaneChangeAttempt;
    private int _bounceCount = 0; // Track number of bounces
    private float _lastBounceTime = -999f; // Time of last bounce
    private const float BOUNCE_COOLDOWN = 1f; // Cooldown between bounces

    void Awake()
    {
        _transform = transform;
        _currentSpeed = baseSpeed;
        _targetX = _transform.position.x;
    }

    void Update()
    {
        HandleAI();
        Move();
        CheckDestroy();
    }

    // Handles AI behavior: raycast forward to detect obstacles
    // If vehicle or player detected: adjust speed based on direction
    // Direction 1 (opposite): decrease speed to avoid collision
    // Direction -1 (same): increase speed to overtake, ALSO check backward for player to yield
    // Otherwise: return to base speed
    void HandleAI()
    {
        bool isGameOver = GameLogicController.Instance != null && GameLogicController.Instance.isGameOver;
        
        Vector3 rayOrigin = _transform.position + Vector3.up * 0.5f;
        Vector3 rayDirection;
        
        // If game over and same direction, raycast forward (Z+) instead of transform.forward
        if (isGameOver && direction == -1)
        {
            rayDirection = Vector3.forward; // Check ahead in Z+ direction
        }
        else
        {
            rayDirection = _transform.forward; // Normal: use transform forward
        }

        bool obstacleFront = false;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, detectDistance))
        {
            Debug.DrawRay(rayOrigin, rayDirection * hit.distance, Color.green);

            if (hit.collider.CompareTag("Vehicle") || hit.collider.CompareTag("Player"))
            {
                obstacleFront = true;
                // Adjust speed based on direction
                if (direction == 1)
                {
                    // Opposite direction: slow down to avoid collision
                    _currentSpeed = Mathf.Lerp(_currentSpeed, baseSpeed * slowDownFactor, Time.deltaTime * 4f);
                }
                else
                {
                    // Same direction: speed up to overtake (or slow down if game over)
                    if (isGameOver)
                    {
                        // Game over: slow down to avoid crashed vehicles ahead
                        _currentSpeed = Mathf.Lerp(_currentSpeed, baseSpeed * slowDownFactor, Time.deltaTime * 4f);
                    }
                    else
                    {
                        // Normal: speed up to overtake (actually means slowing down on road, so increasing approach speed)
                        _currentSpeed = Mathf.Lerp(_currentSpeed, baseSpeed * (1f + (1f - slowDownFactor)), Time.deltaTime * 4f);
                    }
                }

                // Try to change lane if cooldown passed
                if (Time.time - _lastLaneChangeAttempt > laneChangeCooldown)
                {
                    TryChangeLane();
                    _lastLaneChangeAttempt = Time.time;
                }
            }
        }
        else
        {
            Debug.DrawRay(rayOrigin, rayDirection * detectDistance, Color.red);
        }

        // Check behind if same direction (yielding to fast approaching player)
        bool yieldingToPlayer = false;
        if (direction == -1 && !isGameOver)
        {
            Vector3 backwardRay = -_transform.forward;
            if (Physics.Raycast(rayOrigin, backwardRay, out RaycastHit backHit, detectDistance * 2f)) // Look further back
            {
                Debug.DrawRay(rayOrigin, backwardRay * backHit.distance, Color.yellow);
                if (backHit.collider.CompareTag("Player"))
                {
                    yieldingToPlayer = true;
                    // Speed up on the road to get out of the player's way (decrease approach speed)
                    _currentSpeed = Mathf.Lerp(_currentSpeed, baseSpeed * 0.5f, Time.deltaTime * 4f);
                    
                    if (Time.time - _lastLaneChangeAttempt > laneChangeCooldown * 0.5f) // Faster lane change response for player
                    {
                        TryChangeLane();
                        _lastLaneChangeAttempt = Time.time;
                    }
                }
            }
        }

        if (!obstacleFront && !yieldingToPlayer)
        {
            // No obstacle: return to base speed
            _currentSpeed = Mathf.Lerp(_currentSpeed, baseSpeed, Time.deltaTime * 2f);
        }
    }

    // Attempts to change lane by trying both left and right directions
    // Checks if new lane position is within road bounds (-15 to +15) and not occupied
    void TryChangeLane()
    {
        float[] directions = { 1f, -1f };
        
        // Randomize direction order
        if (Random.value > 0.5f)
        {
            directions[0] = -1f;
            directions[1] = 1f;
        }

        // Try each direction until valid lane found
        foreach (float dir in directions)
        {
            float newTargetX = _transform.position.x + dir * laneWidth;
            
            // Check if within road bounds
            if (newTargetX >= -15f && newTargetX <= 15f)
            {
                // Check if the target lane is clear
                Vector3 checkPos = new Vector3(newTargetX, _transform.position.y, _transform.position.z);
                Vector3 boxHalfExtents = new Vector3(laneWidth * 0.4f, 1f, 10f);
                
                // Use OverlapBox to ensure no vehicles or player in the target lane area
                Collider[] colliders = Physics.OverlapBox(checkPos, boxHalfExtents, Quaternion.identity);
                bool isClear = true;
                
                foreach (var col in colliders)
                {
                    if (col.CompareTag("Vehicle") || col.CompareTag("Player"))
                    {
                        isClear = false;
                        break;
                    }
                }
                
                if (isClear)
                {
                    _targetX = newTargetX;
                    _isChangingLane = true;
                    return;
                }
            }
        }
    }

    // Moves vehicle toward player (Z-) and smoothly changes lanes (X)
    void Move()
    {
        // Check if game is over and this is a same-direction vehicle
        bool isGameOver = GameLogicController.Instance != null && GameLogicController.Instance.isGameOver;
        
        Vector3 moveDirection;
        if (isGameOver && direction == -1)
        {
            // Game over + same direction: move forward (Z+) instead of toward player
            moveDirection = Vector3.forward;
        }
        else
        {
            // Normal: all vehicles move toward player (Z-)
            moveDirection = Vector3.back;
        }
        
        _transform.position += moveDirection * _currentSpeed * Time.deltaTime;

        // Smooth lane change movement
        Vector3 pos = _transform.position;
        pos.x = Mathf.MoveTowards(pos.x, _targetX, laneChangeSpeed * Time.deltaTime);
        _transform.position = pos;

        // Mark lane change as complete when reached target
        if (Mathf.Abs(_transform.position.x - _targetX) < 0.05f)
            _isChangingLane = false;
    }

    // Destroys vehicle when it passes behind player
    void CheckDestroy()
    {
        bool isGameOver = GameLogicController.Instance != null && GameLogicController.Instance.isGameOver;
        
        // If game over and same direction vehicle, destroy when too far forward instead
        if (isGameOver && direction == -1)
        {
            if (_transform.position.z >= 170f)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            // Normal: destroy when passes behind player
            if (_transform.position.z <= deleteZ)
            {
                Destroy(gameObject);
            }
        }
    }

    // Initializes vehicle with speed and direction from spawner
    public void Init(float speed, int dir)
    {
        baseSpeed = speed;
        _currentSpeed = speed;
        direction = dir;
    }

    // Handle collision with vehicles and player
    void OnCollisionEnter(Collision collision)
    {
        // Collision with Player - always trigger (even if max bounces reached)
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameLogicController.Instance != null)
            {
                GameLogicController.Instance.OnVehicleCollision(gameObject, collision.gameObject);
            }
            return;
        }
        
        // Collision with Vehicle - check bounce limit and cooldown
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            // Check if max bounces reached
            if (_bounceCount >= maxBounces)
                return; // No more bounces allowed
            
            // Check cooldown
            if (Time.time - _lastBounceTime < BOUNCE_COOLDOWN)
                return; // Still in cooldown
            
            _lastBounceTime = Time.time;
            _bounceCount++;
            
            if (GameLogicController.Instance != null)
            {
                GameLogicController.Instance.OnVehicleCollision(gameObject, collision.gameObject);
            }
        }
    }
}
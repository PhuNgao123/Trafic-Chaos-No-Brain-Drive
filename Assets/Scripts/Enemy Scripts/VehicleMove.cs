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

    private float _currentSpeed;
    private Transform _transform;
    private float _targetX;
    private bool _isChangingLane;
    private float _lastLaneChangeAttempt;
    private bool _hasBounced = false; // Track if vehicle has bounced

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
    // Direction -1 (same): increase speed to overtake
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

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, detectDistance))
        {
            Debug.DrawRay(rayOrigin, rayDirection * hit.distance, Color.green);

            if (hit.collider.CompareTag("Vehicle") || hit.collider.CompareTag("Player"))
            {
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
                        // Normal: speed up to overtake
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

            // No obstacle: return to base speed
            _currentSpeed = Mathf.Lerp(_currentSpeed, baseSpeed, Time.deltaTime * 2f);
        }
    }

    // Attempts to change lane by trying both left and right directions
    // Checks if new lane position is within road bounds (-15 to +15)
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
                _targetX = newTargetX;
                _isChangingLane = true;
                return;
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

    // Handle vehicle-to-vehicle collision
    void OnCollisionEnter(Collision collision)
    {
        // Only bounce once
        if (_hasBounced) return;
        
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            _hasBounced = true; // Mark as bounced
            
            if (GameLogicController.Instance != null)
            {
                GameLogicController.Instance.OnVehicleCollision(gameObject, collision.gameObject);
            }
        }
    }
}

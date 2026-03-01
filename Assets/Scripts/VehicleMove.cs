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
        Vector3 rayOrigin = _transform.position + Vector3.up * 0.5f;
        Vector3 rayDirection = _transform.forward;

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
                    // Same direction: speed up to overtake
                    _currentSpeed = Mathf.Lerp(_currentSpeed, baseSpeed * (1f + (1f - slowDownFactor)), Time.deltaTime * 4f);
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
        // All vehicles move toward player regardless of direction
        _transform.position += Vector3.back * _currentSpeed * Time.deltaTime;

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
        if (_transform.position.z <= deleteZ)
            Destroy(gameObject);
    }

    // Initializes vehicle with speed and direction from spawner
    public void Init(float speed, int dir)
    {
        baseSpeed = speed;
        _currentSpeed = speed;
        direction = dir;
    }
}

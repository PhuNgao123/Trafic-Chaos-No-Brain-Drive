using UnityEngine;

public class PlayerPhysics : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    public RoadSpawner roadSpawner;
    public RoadMover roadMover;

    [Header("=== SPEED ===")]
    public float currentSpeed = 10f;
    public float minSpeed = 5f;
    public float maxSpeed = 30f;
    public float acceleration = 5f;
    public float deceleration = 5f;

    [Header("=== STEERING (ARCADE) ===")]
    public float steerSpeed = 8f;
    public float maxSteerVelocity = 15f;

    [Header("=== DRIFT ===")]
    public float driftAmount = 3f;

    [Header("=== POSITION ===")]
    public float fixedZ = 0f; // Player stays at Z = 0
    public float zNormalizeSpeed = 5f;

    [Header("=== ROAD ALIGNMENT ===")]
    public float lookAheadDistance = 10f; // Look ahead distance on the road
    public float rotationSmoothSpeed = 10f;

    private Rigidbody _rb;
    private float _horizontalInput = 0f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.constraints = RigidbodyConstraints.FreezePositionZ; // Freeze Z position - player stays at Z = 0
        
        if (roadSpawner == null)
            roadSpawner = FindFirstObjectByType<RoadSpawner>();
        
        if (roadMover == null)
            roadMover = FindFirstObjectByType<RoadMover>();
    }

    void Update()
    {
        HandleSpeed();
        HandleInput();
    }

    void FixedUpdate()
    {
        HandleSteering();
        NormalizeZ(); // Keep player at Z = 0
        AlignWithRoad(); // Align rotation with road ahead
    }

    void HandleSpeed()
    {
        float v = Input.GetAxis("Vertical");
        if (v > 0) currentSpeed += acceleration * Time.deltaTime;
        else if (v < 0) currentSpeed -= deceleration * Time.deltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
        
        // Control road movement speed
        if (roadMover != null)
        {
            roadMover.speed = currentSpeed;
        }
    }

    void HandleInput()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
    }

    void HandleSteering()
    {
        if (_rb == null) return;

        // Only move left/right (X axis), Z is frozen by constraints
        Vector3 vel = _rb.linearVelocity;
        
        float targetXVel = _horizontalInput * steerSpeed;
        vel.x = Mathf.Lerp(vel.x, targetXVel, 10f * Time.fixedDeltaTime);
        vel.x = Mathf.Clamp(vel.x, -maxSteerVelocity, maxSteerVelocity);
        
        _rb.linearVelocity = vel;
    }

    void NormalizeZ()
    {
        if (_rb == null) return;

        // Keep player at fixedZ = 0
        float currentZ = _rb.position.z;
        float zDiff = fixedZ - currentZ;

        if (Mathf.Abs(zDiff) > 0.01f)
        {
            Vector3 normalizeForce = Vector3.forward * zDiff * zNormalizeSpeed;
            _rb.AddForce(normalizeForce, ForceMode.Force);
        }
    }

    void AlignWithRoad()
    {
        if (roadSpawner == null || _rb == null || roadMover == null) return;

        // Look at the road ahead (where player is moving towards)
        // Since road moves toward -Z, the ahead section has more negative Z
        
        // Get road container position
        float roadZ = roadMover.transform.position.z;
        
        // Look ahead point on the road (more negative Z than container)
        float lookAheadZ = roadZ - lookAheadDistance;
        
        // Get road direction at the look ahead point
        Vector3 roadDirection = roadSpawner.GetDirectionAtZ(lookAheadZ);
        
        // Create rotation based on road direction
        Quaternion targetRotation = Quaternion.LookRotation(roadDirection, Vector3.up);
        
        // Add drift effect from steering input
        float driftAngle = _horizontalInput * driftAmount;
        Quaternion driftRotation = Quaternion.Euler(0, driftAngle, 0);
        
        // Smooth rotation - don't use MoveRotation to avoid constraint conflicts
        _rb.rotation = Quaternion.Slerp(
            _rb.rotation,
            targetRotation * driftRotation,
            rotationSmoothSpeed * Time.fixedDeltaTime
        );
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }

    public float GetHorizontalInput()
    {
        return _horizontalInput;
    }
}
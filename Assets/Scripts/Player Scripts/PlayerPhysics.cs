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
    public float fixedZ = 0f;
    public float zNormalizeSpeed = 5f;

    [Header("=== ROAD ALIGNMENT ===")]
    public float lookAheadDistance = 10f;
    public float rotationSmoothSpeed = 10f;

    private Rigidbody _rb;
    private float _horizontalInput = 0f;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

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
        NormalizeZ();
        AlignWithRoad();
    }

    void HandleSpeed()
    {
        float v = Input.GetAxis("Vertical");

        if (v > 0)
            currentSpeed += acceleration * Time.deltaTime;
        else if (v < 0)
            currentSpeed -= deceleration * Time.deltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);

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

        Vector3 vel = _rb.linearVelocity;

        float targetXVel = _horizontalInput * steerSpeed;
        vel.x = Mathf.Lerp(vel.x, targetXVel, 10f * Time.fixedDeltaTime);
        vel.x = Mathf.Clamp(vel.x, -maxSteerVelocity, maxSteerVelocity);

        _rb.linearVelocity = vel;
    }

    void NormalizeZ()
    {
        Vector3 pos = _rb.position;
        pos.z = Mathf.Lerp(pos.z, fixedZ, zNormalizeSpeed * Time.fixedDeltaTime);
        _rb.MovePosition(pos);
    }

    void AlignWithRoad()
    {
        if (roadSpawner == null || _rb == null || roadMover == null) return;

        float roadZ = roadMover.transform.position.z;
        float lookAheadZ = roadZ - lookAheadDistance;

        Vector3 roadDirection = roadSpawner.GetDirectionAtZ(lookAheadZ);

        Quaternion targetRotation = Quaternion.LookRotation(roadDirection, Vector3.up);

        float driftAngle = _horizontalInput * driftAmount;
        Quaternion driftRotation = Quaternion.Euler(0, driftAngle, 0);

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
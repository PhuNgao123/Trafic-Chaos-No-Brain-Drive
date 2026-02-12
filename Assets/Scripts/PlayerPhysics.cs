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
    public float fixedZ = 0f; // Xe đứng im tại Z = 0
    public float zNormalizeSpeed = 5f;

    [Header("=== ROAD ALIGNMENT ===")]
    public float lookAheadDistance = 10f; // Nhìn về phía trước trên đường (đã di chuyển)
    public float rotationSmoothSpeed = 10f;

    private Rigidbody rb;
    private float horizontalInput = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionZ; // Freeze Z - xe đứng im
        
        if (roadSpawner == null)
            roadSpawner = FindObjectOfType<RoadSpawner>();
        
        if (roadMover == null)
            roadMover = FindObjectOfType<RoadMover>();
    }

    void Update()
    {
        HandleSpeed();
        HandleInput();
    }

    void FixedUpdate()
    {
        HandleSteering();
        NormalizeZ(); // Giữ xe tại Z = 0
        AlignWithRoad(); // CHỈ align rotation theo đường phía trước
    }

    void HandleSpeed()
    {
        float v = Input.GetAxis("Vertical");
        if (v > 0) currentSpeed += acceleration * Time.deltaTime;
        else if (v < 0) currentSpeed -= deceleration * Time.deltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
        
        // Điều khiển tốc độ road di chuyển
        if (roadMover != null)
        {
            roadMover.speed = currentSpeed;
        }
    }

    void HandleInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
    }

    void HandleSteering()
    {
        if (rb == null) return;

        // CHỈ di chuyển trái phải (X), Z bị freeze bởi constraints
        Vector3 vel = rb.linearVelocity;
        
        float targetXVel = horizontalInput * steerSpeed;
        vel.x = Mathf.Lerp(vel.x, targetXVel, 10f * Time.fixedDeltaTime);
        vel.x = Mathf.Clamp(vel.x, -maxSteerVelocity, maxSteerVelocity);
        
        rb.linearVelocity = vel;
    }

    void NormalizeZ()
    {
        if (rb == null) return;

        // Giữ xe tại fixedZ = 0
        float currentZ = rb.position.z;
        float zDiff = fixedZ - currentZ;

        if (Mathf.Abs(zDiff) > 0.01f)
        {
            Vector3 normalizeForce = Vector3.forward * zDiff * zNormalizeSpeed;
            rb.AddForce(normalizeForce, ForceMode.Force);
        }
    }

    void AlignWithRoad()
    {
        if (roadSpawner == null || rb == null || roadMover == null) return;

        // QUAN TRỌNG: Nhìn vào phần đường PHÍA TRƯỚC (đang chạy tới)
        // Vì road di chuyển về phía -Z, phần đường phía trước là có Z âm hơn
        
        // Lấy vị trí của road container
        float roadZ = roadMover.transform.position.z;
        
        // Điểm cần nhìn: Phía trước xe trên road (Z âm hơn so với road container)
        float lookAheadZ = roadZ - lookAheadDistance;
        
        // Lấy hướng của road tại điểm phía trước
        Vector3 roadDirection = roadSpawner.GetDirectionAtZ(lookAheadZ);
        
        // Tạo rotation theo hướng road
        Quaternion targetRotation = Quaternion.LookRotation(roadDirection, Vector3.up);
        
        // Thêm drift effect từ input
        float driftAngle = horizontalInput * driftAmount;
        Quaternion driftRotation = Quaternion.Euler(0, driftAngle, 0);
        
        // Smooth rotation - KHÔNG dùng MoveRotation vì có thể conflict với constraints
        rb.rotation = Quaternion.Slerp(
            rb.rotation,
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
        return horizontalInput;
    }
}
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

    [Header("=== STEERING ===")]
    public float steerForce = 500f;

    [Header("=== POSITION ===")]
    public float fixedZ = 0f;
    public float zNormalizeSpeed = 5f; 

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        if (roadSpawner == null)
            roadSpawner = FindObjectOfType<RoadSpawner>();
        
        if (roadMover == null)
            roadMover = FindObjectOfType<RoadMover>();
    }

    void Update()
    {
        HandleSpeed();
    }

    void FixedUpdate()
    {
        HandleSteering();
        NormalizeZ();
        NormalizeWithRoad();
    }

    void HandleSpeed()
    {
        float v = Input.GetAxis("Vertical");
        if (v > 0) currentSpeed += acceleration * Time.deltaTime;
        else if (v < 0) currentSpeed -= deceleration * Time.deltaTime;

        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }

    void HandleSteering()
    {
        if (rb == null) return;

        float h = Input.GetAxis("Horizontal");
        
        // Thêm lực trái/phải - physics tự xử lý
        Vector3 steerDirection = transform.right * h;
        rb.AddForce(steerDirection * steerForce * Time.fixedDeltaTime, ForceMode.Force);
    }

    void NormalizeZ()
    {
        if (rb == null) return;

        // Nếu Z lệch khỏi fixedZ, kéo dần về
        float currentZ = rb.position.z;
        float zDiff = fixedZ - currentZ;

        if (Mathf.Abs(zDiff) > 0.01f)
        {
            // Thêm lực kéo về fixedZ
            Vector3 normalizeForce = Vector3.forward * zDiff * zNormalizeSpeed;
            rb.AddForce(normalizeForce, ForceMode.Force);
        }
    }

    void NormalizeWithRoad()
    {
        if (roadSpawner == null || rb == null) return;

        float curveZ = fixedZ;
        
        if (roadMover != null)
        {
            curveZ = fixedZ - roadMover.transform.position.z;
        }

        // Lấy hướng đường để normalize rotation
        Vector3 direction = roadSpawner.GetDirectionAtZ(curveZ);
        Quaternion roadRotation = Quaternion.LookRotation(direction, Vector3.up);
        
        // Chỉ normalize rotation, không động vào position
        rb.MoveRotation(roadRotation);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}

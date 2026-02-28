using UnityEngine;

public class PlayerController : MonoBehaviour
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
    public float steerSpeed = 5f;

    [Header("=== POSITION ===")]
    public float fixedZ = 0f;
    public float heightOffset = 0.5f; // Y offset to keep player close to road

    private float _laneOffset = 0f;
    private Rigidbody _rb;

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
        HandleSteering();
    }

    void FixedUpdate()
    {
        UpdatePosition();
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
        float h = Input.GetAxis("Horizontal");
        _laneOffset += h * steerSpeed * Time.deltaTime;
    }

    void UpdatePosition()
    {
        if (roadSpawner == null || _rb == null) return;

        // Calculate curve position relative to player
        float curveZ = fixedZ;
        
        if (roadMover != null)
        {
            curveZ = fixedZ - roadMover.transform.position.z;
        }

        // Get elevation (Y) at this position - no X curve
        float roadY = roadSpawner.GetElevationAtZ(curveZ);

        // Player position = X from steering + Y from road elevation + fixed Z
        Vector3 targetPos = new Vector3(
            _laneOffset,  // Simple X steering, no curve following
            roadY + heightOffset,
            fixedZ
        );

        // Use MovePosition to maintain physics
        _rb.MovePosition(targetPos);

        // No rotation - straight roads, player always faces forward (Z+)
        _rb.MoveRotation(Quaternion.identity);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}

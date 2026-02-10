using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    public RoadSpawner roadSpawner;

    [Header("=== SPEED ===")]
    public float currentSpeed = 10f;
    public float minSpeed = 5f;
    public float maxSpeed = 30f;
    public float acceleration = 5f;
    public float deceleration = 5f;

    [Header("=== STEERING ===")]
    public float steerSpeed = 5f;
    public float maxSteerOffset = 3f;
    public float driftAngle = 20f;

    float roadZ = 0f;
    float laneOffset = 0f;
    float currentDrift = 0f;

    void Start()
    {
        if (roadSpawner == null)
            roadSpawner = FindObjectOfType<RoadSpawner>();
    }

    void Update()
    {
        HandleSpeed();
        HandleSteering();

        roadZ += currentSpeed * Time.deltaTime;

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
        laneOffset += h * steerSpeed * Time.deltaTime;
        laneOffset = Mathf.Clamp(laneOffset, -maxSteerOffset, maxSteerOffset);

        currentDrift = Mathf.Lerp(currentDrift, -h * driftAngle, 8f * Time.deltaTime);
    }

    void UpdatePosition()
    {
        Vector3 center = roadSpawner.GetPositionAtZ(roadZ);
        Vector3 dir = roadSpawner.GetDirectionAtZ(roadZ);

        Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        Vector3 pos = center + right * laneOffset;

        transform.position = pos;

        Quaternion roadRot = Quaternion.LookRotation(dir, Vector3.up);
        Quaternion driftRot = Quaternion.AngleAxis(currentDrift, Vector3.forward);

        transform.rotation = roadRot * driftRot;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}

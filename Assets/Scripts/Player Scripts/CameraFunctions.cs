using UnityEngine;

public class CameraFunctions : MonoBehaviour
{
    [Header("=== TARGET ===")]
    public Transform target;
    public PlayerPhysics playerPhysics; // To get player speed

    [Header("=== OFFSET ===")]
    public Vector3 offset = new Vector3(0, 5, -10);
    public float offsetSmoothSpeed = 5f;

    [Header("=== SMOOTHNESS ===")]
    public float positionSmooth = 8f;
    public float rotationSmooth = 6f;

    [Header("=== FOV DYNAMIC ===")]
    public Camera cam;
    public float baseFOV = 60f;
    public float maxFOV = 80f;
    public float fovSmoothSpeed = 3f;

    [Header("=== SHAKE REDUCTION ===")]
    public float shakeReduction = 0.95f; // 0-1, higher = smoother

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _lastTargetPos;
    private float _currentFOV;

    void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        // Auto-find PlayerPhysics if not set
        if (playerPhysics == null)
            playerPhysics = FindFirstObjectByType<PlayerPhysics>();

        // Use PlayerPhysics as target if not set
        if (target == null && playerPhysics != null)
            target = playerPhysics.transform;

        if (target != null)
            _lastTargetPos = target.position;

        _currentFOV = baseFOV;
        if (cam != null)
            cam.fieldOfView = _currentFOV;
    }

    void LateUpdate()
    {
        if (target == null) return;

        UpdateFOV();
        UpdatePosition();
        UpdateRotation();
    }

    void UpdateFOV()
    {
        if (cam == null || playerPhysics == null) return;

        // Calculate FOV based on speed
        float speed = playerPhysics.GetCurrentSpeed();
        float maxSpeed = playerPhysics.maxSpeed;
        float speedRatio = Mathf.Clamp01(speed / maxSpeed);

        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speedRatio);
        _currentFOV = Mathf.Lerp(_currentFOV, targetFOV, fovSmoothSpeed * Time.deltaTime);

        cam.fieldOfView = _currentFOV;
    }

    void UpdatePosition()
    {
        // Calculate target velocity to reduce shake
        Vector3 targetVelocity = (target.position - _lastTargetPos) / Time.deltaTime;
        _lastTargetPos = target.position;

        // Smooth velocity to reduce shake
        _velocity = Vector3.Lerp(_velocity, targetVelocity, shakeReduction);

        // Target position with offset
        Vector3 targetPos = target.position + target.TransformDirection(offset);

        // Smooth follow for smoother camera movement
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            positionSmooth * Time.deltaTime
        );
    }

    void UpdateRotation()
    {
        // Always look at target
        Vector3 lookDirection = target.position - transform.position;
        
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSmooth * Time.deltaTime
            );
        }
    }
}

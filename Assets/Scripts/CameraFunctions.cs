using UnityEngine;

public class CameraFunctions : MonoBehaviour
{
    [Header("=== TARGET ===")]
    public Transform target;
    public PlayerPhysics playerPhysics; // Để lấy speed

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
    public float shakeReduction = 0.95f; // 0-1, càng cao càng mượt

    private Vector3 velocity = Vector3.zero;
    private Vector3 lastTargetPos;
    private float currentFOV;

    void Start()
    {
        if (cam == null)
            cam = GetComponent<Camera>();

        // Tự động tìm PlayerPhysics nếu chưa set
        if (playerPhysics == null)
            playerPhysics = FindObjectOfType<PlayerPhysics>();

        // Nếu target chưa set, dùng PlayerPhysics làm target
        if (target == null && playerPhysics != null)
            target = playerPhysics.transform;

        if (target != null)
            lastTargetPos = target.position;

        currentFOV = baseFOV;
        if (cam != null)
            cam.fieldOfView = currentFOV;
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

        // Tính FOV dựa trên tốc độ
        float speed = playerPhysics.GetCurrentSpeed();
        float maxSpeed = playerPhysics.maxSpeed;
        float speedRatio = Mathf.Clamp01(speed / maxSpeed);

        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, speedRatio);
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, fovSmoothSpeed * Time.deltaTime);

        cam.fieldOfView = currentFOV;
    }

    void UpdatePosition()
    {
        // Tính vận tốc của target để giảm shake
        Vector3 targetVelocity = (target.position - lastTargetPos) / Time.deltaTime;
        lastTargetPos = target.position;

        // Smooth velocity để giảm rung
        velocity = Vector3.Lerp(velocity, targetVelocity, shakeReduction);

        // Vị trí mục tiêu với offset
        Vector3 targetPos = target.position + target.TransformDirection(offset);

        // Smooth follow với SmoothDamp để mượt hơn
        transform.position = Vector3.Lerp(
            transform.position,
            targetPos,
            positionSmooth * Time.deltaTime
        );
    }

    void UpdateRotation()
    {
        // Luôn nhìn về target
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

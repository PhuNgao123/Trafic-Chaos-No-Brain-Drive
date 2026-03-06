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
    public float maxFOV = 90f; // Increased for more intense effect
    public float fovSmoothSpeed = 5f; // Faster FOV change

    [Header("=== SHAKE REDUCTION ===")]
    public float shakeReduction = 0.95f; // 0-1, higher = smoother

    [Header("=== CAMERA SHAKE ===")]
    public float nearMissShakeDuration = 0.2f;
    public float nearMissShakeIntensity = 0.15f;
    public float crashShakeDuration = 1.5f;
    public float crashShakeIntensity = 0.8f;

    [Header("=== MENU CAMERA ===")]
    public Vector3 menuPosition = new Vector3(7f, 2f, 5f);
    public Vector3 menuRotation = new Vector3(8f, 210f, 0f);
    public float cameraTransitionSpeed = 2f;

    private Vector3 _velocity = Vector3.zero;
    private Vector3 _lastTargetPos;
    private float _currentFOV;
    private bool _isGameOver = false;
    private Vector3 _gameOverPosition;
    private bool _isInMenuMode = true;
    private bool _isTransitioning = false;
    
    // Shake variables
    private float _shakeTimer = 0f;
    private float _shakeIntensity = 0f;
    private Vector3 _shakeOffset = Vector3.zero;

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

        // Set initial menu position
        transform.position = menuPosition;
        transform.eulerAngles = menuRotation;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Check if game started
        if (_isInMenuMode && GameLogicController.Instance != null && GameLogicController.Instance.isGameStarted)
        {
            StartCameraTransition();
        }

        // Check game over state
        if (!_isGameOver && GameLogicController.Instance != null && GameLogicController.Instance.isGameOver)
        {
            OnGameOver();
        }

        UpdateFOV();
        UpdateShake();
        
        if (_isInMenuMode || _isTransitioning)
        {
            UpdateMenuCamera();
        }
        else if (!_isGameOver)
        {
            UpdatePosition();
            UpdateRotation();
        }
        // If game over, camera stays at _gameOverPosition with shake only
    }

    void StartCameraTransition()
    {
        _isInMenuMode = false;
        _isTransitioning = true;
    }

    void UpdateMenuCamera()
    {
        if (!_isTransitioning)
            return;

        // Calculate target position
        Vector3 targetPos = target.position + target.TransformDirection(offset);
        Vector3 lookDirection = target.position - targetPos;
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        // Lerp position and rotation
        transform.position = Vector3.Lerp(transform.position, targetPos, cameraTransitionSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, cameraTransitionSpeed * Time.deltaTime);

        // Check if transition complete
        if (Vector3.Distance(transform.position, targetPos) < 0.5f)
        {
            _isTransitioning = false;
            _lastTargetPos = target.position;
        }
    }

    void OnGameOver()
    {
        _isGameOver = true;
        _gameOverPosition = transform.position;
        TriggerCrashShake();
    }

    void UpdateFOV()
    {
        if (cam == null || playerPhysics == null) return;

        // Calculate FOV based on speed (more intense)
        float speed = playerPhysics.GetCurrentSpeed();
        float maxSpeed = playerPhysics.maxSpeed;
        float speedRatio = Mathf.Clamp01(speed / maxSpeed);

        // Use power curve for more dramatic effect
        float fovCurve = Mathf.Pow(speedRatio, 1.5f);
        float targetFOV = Mathf.Lerp(baseFOV, maxFOV, fovCurve);
        
        _currentFOV = Mathf.Lerp(_currentFOV, targetFOV, fovSmoothSpeed * Time.deltaTime);

        cam.fieldOfView = _currentFOV;
    }

    void UpdateShake()
    {
        if (_shakeTimer > 0f)
        {
            _shakeTimer -= Time.deltaTime;
            
            // Generate random shake offset
            _shakeOffset = Random.insideUnitSphere * _shakeIntensity;
            
            // Decay intensity over time
            float shakeProgress = 1f - (_shakeTimer / (_shakeIntensity > 0.5f ? crashShakeDuration : nearMissShakeDuration));
            _shakeIntensity *= Mathf.Lerp(1f, 0.5f, shakeProgress);
        }
        else
        {
            _shakeOffset = Vector3.Lerp(_shakeOffset, Vector3.zero, Time.deltaTime * 10f);
        }

        // Apply shake to camera position
        if (_isGameOver)
        {
            transform.position = _gameOverPosition + _shakeOffset;
        }
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
        Vector3 smoothPos = Vector3.Lerp(
            transform.position,
            targetPos,
            positionSmooth * Time.deltaTime
        );

        // Apply shake offset
        transform.position = smoothPos + _shakeOffset;
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

    // Public methods to trigger shake effects
    public void TriggerNearMissShake()
    {
        _shakeTimer = nearMissShakeDuration;
        _shakeIntensity = nearMissShakeIntensity;
    }

    public void TriggerCrashShake()
    {
        _shakeTimer = crashShakeDuration;
        _shakeIntensity = crashShakeIntensity;
    }
}

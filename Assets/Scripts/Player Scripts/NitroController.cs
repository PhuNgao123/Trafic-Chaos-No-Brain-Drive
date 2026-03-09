using UnityEngine;

// Nitro: activate with Space. 5x speed while active and front collider disabled (invincible).
// Nitro stacks from side slipstream (close to bot cars on left/right) or NitroPickup power-ups.
// During nitro, player becomes much heavier and pushes cars out of the way.
public class NitroController : MonoBehaviour
{
    [Header("References")]
    public Collider frontTriggerCollider;  // GameOverTrigger collider - disabled during nitro
    public PlayerPhysics playerPhysics;
    public Rigidbody playerRigidbody;

    [Header("Nitro Amount")]
    public float maxNitroAmount = 100f;
    public float nitroDrainPerSecond = 10f;
    public float nitroSlipstreamAddPerSecond = 50f;  // When bot car is in side zones
    public float nitroPickupAmount = 50f;

    [Header("Nitro Power")]
    public float nitroSpeedMultiplier = 5f;  // 5x speed during nitro
    public float nitroMassMultiplier = 10f;  // 10x heavier to push cars

    [Header("Optional: Side zones (auto-created if missing)")]
    public NitroStackZone nitroStackZone;

    private float _nitroAmount;
    private bool _isNitroActive;
    private bool _frontColliderWasEnabled = true;
    private float _originalMass = 1f;

    void Start()
    {
        _nitroAmount = 0f;

        if (frontTriggerCollider == null)
        {
            Transform front = transform.Find("GameOverTrigger");
            if (front != null)
                frontTriggerCollider = front.GetComponent<Collider>();
        }

        if (playerPhysics == null)
            playerPhysics = GetComponent<PlayerPhysics>();

        if (playerRigidbody == null)
            playerRigidbody = GetComponent<Rigidbody>();

        // Store original mass
        if (playerRigidbody != null)
            _originalMass = playerRigidbody.mass;

        if (nitroStackZone == null)
        {
            nitroStackZone = GetComponentInChildren<NitroStackZone>();
            if (nitroStackZone == null)
                CreateSideNitroZones();
        }
    }

    void CreateSideNitroZones()
    {
        // Left side zone
        GameObject left = new GameObject("NitroStackZone_Left");
        left.transform.SetParent(transform, false);
        left.transform.localPosition = new Vector3(-2f, 0.5f, 0f); // Left side of car
        left.transform.localRotation = Quaternion.identity;
        left.transform.localScale = Vector3.one;

        BoxCollider leftCol = left.AddComponent<BoxCollider>();
        leftCol.isTrigger = true;
        leftCol.size = new Vector3(1.5f, 1.5f, 4f);
        leftCol.center = Vector3.zero;

        NitroStackZone leftZone = left.AddComponent<NitroStackZone>();
        leftZone.nitroAddPerSecond = nitroSlipstreamAddPerSecond;
        leftZone.nitroController = this;

        // Right side zone
        GameObject right = new GameObject("NitroStackZone_Right");
        right.transform.SetParent(transform, false);
        right.transform.localPosition = new Vector3(2f, 0.5f, 0f); // Right side of car
        right.transform.localRotation = Quaternion.identity;
        right.transform.localScale = Vector3.one;

        BoxCollider rightCol = right.AddComponent<BoxCollider>();
        rightCol.isTrigger = true;
        rightCol.size = new Vector3(1.5f, 1.5f, 4f);
        rightCol.center = Vector3.zero;

        NitroStackZone rightZone = right.AddComponent<NitroStackZone>();
        rightZone.nitroAddPerSecond = nitroSlipstreamAddPerSecond;
        rightZone.nitroController = this;

        // Keep reference to one of them so the field is not null
        nitroStackZone = leftZone;
    }

    void Update()
    {
        bool gameOver = GameLogicController.Instance != null && GameLogicController.Instance.isGameOver;

        if (gameOver)
        {
            if (_isNitroActive)
                EndNitro();
            return;
        }

        // Activate nitro with Space only when bar is full (drains until empty)
        if (Input.GetKeyDown(KeyCode.Space) && IsNitroReady && !_isNitroActive)
            StartNitro();

        if (_isNitroActive)
        {
            _nitroAmount -= nitroDrainPerSecond * Time.deltaTime;
            if (_nitroAmount <= 0f)
            {
                _nitroAmount = 0f;
                EndNitro();
            }
        }
    }

    void StartNitro()
    {
        _isNitroActive = true;
        
        // 5x speed boost
        if (playerPhysics != null)
            playerPhysics.speedMultiplier = nitroSpeedMultiplier;
        
        // Make player much heavier to push cars
        if (playerRigidbody != null)
            playerRigidbody.mass = _originalMass * nitroMassMultiplier;
        
        // Disable front collision trigger (invincible)
        if (frontTriggerCollider != null)
        {
            _frontColliderWasEnabled = frontTriggerCollider.enabled;
            frontTriggerCollider.enabled = false;
        }
    }

    void EndNitro()
    {
        _isNitroActive = false;
        
        // Restore normal speed
        if (playerPhysics != null)
            playerPhysics.speedMultiplier = 1f;
        
        // Restore original mass
        if (playerRigidbody != null)
            playerRigidbody.mass = _originalMass;
        
        // Re-enable front collision trigger
        if (frontTriggerCollider != null)
            frontTriggerCollider.enabled = _frontColliderWasEnabled;
    }

    public void AddNitro(float amount)
    {
        _nitroAmount = Mathf.Min(_nitroAmount + amount, maxNitroAmount);
    }

    public void AddNitroPickup()
    {
        AddNitro(nitroPickupAmount);
    }

    public bool IsNitroActive => _isNitroActive;
    public float NitroAmount => _nitroAmount;
    public float NitroPercent => maxNitroAmount > 0 ? _nitroAmount / maxNitroAmount : 0f;
    public bool IsNitroReady => _nitroAmount >= maxNitroAmount - 0.01f;
}

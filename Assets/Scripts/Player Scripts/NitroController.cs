using UnityEngine;

// Nitro: activate with Space. 2x speed while active and front collider disabled (invincible).
// Nitro stacks from side slipstream (close to bot cars on left/right) or NitroPickup power-ups.
public class NitroController : MonoBehaviour
{
    [Header("References")]
    public Collider frontTriggerCollider;  // GameOverTrigger collider - disabled during nitro
    public PlayerPhysics playerPhysics;

    [Header("Nitro Amount")]
    public float maxNitroAmount = 100f;
    public float nitroDrainPerSecond = 25f;
    public float nitroSlipstreamAddPerSecond = 15f;  // When bot car is in side zones
    public float nitroPickupAmount = 50f;

    [Header("Optional: Side zones (auto-created if missing)")]
    public NitroStackZone nitroStackZone;

    private float _nitroAmount;
    private bool _isNitroActive;
    private bool _frontColliderWasEnabled = true;

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
        if (playerPhysics != null)
            playerPhysics.speedMultiplier = 2f;
        if (frontTriggerCollider != null)
        {
            _frontColliderWasEnabled = frontTriggerCollider.enabled;
            frontTriggerCollider.enabled = false;
        }
    }

    void EndNitro()
    {
        _isNitroActive = false;
        if (playerPhysics != null)
            playerPhysics.speedMultiplier = 1f;
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

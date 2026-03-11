using UnityEngine;

/// <summary>
/// Visual effect for shield - shows/hides shield bubble around player.
/// Automatically syncs with PlayerInvincibility state.
/// </summary>
public class ShieldVisual : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    [Tooltip("PlayerInvincibility component to monitor")]
    public PlayerInvincibility playerInvincibility;

    [Header("=== VISUAL SETTINGS ===")]
    [Tooltip("Shield visual GameObject (sphere, particle effect, etc.)")]
    public GameObject shieldVisual;

    [Tooltip("Pulse animation speed")]
    public float pulseSpeed = 2f;

    [Tooltip("Pulse scale range")]
    public Vector2 pulseScaleRange = new Vector2(0.95f, 1.05f);

    private Vector3 _originalScale;
    private bool _wasInvincible = false;

    void Start()
    {
        // Auto-find PlayerInvincibility if not assigned
        if (playerInvincibility == null)
        {
            playerInvincibility = GetComponent<PlayerInvincibility>();
        }

        // Store original scale
        if (shieldVisual != null)
        {
            _originalScale = shieldVisual.transform.localScale;
            shieldVisual.SetActive(false); // Hide initially
        }
    }

    void Update()
    {
        if (playerInvincibility == null || shieldVisual == null) return;

        bool isInvincible = playerInvincibility.IsInvincible;

        // Show/hide shield visual
        if (isInvincible != _wasInvincible)
        {
            shieldVisual.SetActive(isInvincible);
            _wasInvincible = isInvincible;

            if (isInvincible)
            {
                Debug.Log("[ShieldVisual] Shield visual activated!");
            }
            else
            {
                Debug.Log("[ShieldVisual] Shield visual deactivated!");
            }
        }

        // Pulse animation when active
        if (isInvincible)
        {
            float pulse = Mathf.Lerp(pulseScaleRange.x, pulseScaleRange.y, 
                (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
            shieldVisual.transform.localScale = _originalScale * pulse;
        }
    }
}

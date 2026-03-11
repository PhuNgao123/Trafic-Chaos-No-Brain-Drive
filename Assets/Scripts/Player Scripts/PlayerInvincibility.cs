using UnityEngine;

/// <summary>
/// Manages player invincibility state (optional component).
/// Can be used for temporary invincibility after taking damage or during power-ups.
/// </summary>
public class PlayerInvincibility : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Duration of invincibility in seconds")]
    private float invincibilityDuration = 2f;

    private float invincibilityTimer = 0f;

    /// <summary>
    /// Returns true if the player is currently invincible
    /// </summary>
    public bool IsInvincible => invincibilityTimer > 0f;

    void Update()
    {
        // Count down invincibility timer
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                invincibilityTimer = 0f;
                Debug.Log("[PlayerInvincibility] Invincibility ended");
            }
        }
    }

    /// <summary>
    /// Activates invincibility for the specified duration
    /// </summary>
    /// <param name="duration">Duration in seconds (uses default if not specified)</param>
    public void ActivateInvincibility(float duration = -1f)
    {
        if (duration < 0f)
        {
            duration = invincibilityDuration;
        }

        invincibilityTimer = duration;
        Debug.Log($"[PlayerInvincibility] Invincibility activated for {duration} seconds");
    }

    /// <summary>
    /// Deactivates invincibility immediately
    /// </summary>
    public void DeactivateInvincibility()
    {
        invincibilityTimer = 0f;
        Debug.Log("[PlayerInvincibility] Invincibility deactivated");
    }
}

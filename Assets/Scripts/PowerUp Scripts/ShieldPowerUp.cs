using UnityEngine;

/// <summary>
/// Shield power-up that grants temporary invincibility to the player.
/// Disables collision damage and optionally shows a visual shield effect.
/// </summary>
public class ShieldPowerUp : MonoBehaviour
{
    [Header("=== SHIELD SETTINGS ===")]
    [Tooltip("Duration of shield invincibility in seconds")]
    public float shieldDuration = 5f;

    [Tooltip("Should the power-up rotate?")]
    public bool rotatePickup = true;

    [Tooltip("Rotation speed")]
    public float rotationSpeed = 100f;

    [Header("=== AUDIO ===")]
    [Tooltip("Sound effect name to play when picked up")]
    public string pickupSoundName = "PowerUpCollected";

    void Update()
    {
        // Rotate the power-up for visual effect
        if (rotatePickup)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if player picked up the shield
        PlayerPhysics playerPhysics = other.GetComponent<PlayerPhysics>();
        if (playerPhysics == null)
        {
            // Try to get from parent
            playerPhysics = other.GetComponentInParent<PlayerPhysics>();
        }

        if (playerPhysics != null)
        {
            // Get PlayerInvincibility component
            PlayerInvincibility invincibility = playerPhysics.GetComponent<PlayerInvincibility>();
            
            if (invincibility != null)
            {
                // Activate shield
                invincibility.ActivateInvincibility(shieldDuration);

                // Play pickup sound
                if (AudioManager.Instance != null && !string.IsNullOrEmpty(pickupSoundName))
                {
                    AudioManager.Instance.PlaySFX(pickupSoundName);
                }

                // Destroy the power-up
                Destroy(gameObject);
            }
        }
    }
}

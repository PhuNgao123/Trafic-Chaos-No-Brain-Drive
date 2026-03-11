using UnityEngine;

/// <summary>
/// Health power-up that restores player's HP.
/// </summary>
public class HealthPowerUp : MonoBehaviour
{
    [Header("=== HEALTH SETTINGS ===")]
    [Tooltip("Amount of health to restore")]
    public float healAmount = 30f;

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
        Debug.Log($"[HealthPowerUp] Trigger entered by: {other.gameObject.name}");
        
        // Check if player picked up the health
        PlayerPhysics playerPhysics = other.GetComponent<PlayerPhysics>();
        if (playerPhysics == null)
        {
            // Try to get from parent
            playerPhysics = other.GetComponentInParent<PlayerPhysics>();
        }

        if (playerPhysics != null)
        {
            Debug.Log("[HealthPowerUp] Player detected!");
            
            // Calculate heal amount (don't exceed max health)
            float currentHealth = playerPhysics.currentHealth;
            float maxHealth = playerPhysics.maxHealth;
            float actualHeal = Mathf.Min(healAmount, maxHealth - currentHealth);
            
            if (actualHeal > 0)
            {
                // Heal player directly
                playerPhysics.Heal(actualHeal);
                
                Debug.Log($"[HealthPowerUp] ✓ Healed player for {actualHeal} HP! Current health: {playerPhysics.currentHealth}/{maxHealth}");

                // Play pickup sound
                if (AudioManager.Instance != null && !string.IsNullOrEmpty(pickupSoundName))
                {
                    AudioManager.Instance.PlaySFX(pickupSoundName);
                }

                // Destroy the power-up
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("[HealthPowerUp] Player already at full health!");
            }
        }
    }
}

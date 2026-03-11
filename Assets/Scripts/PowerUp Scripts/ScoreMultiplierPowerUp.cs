using UnityEngine;

/// <summary>
/// Score multiplier power-up (e.g., x10 score boost).
/// Activates score multiplier for a limited time.
/// </summary>
public class ScoreMultiplierPowerUp : MonoBehaviour
{
    [Header("=== MULTIPLIER SETTINGS ===")]
    [Tooltip("Score multiplier value (e.g., 10 for x10)")]
    public float multiplierValue = 10f;

    [Tooltip("Duration of multiplier in seconds")]
    public float duration = 10f;

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
        Debug.Log($"[ScoreMultiplierPowerUp] Trigger entered by: {other.gameObject.name}");
        
        // Check if player picked up the multiplier
        PlayerPhysics playerPhysics = other.GetComponent<PlayerPhysics>();
        if (playerPhysics == null)
        {
            // Try to get from parent
            playerPhysics = other.GetComponentInParent<PlayerPhysics>();
        }

        if (playerPhysics != null)
        {
            Debug.Log("[ScoreMultiplierPowerUp] Player detected!");
            
            // Get ScoreMultiplierController component
            ScoreMultiplierController multiplierController = playerPhysics.GetComponent<ScoreMultiplierController>();
            
            if (multiplierController != null)
            {
                // Activate multiplier
                multiplierController.ActivateMultiplier(multiplierValue, duration);
                Debug.Log($"[ScoreMultiplierPowerUp] ✓ Score multiplier activated! x{multiplierValue} for {duration} seconds!");

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
                Debug.LogWarning("[ScoreMultiplierPowerUp] ScoreMultiplierController component not found on player! Please add it to CarPhysic GameObject.");
            }
        }
    }
}

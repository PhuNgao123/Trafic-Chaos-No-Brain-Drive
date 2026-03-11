using UnityEngine;

// Handles player collision detection for nitro destruction
// Attach this to the trigger collider for collision detection (small trigger at front of car)
public class PlayerController : MonoBehaviour
{
    public NitroController nitroController; // Reference to check if nitro is active

    void Start()
    {
        // Auto-find NitroController if not assigned
        if (nitroController == null)
        {
            // Look in parent (player car)
            if (transform.parent != null)
                nitroController = transform.parent.GetComponent<NitroController>();
            
            // If still null, search in scene
            if (nitroController == null)
                nitroController = FindFirstObjectByType<NitroController>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check collision with vehicle
        if (other.CompareTag("Vehicle"))
        {
            // If nitro is active, destroy the bot car
            if (nitroController != null && nitroController.IsNitroActive)
            {
                Debug.Log($"Nitro active! Destroying {other.name}");
                Destroy(other.gameObject);
                return;
            }
            
            // Normal collision - let PlayerDamageHandler handle damage
            // Don't trigger game over here, let health system handle it
            Debug.Log($"[PlayerController] Collision with {other.name} - damage will be handled by PlayerDamageHandler");
        }
    }
}

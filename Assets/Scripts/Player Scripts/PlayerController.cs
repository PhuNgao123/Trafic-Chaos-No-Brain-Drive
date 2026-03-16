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
        // Check collision with vehicle or police
        if (other.CompareTag("Vehicle") || other.CompareTag("Police"))
        {
            // If nitro is active, destroy the bot car
            if (nitroController != null && nitroController.IsNitroActive)
            {
                Destroy(other.gameObject);
                return;
            }
        }
    }
}

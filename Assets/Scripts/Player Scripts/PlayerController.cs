using UnityEngine;

// Handles player game over detection
// Attach this to the trigger collider for gameover detection (small trigger at front of car)
public class PlayerController : MonoBehaviour
{
    public NitroController nitroController; // Reference to check if nitro is active
    
    private bool _hasTriggeredGameOver = false;

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
        // Only trigger game over once
        if (_hasTriggeredGameOver) return;
        
        // Check collision with vehicle
        if (other.CompareTag("Vehicle"))
        {
            // If nitro is active, destroy the bot car instead of game over
            if (nitroController != null && nitroController.IsNitroActive)
            {
                Debug.Log($"Nitro active! Destroying {other.name}");
                Destroy(other.gameObject);
                return; // Don't trigger game over
            }
            
            // Normal collision - trigger game over
            _hasTriggeredGameOver = true;
            
            if (GameLogicController.Instance != null)
            {
                // Pass CarPhysic object (parent of this trigger)
                GameObject carPhysic = transform.parent != null 
                    ? transform.parent.gameObject 
                    : gameObject;
                    
                GameLogicController.Instance.TriggerGameOver(other.gameObject, carPhysic);
            }
        }
    }
}

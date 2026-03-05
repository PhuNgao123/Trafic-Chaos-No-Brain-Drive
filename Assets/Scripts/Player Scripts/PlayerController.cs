using UnityEngine;

// Handles player game over detection
// Attach this to the trigger collider for gameover detection (small trigger at front of car)
public class PlayerController : MonoBehaviour
{
    [Header("Debug")]
    public bool showDebugLogs = false;

    private bool _hasTriggeredGameOver = false;

    void OnTriggerEnter(Collider other)
    {
        if (showDebugLogs)
            Debug.Log($"[PlayerController] Trigger on {gameObject.name} detected: {other.gameObject.name} (Tag: {other.tag})");

        // Only trigger game over once
        if (_hasTriggeredGameOver) return;
        
        // Check collision with vehicle
        if (other.CompareTag("Vehicle"))
        {
            if (showDebugLogs)
                Debug.Log($"[PlayerController] GAME OVER triggered by {gameObject.name}!");

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

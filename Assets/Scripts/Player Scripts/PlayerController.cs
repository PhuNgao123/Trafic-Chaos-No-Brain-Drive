using UnityEngine;

// Handles player game over detection
// Attach this to the trigger collider for gameover detection (small trigger at front of car)
public class PlayerController : MonoBehaviour
{
    private bool _hasTriggeredGameOver = false;

    void OnTriggerEnter(Collider other)
    {
        // Only trigger game over once
        if (_hasTriggeredGameOver) return;
        
        // Check collision with vehicle
        if (other.CompareTag("Vehicle"))
        {
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

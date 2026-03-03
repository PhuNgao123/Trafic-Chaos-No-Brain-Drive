using UnityEngine;

// Handles player game logic (collision detection, game over, etc.)
// Works alongside PlayerPhysics which handles movement
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
                GameLogicController.Instance.TriggerGameOver(other.gameObject, gameObject);
            }
        }
    }
}

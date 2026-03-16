using UnityEngine;

// Police vehicle behavior - handles collision with player differently
public class PoliceVehicle : MonoBehaviour
{
    [Header("Police Settings")]
    public bool triggerGameOverOnHit = true; // Police collision causes game over
    
    public static bool LastGameOverWasPolice { get; private set; } = false;
    
    void OnCollisionEnter(Collision collision)
    {
        // Collision with Player - PlayerDamageHandler handles TriggerGameOver directly
        // We just set the flag here so penalty logic knows it was police
        if (collision.gameObject.CompareTag("Player"))
        {
            LastGameOverWasPolice = true;
        }
        
        if (collision.gameObject.CompareTag("Vehicle"))
        {
            if (GameLogicController.Instance != null)
                GameLogicController.Instance.OnVehicleCollision(gameObject, collision.gameObject);
        }
        
        if (collision.gameObject.CompareTag("Police"))
        {
            if (GameLogicController.Instance != null)
                GameLogicController.Instance.OnVehicleCollision(gameObject, collision.gameObject);
        }
    }
    
    // Reset police game over flag when game starts
    public static void ResetPoliceGameOverFlag()
    {
        LastGameOverWasPolice = false;
    }

    // Force-set the flag (called from GameLogicController when collided vehicle tag is Police)
    public static void SetPoliceGameOverFlag()
    {
        LastGameOverWasPolice = true;
    }
}
using UnityEngine;

/// <summary>
/// Handles collision detection with enemy vehicles and applies damage to the player.
/// Integrates with PlayerHealth for damage application and GameLogicController for game over.
/// Supports invincibility state through PlayerInvincibility component.
/// </summary>
[RequireComponent(typeof(PlayerHealth))]
public class PlayerDamageHandler : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private PlayerInvincibility playerInvincibility;
    private GameObject lastCollidedVehicle;

    void Awake()
    {
        Debug.Log($"[PlayerDamageHandler] Awake called on {gameObject.name}");
        
        // Get reference to PlayerHealth component on the same GameObject
        playerHealth = GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogError("[PlayerDamageHandler] PlayerHealth component not found!");
        }
        else
        {
            Debug.Log($"[PlayerDamageHandler] PlayerHealth found! Max health: {playerHealth.maxHealth}");
        }

        // Get reference to PlayerInvincibility component (optional)
        playerInvincibility = GetComponent<PlayerInvincibility>();
    }

    void Start()
    {
        // Subscribe to OnHealthDepleted event to trigger game over
        if (playerHealth != null)
        {
            playerHealth.OnHealthDepleted += HandleHealthDepleted;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from event to prevent memory leaks
        if (playerHealth != null)
        {
            playerHealth.OnHealthDepleted -= HandleHealthDepleted;
        }
    }

    /// <summary>
    /// Detects collisions with enemy vehicles and applies damage.
    /// Called by Unity's physics system when a collision occurs.
    /// Damage is ignored if player is invincible.
    /// </summary>
    /// <param name="collision">The collision data</param>
    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision);
    }

    /// <summary>
    /// Public method to handle collision (can be called from child CollisionForwarder)
    /// </summary>
    public void HandleCollision(Collision collision)
    {
        // Debug: Log all collisions with detailed info
        Debug.Log($"[PlayerDamageHandler] ===== COLLISION DETECTED =====");
        Debug.Log($"[PlayerDamageHandler] GameObject: {collision.gameObject.name}");
        Debug.Log($"[PlayerDamageHandler] Tag: {collision.gameObject.tag}");
        Debug.Log($"[PlayerDamageHandler] Layer: {LayerMask.LayerToName(collision.gameObject.layer)}");
        Debug.Log($"[PlayerDamageHandler] Contact points: {collision.contactCount}");

        // Try to find "Vehicle" tag on the collider's GameObject or its parent
        GameObject vehicleObject = null;

        if (collision.gameObject.CompareTag("Vehicle"))
        {
            vehicleObject = collision.gameObject;
            Debug.Log($"[PlayerDamageHandler] ✓ Found Vehicle tag on collision object");
        }
        else if (collision.transform.parent != null && collision.transform.parent.CompareTag("Vehicle"))
        {
            vehicleObject = collision.transform.parent.gameObject;
            Debug.Log($"[PlayerDamageHandler] ✓ Found Vehicle tag on parent object");
        }
        else
        {
            Debug.Log($"[PlayerDamageHandler] ✗ No Vehicle tag found (not an enemy vehicle)");
        }

        if (vehicleObject != null)
        {
            // Kiểm tra invincibility - nếu đang bất tử thì bỏ qua damage
            if (playerInvincibility != null && playerInvincibility.IsInvincible)
            {
                Debug.Log("[PlayerDamageHandler] Player is invincible! Damage ignored.");
                return;
            }

            // Store reference to the colliding vehicle for game over handling
            lastCollidedVehicle = vehicleObject;

            // Attempt to retrieve VehicleDamage component from the vehicle or its children
            VehicleDamage vehicleDamage = vehicleObject.GetComponent<VehicleDamage>();
            if (vehicleDamage == null)
            {
                vehicleDamage = vehicleObject.GetComponentInChildren<VehicleDamage>();
            }

            if (vehicleDamage != null)
            {
                Debug.Log($"[PlayerDamageHandler] ✓ VehicleDamage found! Damage value: {vehicleDamage.damage}");
                
                // Apply damage to player health
                playerHealth.TakeDamage(vehicleDamage.damage);

                Debug.Log($"[PlayerDamageHandler] ✓ Player took {vehicleDamage.damage} damage from {vehicleObject.name}. Current health: {playerHealth.currentHealth}");
            }
            else
            {
                Debug.LogWarning($"[PlayerDamageHandler] ✗ Vehicle {vehicleObject.name} has no VehicleDamage component!");
            }
        }
        
        Debug.Log($"[PlayerDamageHandler] ===== END COLLISION =====");
    }

    /// <summary>
    /// Handles the health depleted event by triggering game over.
    /// </summary>
    private void HandleHealthDepleted()
    {
        Debug.Log("[PlayerDamageHandler] Health depleted! Triggering game over...");

        // Trigger game over through GameLogicController
        if (GameLogicController.Instance != null)
        {
            // Pass the last collided vehicle and this GameObject (player) to TriggerGameOver
            GameLogicController.Instance.TriggerGameOver(lastCollidedVehicle, gameObject);
        }
        else
        {
            Debug.LogError("[PlayerDamageHandler] GameLogicController.Instance is null!");
        }
    }
}


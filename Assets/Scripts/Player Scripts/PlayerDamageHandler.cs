using UnityEngine;

/// <summary>
/// Handles collision detection with enemy vehicles and applies damage to the player.
/// Integrates with PlayerPhysics for damage application and GameLogicController for game over.
/// Supports invincibility state through PlayerInvincibility component.
/// </summary>
[RequireComponent(typeof(PlayerPhysics))]
public class PlayerDamageHandler : MonoBehaviour
{
    private PlayerPhysics playerPhysics;
    private PlayerInvincibility playerInvincibility;
    private GameObject lastCollidedVehicle;
    private bool _hitPolice = false; // Track police hit independently

    void Awake()
    {
        // Get reference to PlayerPhysics component on the same GameObject
        playerPhysics = GetComponent<PlayerPhysics>();

        // Get reference to PlayerInvincibility component (optional)
        playerInvincibility = GetComponent<PlayerInvincibility>();
    }

    void Start()
    {
        // Subscribe to OnHealthDepleted event to trigger game over
        if (playerPhysics != null)
        {
            playerPhysics.OnHealthDepleted += HandleHealthDepleted;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from event to prevent memory leaks
        if (playerPhysics != null)
        {
            playerPhysics.OnHealthDepleted -= HandleHealthDepleted;
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
        GameObject vehicleObject = null;
        if (collision.gameObject.CompareTag("Vehicle") || collision.gameObject.CompareTag("Police"))
        {
            vehicleObject = collision.gameObject;
        }
        else if (collision.transform.parent != null && (collision.transform.parent.CompareTag("Vehicle") || collision.transform.parent.CompareTag("Police")))
        {
            vehicleObject = collision.transform.parent.gameObject;
        }

        if (vehicleObject == null) return;

        if (playerInvincibility != null && playerInvincibility.IsInvincible) return;

        lastCollidedVehicle = vehicleObject;

        // Track police hit independently - regardless of crash state
        if (vehicleObject.CompareTag("Police"))
        {
            _hitPolice = true;
            Debug.Log($"[Police] Player hit police! HP before: {playerPhysics?.currentHealth}/{playerPhysics?.maxHealth}");
        }

        // Both Police and Vehicle: use VehicleDamage component on the prefab
        VehicleDamage vehicleDamage = vehicleObject.GetComponent<VehicleDamage>();
        if (vehicleDamage == null)
            vehicleDamage = vehicleObject.GetComponentInChildren<VehicleDamage>();

        if (vehicleDamage != null && playerPhysics != null)
        {
            if (vehicleObject.CompareTag("Police"))
                Debug.Log($"[Police] Player hit police! Damage: {vehicleDamage.damage}, HP before: {playerPhysics.currentHealth}/{playerPhysics.maxHealth}");
            playerPhysics.TakeDamage(vehicleDamage.damage);
        }
    }

    /// <summary>
    /// Handles the health depleted event by triggering game over.
    /// </summary>
    private void HandleHealthDepleted()
    {
        if (GameLogicController.Instance != null)
        {
            // Pass police hit flag so penalty is applied correctly regardless of vehicle crash state
            GameLogicController.Instance.TriggerGameOver(lastCollidedVehicle, gameObject, _hitPolice);
        }
    }
}

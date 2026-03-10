using UnityEngine;

/// <summary>
/// Forwards collision events from child GameObject to parent.
/// Attach this to CarPhysic child to forward collisions to Player root.
/// </summary>
public class CollisionForwarder : MonoBehaviour
{
    private PlayerDamageHandler damageHandler;

    void Awake()
    {
        // Get PlayerDamageHandler from parent GameObject
        damageHandler = GetComponentInParent<PlayerDamageHandler>();

        if (damageHandler == null)
        {
            Debug.LogError("[CollisionForwarder] PlayerDamageHandler not found in parent!");
        }
    }

    /// <summary>
    /// Forwards OnCollisionEnter to parent's PlayerDamageHandler
    /// </summary>
    void OnCollisionEnter(Collision collision)
    {
        if (damageHandler != null)
        {
            // Forward collision to parent
            damageHandler.HandleCollision(collision);
        }
    }
}

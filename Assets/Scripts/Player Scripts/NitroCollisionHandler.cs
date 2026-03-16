using UnityEngine;

// Handles collisions during nitro - pushes and destroys bot cars on contact
// Attach this to the player car's main collider (body)
public class NitroCollisionHandler : MonoBehaviour
{
    public NitroController nitroController;
    
    [Header("Collision Force")]
    public float pushForce = 50f;  // Force to push cars away
    public float upwardForce = 10f;  // Upward force for dramatic effect
    public bool destroyAfterPush = true;  // Destroy cars after pushing them
    public float destroyDelay = 0.5f;  // Delay before destroying (let them fly first)

    void Start()
    {
        // Auto-find NitroController
        if (nitroController == null)
            nitroController = GetComponent<NitroController>();
        
        if (nitroController == null)
            nitroController = FindFirstObjectByType<NitroController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        // If nitro is active and we hit a vehicle, push it away
        if (nitroController != null && nitroController.IsNitroActive)
        {
            if (collision.gameObject.CompareTag("Vehicle") || collision.gameObject.CompareTag("Police"))
            {
                Debug.Log($"Nitro collision! Pushing {collision.gameObject.name}");
                
                // Get the bot car's rigidbody
                Rigidbody botRb = collision.gameObject.GetComponent<Rigidbody>();
                if (botRb != null)
                {
                    // Calculate push direction (away from player)
                    Vector3 pushDirection = (collision.transform.position - transform.position).normalized;
                    
                    // Make sure rigidbody is not kinematic
                    botRb.isKinematic = false;
                    botRb.useGravity = true;
                    
                    // Apply massive force to push the car away
                    Vector3 force = pushDirection * pushForce;
                    force.y = upwardForce;  // Add upward force for dramatic effect
                    
                    botRb.AddForce(force, ForceMode.Impulse);
                    
                    // Add random spin for more chaos
                    botRb.AddTorque(Random.insideUnitSphere * 20f, ForceMode.Impulse);
                    
                    // Optionally destroy after a delay (let it fly first)
                    if (destroyAfterPush)
                    {
                        Destroy(collision.gameObject, destroyDelay);
                    }
                }
                else
                {
                    // No rigidbody, just destroy immediately
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}

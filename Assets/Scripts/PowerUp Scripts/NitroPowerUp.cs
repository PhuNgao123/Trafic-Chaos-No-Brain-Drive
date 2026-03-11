using UnityEngine;

/// <summary>
/// Nitro power-up that adds nitro to player when collected
/// Attach to nitro powerup prefab and set tag to "NitroPickup"
/// </summary>
public class NitroPowerUp : MonoBehaviour
{
    [Header("Nitro Settings")]
    [Tooltip("Amount of nitro to add (0-100)")]
    public float nitroAmount = 50f;
    
    [Header("Audio (Optional)")]
    public AudioClip pickupSound;
    
    [Header("VFX (Optional)")]
    public GameObject pickupEffect;
    
    void Start()
    {
        // Ensure this object has the correct tag
        if (!gameObject.CompareTag("NitroPickup"))
        {
            gameObject.tag = "NitroPickup";
        }
        
        // Ensure we have a trigger collider
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Check if player collected it - look for any player-related tag
        if (other.CompareTag("Player") || other.CompareTag("Car Hit") || other.CompareTag("Car"))
        {
            NitroController nitroController = null;
            
            // Try to find NitroController in multiple ways
            // 1. Check the colliding object itself
            nitroController = other.GetComponent<NitroController>();
            
            // 2. Check parent if not found
            if (nitroController == null && other.transform.parent != null)
            {
                nitroController = other.GetComponentInParent<NitroController>();
            }
            
            // 3. Check children if not found
            if (nitroController == null)
            {
                nitroController = other.GetComponentInChildren<NitroController>();
            }
            
            // 4. Find by tag "Player" in scene as last resort
            if (nitroController == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    nitroController = player.GetComponent<NitroController>();
                }
            }
            
            if (nitroController != null)
            {
                // Add nitro
                nitroController.AddNitro(nitroAmount);
                
                // Play sound
                if (pickupSound != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(pickupSound.name);
                }
                
                // Spawn VFX
                if (pickupEffect != null)
                {
                    Instantiate(pickupEffect, transform.position, Quaternion.identity);
                }
                
                // Destroy powerup
                Destroy(gameObject);
            }
        }
    }
}

using UnityEngine;

// Invisible trigger placed on the sides of the player car. When a bot (Vehicle) is inside, stacks nitro (slipstream).
// Also used by NitroController to auto-create side zones if not present in prefab.
public class NitroStackZone : MonoBehaviour
{
    public NitroController nitroController;
    public float nitroAddPerSecond = 15f;
    public bool enableDebugLogs = false; // Enable to see debug messages

    void OnTriggerStay(Collider other)
    {
        if (nitroController == null)
        {
            if (enableDebugLogs)
                Debug.LogWarning("NitroStackZone: nitroController is null!");
            return;
        }
        
        if (GameLogicController.Instance != null && GameLogicController.Instance.isGameOver) return;

        if (other.CompareTag("Vehicle") || other.CompareTag("Police"))
        {
            nitroController.AddNitro(nitroAddPerSecond * Time.deltaTime);
            if (enableDebugLogs)
                Debug.Log($"NitroStackZone: Adding nitro from {other.name}, current: {nitroController.NitroPercent * 100f}%");
        }
        else if (enableDebugLogs)
        {
            Debug.Log($"NitroStackZone: Detected {other.name} but tag is '{other.tag}', not 'Vehicle'");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (nitroController == null) return;
        
        if (other.CompareTag("NitroPickup"))
        {
            nitroController.AddNitroPickup();
            Destroy(other.gameObject);
            if (enableDebugLogs)
                Debug.Log("NitroStackZone: Picked up nitro pickup!");
        }
    }
}

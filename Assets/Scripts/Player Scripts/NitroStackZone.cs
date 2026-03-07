using UnityEngine;

// Invisible trigger placed on the sides of the player car. When a bot (Vehicle) is inside, stacks nitro (slipstream).
// Also used by NitroController to auto-create side zones if not present in prefab.
public class NitroStackZone : MonoBehaviour
{
    public NitroController nitroController;
    public float nitroAddPerSecond = 15f;

    void OnTriggerStay(Collider other)
    {
        if (nitroController == null) return;
        if (GameLogicController.Instance != null && GameLogicController.Instance.isGameOver) return;

        if (other.CompareTag("Vehicle"))
            nitroController.AddNitro(nitroAddPerSecond * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (nitroController == null) return;
        if (other.CompareTag("NitroPickup"))
        {
            nitroController.AddNitroPickup();
            Destroy(other.gameObject);
        }
    }
}

using UnityEngine;

// Applies upgrade bonus to a spawned vehicle instance.
// Prefab stats are never modified - UI calculates display values separately.
public class VehicleUpgrader : MonoBehaviour
{
    public static void ApplyUpgrade(GameObject vehicleInstance)
    {
        VehicleInfo info = vehicleInstance.GetComponent<VehicleInfo>();
        if (info == null || !info.isUpgraded) return;

        float m = GarageManager.Instance != null ? GarageManager.Instance.upgradeMultiplier : 1.2f;

        PlayerPhysics physics = vehicleInstance.GetComponentInChildren<PlayerPhysics>();
        if (physics != null)
        {
            physics.maxSpeed         *= m;
            physics.minSpeed         *= m;
            physics.acceleration     *= m;
            physics.steerSpeed       *= m;
            physics.maxSteerVelocity *= m;
            physics.maxHealth        *= m;
        }

        NitroController nitro = vehicleInstance.GetComponentInChildren<NitroController>();
        if (nitro != null)
        {
            nitro.maxNitroAmount              *= m;
            nitro.nitroSlipstreamAddPerSecond *= m;
            nitro.nitroPickupAmount           *= m;
            nitro.nitroDrainPerSecond         /= m;
        }
    }
}

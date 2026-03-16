using UnityEngine;

// Applies +20% upgrade bonus to all beneficial stats when the vehicle is marked as upgraded.
// Attach to the same root GameObject as VehicleInfo, or call ApplyUpgrade() from PlayerSpawner.
public class VehicleUpgrader : MonoBehaviour
{
    private const float UPGRADE_MULTIPLIER = 1.2f;

    // Call this after spawning the vehicle instance if VehicleInfo.isUpgraded == true
    public static void ApplyUpgrade(GameObject vehicleInstance)
    {
        VehicleInfo info = vehicleInstance.GetComponent<VehicleInfo>();
        if (info == null || !info.isUpgraded) return;

        // PlayerPhysics stats
        PlayerPhysics physics = vehicleInstance.GetComponentInChildren<PlayerPhysics>();
        if (physics != null)
        {
            physics.maxSpeed          *= UPGRADE_MULTIPLIER;
            physics.minSpeed          *= UPGRADE_MULTIPLIER;
            physics.acceleration      *= UPGRADE_MULTIPLIER;
            physics.steerSpeed        *= UPGRADE_MULTIPLIER;
            physics.maxSteerVelocity  *= UPGRADE_MULTIPLIER;
            physics.maxHealth         *= UPGRADE_MULTIPLIER;
        }

        // NitroController stats
        NitroController nitro = vehicleInstance.GetComponentInChildren<NitroController>();
        if (nitro != null)
        {
            nitro.maxNitroAmount              *= UPGRADE_MULTIPLIER;
            nitro.nitroSlipstreamAddPerSecond *= UPGRADE_MULTIPLIER;
            nitro.nitroPickupAmount           *= UPGRADE_MULTIPLIER;
            // Reduce drain = benefit (drain 20% less)
            nitro.nitroDrainPerSecond         /= UPGRADE_MULTIPLIER;
        }
    }
}

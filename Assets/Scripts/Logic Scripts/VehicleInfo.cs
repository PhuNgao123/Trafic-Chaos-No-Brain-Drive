using UnityEngine;

public class VehicleInfo : MonoBehaviour
{
    [Header("Vehicle Info")]
    public string vehicleName = "Car";
    public int price = 0;
    public bool isOwned = false;

    [Header("Upgrade")]
    public int upgradeCost = 50; // diamonds required
    public bool isUpgraded = false;

    [Header("Repair")]
    [Tooltip("Seconds to wait after paying repair cost (default 600 = 10 minutes)")]
    public float repairDurationSeconds = 600f;
    public bool isReady = true;
    [Tooltip("Repair cost in coins. Leave 0 to auto-calculate (10% of price)")]
    public int repairCost = 0;

    [Header("Display")]
    public Sprite vehicleIcon;

    // If repairCost is set manually use it, otherwise fallback to 10% of price
    public int GetRepairCost() => repairCost > 0 ? repairCost : Mathf.Max(1, Mathf.RoundToInt(price * 0.1f));
}
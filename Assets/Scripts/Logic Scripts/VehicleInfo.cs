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
    public bool isReady = true; // false = xe bị hỏng, cần sửa trước khi dùng

    [Header("Display")]
    public Sprite vehicleIcon;

    // Repair cost = 10% of purchase price (minimum 1)
    public int GetRepairCost() => Mathf.Max(1, Mathf.RoundToInt(price * 0.1f));
}
using UnityEngine;

// Component gắn vào prefab xe để chứa thông tin bổ sung
// Stats sẽ lấy trực tiếp từ PlayerPhysics
public class VehicleInfo : MonoBehaviour
{
    [Header("Display Info")]
    public string vehicleName = "Car";
    public Sprite vehicleIcon; // Icon hiển thị trong UI
    
    [Header("Purchase")]
    public int price = 0; // 0 = free/unlocked by default
}

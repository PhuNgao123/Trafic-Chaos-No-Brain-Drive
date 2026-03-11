using UnityEngine;

public class VehicleInfo : MonoBehaviour
{
    [Header("Vehicle Info")]
    public string vehicleName = "Car";
    public int price = 0;
    public bool isOwned = false;
    
    [Header("Display")]
    public Sprite vehicleIcon;
}
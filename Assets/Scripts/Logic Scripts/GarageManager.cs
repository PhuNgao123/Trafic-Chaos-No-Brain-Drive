using UnityEngine;
using System.Collections.Generic;

public class GarageManager : MonoBehaviour
{
    public static GarageManager Instance { get; private set; }

    [Header("Vehicle Prefabs")]
    [Tooltip("Kéo các prefab xe vào đây. Prefab phải có component VehicleInfo và PlayerPhysics")]
    public List<GameObject> vehiclePrefabs = new List<GameObject>();
    
    [Header("Current Selection")]
    public int selectedVehicleIndex = 0;
    
    private const string SELECTED_VEHICLE_KEY = "SelectedVehicle";
    private const string UNLOCKED_VEHICLES_KEY = "UnlockedVehicles";
    private HashSet<int> unlockedVehicles = new HashSet<int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadData()
    {
        // Load selected vehicle
        selectedVehicleIndex = PlayerPrefs.GetInt(SELECTED_VEHICLE_KEY, 0);
        
        // Load unlocked vehicles
        string unlockedData = PlayerPrefs.GetString(UNLOCKED_VEHICLES_KEY, "0"); // First vehicle unlocked by default
        string[] unlockedIndices = unlockedData.Split(',');
        
        unlockedVehicles.Clear();
        foreach (string indexStr in unlockedIndices)
        {
            if (int.TryParse(indexStr, out int index))
            {
                unlockedVehicles.Add(index);
            }
        }
    }

    void SaveData()
    {
        // Save selected vehicle
        PlayerPrefs.SetInt(SELECTED_VEHICLE_KEY, selectedVehicleIndex);
        
        // Save unlocked vehicles
        List<string> unlockedIndices = new List<string>();
        foreach (int index in unlockedVehicles)
        {
            unlockedIndices.Add(index.ToString());
        }
        PlayerPrefs.SetString(UNLOCKED_VEHICLES_KEY, string.Join(",", unlockedIndices));
        PlayerPrefs.Save();
    }

    public GameObject GetSelectedVehiclePrefab()
    {
        if (selectedVehicleIndex >= 0 && selectedVehicleIndex < vehiclePrefabs.Count)
        {
            return vehiclePrefabs[selectedVehicleIndex];
        }
        return null;
    }

    public void SelectVehicle(int index)
    {
        if (index >= 0 && index < vehiclePrefabs.Count)
        {
            selectedVehicleIndex = index;
            SaveData();
        }
    }

    public bool PurchaseVehicle(int index)
    {
        if (index < 0 || index >= vehiclePrefabs.Count)
            return false;

        // Already unlocked
        if (IsVehicleUnlocked(index))
            return true;

        GameObject prefab = vehiclePrefabs[index];
        VehicleInfo info = prefab.GetComponent<VehicleInfo>();
        
        if (info == null)
        {
            Debug.LogError($"Vehicle prefab at index {index} missing VehicleInfo component!");
            return false;
        }

        // Try to spend coins
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.SpendCoins(info.price))
        {
            unlockedVehicles.Add(index);
            SaveData();
            Debug.Log($"Purchased {info.vehicleName} for {info.price} coins");
            return true;
        }

        Debug.Log($"Not enough coins to purchase {info.vehicleName}");
        return false;
    }

    public bool IsVehicleUnlocked(int index)
    {
        return unlockedVehicles.Contains(index);
    }

    public int GetVehicleCount()
    {
        return vehiclePrefabs.Count;
    }

    public GameObject GetVehiclePrefabAt(int index)
    {
        if (index >= 0 && index < vehiclePrefabs.Count)
        {
            return vehiclePrefabs[index];
        }
        return null;
    }
    
    // Helper: lấy VehicleInfo từ prefab
    public VehicleInfo GetVehicleInfoAt(int index)
    {
        GameObject prefab = GetVehiclePrefabAt(index);
        return prefab != null ? prefab.GetComponent<VehicleInfo>() : null;
    }
    
    // Helper: lấy PlayerPhysics từ prefab
    public PlayerPhysics GetVehiclePhysicsAt(int index)
    {
        GameObject prefab = GetVehiclePrefabAt(index);
        return prefab != null ? prefab.GetComponentInChildren<PlayerPhysics>() : null;
    }
}

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
    private const string OWNED_VEHICLES_KEY = "OwnedVehicles";

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
        selectedVehicleIndex = PlayerPrefs.GetInt(SELECTED_VEHICLE_KEY, 0);
        
        // Ensure first vehicle is always owned
        SetVehicleOwned(0, true);
        
        // Load owned vehicles
        LoadOwnedVehicles();
        
        // Ensure selected vehicle is owned
        if (!IsVehicleOwned(selectedVehicleIndex))
        {
            selectedVehicleIndex = 0;
            SaveData();
        }
    }
    
    void LoadOwnedVehicles()
    {
        string ownedData = PlayerPrefs.GetString(OWNED_VEHICLES_KEY, "");
        if (!string.IsNullOrEmpty(ownedData))
        {
            string[] ownedIndices = ownedData.Split(',');
            foreach (string indexStr in ownedIndices)
            {
                if (int.TryParse(indexStr, out int index))
                {
                    SetVehicleOwned(index, true);
                }
            }
        }
    }

    void SaveData()
    {
        PlayerPrefs.SetInt(SELECTED_VEHICLE_KEY, selectedVehicleIndex);
        SaveOwnedVehicles();
        PlayerPrefs.Save();
    }
    
    void SaveOwnedVehicles()
    {
        List<string> ownedIndices = new List<string>();
        for (int i = 0; i < vehiclePrefabs.Count; i++)
        {
            if (IsVehicleOwned(i))
            {
                ownedIndices.Add(i.ToString());
            }
        }
        
        string ownedData = string.Join(",", ownedIndices);
        PlayerPrefs.SetString(OWNED_VEHICLES_KEY, ownedData);
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
        {
            Debug.LogError($"[Purchase] Invalid vehicle index: {index}");
            return false;
        }

        // Already owned
        if (IsVehicleOwned(index))
        {
            Debug.Log($"[Purchase] Vehicle at index {index} is already owned");
            return true;
        }

        VehicleInfo info = GetVehicleInfoAt(index);
        if (info == null)
        {
            Debug.LogError($"[Purchase] No VehicleInfo found at index {index}");
            return false;
        }

        // Check if player has enough coins
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("[Purchase] CurrencyManager.Instance is null");
            return false;
        }
        
        int currentCoins = CurrencyManager.Instance.GetCoins();
        Debug.Log($"[Purchase] Checking purchase: {info.vehicleName} costs {info.price}, player has {currentCoins} coins");
        
        if (currentCoins < info.price)
        {
            Debug.Log($"[Purchase] Insufficient funds: need {info.price}, have {currentCoins}");
            return false;
        }

        // Try to spend coins
        bool spendSuccess = CurrencyManager.Instance.SpendCoins(info.price);
        if (spendSuccess)
        {
            SetVehicleOwned(index, true);
            SaveData();
            Debug.Log($"[Purchase] Successfully purchased {info.vehicleName} for {info.price} coins!");
            return true;
        }
        else
        {
            Debug.LogError($"[Purchase] Failed to spend {info.price} coins for {info.vehicleName}");
            return false;
        }
    }

    public bool IsVehicleOwned(int index)
    {
        if (index < 0 || index >= vehiclePrefabs.Count)
            return false;
            
        VehicleInfo info = GetVehicleInfoAt(index);
        return info != null ? info.isOwned : false;
    }
    
    void SetVehicleOwned(int index, bool owned)
    {
        VehicleInfo info = GetVehicleInfoAt(index);
        if (info != null)
        {
            info.isOwned = owned;
        }
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
    
    public VehicleInfo GetVehicleInfoAt(int index)
    {
        GameObject prefab = GetVehiclePrefabAt(index);
        return prefab != null ? prefab.GetComponent<VehicleInfo>() : null;
    }
    
    public PlayerPhysics GetVehiclePhysicsAt(int index)
    {
        GameObject prefab = GetVehiclePrefabAt(index);
        return prefab != null ? prefab.GetComponentInChildren<PlayerPhysics>() : null;
    }
    
    public void ReloadData()
    {
        LoadData();
    }
    
    public void ResetData()
    {
        selectedVehicleIndex = 0;
        
        // Reset all vehicles to not owned except first one
        for (int i = 0; i < vehiclePrefabs.Count; i++)
        {
            SetVehicleOwned(i, i == 0);
        }
        
        SaveData();
    }
}

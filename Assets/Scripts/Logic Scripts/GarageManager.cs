using UnityEngine;
using System.Collections.Generic;

public class GarageManager : MonoBehaviour
{
    public static GarageManager Instance { get; private set; }

    [Header("Vehicle Prefabs")]
    [Tooltip("Kéo các prefab xe vào đây. Prefab phải có component VehicleInfo và PlayerPhysics")]
    public List<GameObject> vehiclePrefabs = new List<GameObject>();

    [Header("Upgrade")]
    [Tooltip("Stat multiplier when vehicle is upgraded (1.2 = +20%, 1.5 = +50%)")]
    public float upgradeMultiplier = 1.2f;

    [Header("Current Selection")]
    public int selectedVehicleIndex = 0;
    
    private const string SELECTED_VEHICLE_KEY = "SelectedVehicle";
    private const string OWNED_VEHICLES_KEY = "OwnedVehicles";
    private const string UPGRADED_VEHICLES_KEY = "UpgradedVehicles";
    private const string BROKEN_VEHICLES_KEY = "BrokenVehicles";      // indices of broken vehicles
    private const string REPAIR_TIME_KEY_PREFIX = "RepairFinish_";    // + index = Unix timestamp

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

    void Update()
    {
        // Tick repair timers every second
        _repairCheckTimer += Time.deltaTime;
        if (_repairCheckTimer >= 1f)
        {
            _repairCheckTimer = 0f;
            CheckRepairTimers();
        }
    }

    private float _repairCheckTimer = 0f;

    void LoadData()
    {
        selectedVehicleIndex = PlayerPrefs.GetInt(SELECTED_VEHICLE_KEY, 0);
        
        // Ensure first vehicle is always owned
        SetVehicleOwned(0, true);
        
        // Load owned vehicles
        LoadOwnedVehicles();
        
        // Load upgraded vehicles
        LoadUpgradedVehicles();
        
        // Load broken/repair state
        LoadRepairState();
        
        // Ensure selected vehicle is owned
        if (!IsVehicleOwned(selectedVehicleIndex))
        {
            selectedVehicleIndex = 0;
            SaveData();
        }
        
        // Tick repair timers on load
        CheckRepairTimers();
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
                    SetVehicleOwned(index, true);
            }
        }
    }

    void LoadUpgradedVehicles()
    {
        string upgradedData = PlayerPrefs.GetString(UPGRADED_VEHICLES_KEY, "");
        if (!string.IsNullOrEmpty(upgradedData))
        {
            string[] upgradedIndices = upgradedData.Split(',');
            foreach (string indexStr in upgradedIndices)
            {
                if (int.TryParse(indexStr, out int index))
                    SetVehicleUpgraded(index, true);
            }
        }
    }

    void SaveData()
    {
        PlayerPrefs.SetInt(SELECTED_VEHICLE_KEY, selectedVehicleIndex);
        SaveOwnedVehicles();
        SaveUpgradedVehicles();
        PlayerPrefs.Save();
    }

    void SaveOwnedVehicles()
    {
        List<string> ownedIndices = new List<string>();
        for (int i = 0; i < vehiclePrefabs.Count; i++)
        {
            if (IsVehicleOwned(i))
                ownedIndices.Add(i.ToString());
        }
        PlayerPrefs.SetString(OWNED_VEHICLES_KEY, string.Join(",", ownedIndices));
    }

    void SaveUpgradedVehicles()
    {
        List<string> upgradedIndices = new List<string>();
        for (int i = 0; i < vehiclePrefabs.Count; i++)
        {
            if (IsVehicleUpgraded(i))
                upgradedIndices.Add(i.ToString());
        }
        PlayerPrefs.SetString(UPGRADED_VEHICLES_KEY, string.Join(",", upgradedIndices));
    }

    void LoadRepairState()
    {
        // Load broken vehicles list
        string brokenData = PlayerPrefs.GetString(BROKEN_VEHICLES_KEY, "");
        if (!string.IsNullOrEmpty(brokenData))
        {
            foreach (string indexStr in brokenData.Split(','))
            {
                if (int.TryParse(indexStr, out int index))
                {
                    VehicleInfo info = GetVehicleInfoAt(index);
                    if (info != null) info.isReady = false;
                }
            }
        }
    }

    void SaveRepairState()
    {
        List<string> brokenIndices = new List<string>();
        for (int i = 0; i < vehiclePrefabs.Count; i++)
        {
            VehicleInfo info = GetVehicleInfoAt(i);
            if (info != null && !info.isReady)
                brokenIndices.Add(i.ToString());
        }
        PlayerPrefs.SetString(BROKEN_VEHICLES_KEY, string.Join(",", brokenIndices));
    }

    // Check if any repair timers have completed
    public void CheckRepairTimers()
    {
        double nowUnix = GetUnixTime();
        for (int i = 0; i < vehiclePrefabs.Count; i++)
        {
            VehicleInfo info = GetVehicleInfoAt(i);
            if (info == null || info.isReady) continue;

            string key = REPAIR_TIME_KEY_PREFIX + i;
            if (PlayerPrefs.HasKey(key))
            {
                double finishTime = double.Parse(PlayerPrefs.GetString(key));
                if (nowUnix >= finishTime)
                {
                    // Repair complete
                    info.isReady = true;
                    PlayerPrefs.DeleteKey(key);
                }
            }
        }
        SaveRepairState();
        PlayerPrefs.Save();
    }

    // Returns seconds remaining until repair is done (0 if done or not in repair)
    public float GetRepairSecondsRemaining(int index)
    {
        string key = REPAIR_TIME_KEY_PREFIX + index;
        if (!PlayerPrefs.HasKey(key)) return 0f;
        double finishTime = double.Parse(PlayerPrefs.GetString(key));
        float remaining = (float)(finishTime - GetUnixTime());
        return Mathf.Max(0f, remaining);
    }

    static double GetUnixTime() =>
        (System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;

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
        if (index < 0 || index >= vehiclePrefabs.Count) return false;
        VehicleInfo info = GetVehicleInfoAt(index);
        return info != null && info.isOwned;
    }

    void SetVehicleOwned(int index, bool owned)
    {
        VehicleInfo info = GetVehicleInfoAt(index);
        if (info != null) info.isOwned = owned;
    }

    public bool IsVehicleUpgraded(int index)
    {
        if (index < 0 || index >= vehiclePrefabs.Count) return false;
        VehicleInfo info = GetVehicleInfoAt(index);
        return info != null && info.isUpgraded;
    }

    void SetVehicleUpgraded(int index, bool upgraded)
    {
        VehicleInfo info = GetVehicleInfoAt(index);
        if (info != null) info.isUpgraded = upgraded;
    }

    public bool UpgradeVehicle(int index)
    {
        if (index < 0 || index >= vehiclePrefabs.Count) return false;
        if (!IsVehicleOwned(index)) return false;
        if (IsVehicleUpgraded(index)) return true;

        VehicleInfo info = GetVehicleInfoAt(index);
        if (info == null) return false;

        if (CurrencyManager.Instance == null) return false;
        if (!CurrencyManager.Instance.SpendDiamonds(info.upgradeCost)) return false;

        SetVehicleUpgraded(index, true);
        SaveData();
        return true;
    }

    // Mark vehicle as broken (called on game over)
    public void BreakVehicle(int index)
    {
        VehicleInfo info = GetVehicleInfoAt(index);
        if (info == null || !info.isOwned) return;
        info.isReady = false;
        SaveRepairState();
        PlayerPrefs.Save();
    }

    // Pay coins to start repair timer
    public bool StartRepair(int index)
    {
        VehicleInfo info = GetVehicleInfoAt(index);
        if (info == null || info.isReady) return false;
        if (CurrencyManager.Instance == null) return false;

        int cost = info.GetRepairCost();
        if (!CurrencyManager.Instance.SpendCoins(cost)) return false;

        // Set finish timestamp
        double finishTime = GetUnixTime() + info.repairDurationSeconds;
        PlayerPrefs.SetString(REPAIR_TIME_KEY_PREFIX + index, finishTime.ToString("F0"));
        SaveRepairState();
        PlayerPrefs.Save();
        return true;
    }

    // Pay diamonds to finish repair instantly
    public bool InstantRepair(int index)
    {
        VehicleInfo info = GetVehicleInfoAt(index);
        if (info == null || info.isReady) return false;
        if (CurrencyManager.Instance == null) return false;

        // Instant repair costs same as upgrade cost in diamonds (adjustable)
        int diamondCost = Mathf.Max(1, info.upgradeCost / 5);
        if (!CurrencyManager.Instance.SpendDiamonds(diamondCost)) return false;

        info.isReady = true;
        PlayerPrefs.DeleteKey(REPAIR_TIME_KEY_PREFIX + index);
        SaveRepairState();
        PlayerPrefs.Save();
        return true;
    }

    public bool IsVehicleReady(int index)
    {
        VehicleInfo info = GetVehicleInfoAt(index);
        return info != null && info.isReady;
    }

    // Returns diamond cost for instant repair
    public int GetInstantRepairDiamondCost(int index)
    {
        VehicleInfo info = GetVehicleInfoAt(index);
        if (info == null) return 0;
        return Mathf.Max(1, info.upgradeCost / 5);
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
        
        for (int i = 0; i < vehiclePrefabs.Count; i++)
        {
            SetVehicleOwned(i, i == 0);
            SetVehicleUpgraded(i, false);
            VehicleInfo info = GetVehicleInfoAt(i);
            if (info != null) info.isReady = true;
            PlayerPrefs.DeleteKey(REPAIR_TIME_KEY_PREFIX + i);
        }
        
        SaveData();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GarageUI : MonoBehaviour
{
    [Header("Vehicle Display")]
    public TextMeshProUGUI vehicleNameText;
    public TextMeshProUGUI maxSpeedText;
    public TextMeshProUGUI accelerationText;
    public TextMeshProUGUI steeringText;
    public TextMeshProUGUI handlingText;
    public TextMeshProUGUI priceText;
    
    [Header("Navigation")]
    public Button previousButton;
    public Button nextButton;
    public Button purchaseButton;
    
    [Header("Upgrade")]
    public Button upgradeButton;
    public TextMeshProUGUI upgradeButtonText;
    public TextMeshProUGUI diamondBalanceText;

    [Header("Repair")]
    public Button repairButton;          // Pay coins to start repair timer
    public Button instantRepairButton;   // Pay diamonds to repair instantly
    public TextMeshProUGUI repairButtonText;
    public TextMeshProUGUI instantRepairButtonText;
    public TextMeshProUGUI repairTimerText; // Shows countdown
    
    [Header("Format Strings")]
    public string maxSpeedFormat = "Max Speed: {0}";
    public string accelerationFormat = "Acceleration: {0}";
    public string steeringFormat = "Steering: {0}";
    public string handlingFormat = "Handling: {0}";
    public string priceFormat = "{0} coins";
    public string unlockedText = "UNLOCKED";
    public string selectedText = "SELECTED";
    
    private int currentIndex = 0;
    private Vector3 spawnPosition = new Vector3(6f, 0f, 0f);
    private float _timerRefresh = 0f;

    void Start()
    {
        // Setup buttons
        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousVehicle);
        
        if (nextButton != null)
            nextButton.onClick.AddListener(NextVehicle);
        
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(PurchaseCurrentVehicle);
        
        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeCurrentVehicle);
        
        if (repairButton != null)
            repairButton.onClick.AddListener(RepairCurrentVehicle);
        
        if (instantRepairButton != null)
            instantRepairButton.onClick.AddListener(InstantRepairCurrentVehicle);
        
        // Load current selection
        if (GarageManager.Instance != null)
        {
            currentIndex = GarageManager.Instance.selectedVehicleIndex;
        }
        
        // Update display immediately
        UpdateDisplay();
    }

    void Update()
    {
        _timerRefresh += Time.deltaTime;
        if (_timerRefresh >= 1f)
        {
            _timerRefresh = 0f;
            UpdateRepairTimer();
        }
    }

    void PreviousVehicle()
    {
        if (GarageManager.Instance == null) return;
        
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = GarageManager.Instance.GetVehicleCount() - 1;
        
        // Auto-select if owned
        if (GarageManager.Instance.IsVehicleOwned(currentIndex))
        {
            GarageManager.Instance.SelectVehicle(currentIndex);
        }
        
        UpdateDisplay();
        SwapVehicle();
        NotifyStartMenuUpdate();
    }

    void NextVehicle()
    {
        if (GarageManager.Instance == null) return;
        
        currentIndex++;
        if (currentIndex >= GarageManager.Instance.GetVehicleCount())
            currentIndex = 0;
        
        // Auto-select if owned
        if (GarageManager.Instance.IsVehicleOwned(currentIndex))
        {
            GarageManager.Instance.SelectVehicle(currentIndex);
        }
        
        UpdateDisplay();
        SwapVehicle();
        NotifyStartMenuUpdate();
    }

    void PurchaseCurrentVehicle()
    {
        if (GarageManager.Instance == null) return;
        
        // Check if already owned
        if (GarageManager.Instance.IsVehicleOwned(currentIndex))
        {
            // Already owned, just select it
            GarageManager.Instance.SelectVehicle(currentIndex);
            UpdateDisplay();
            return;
        }
        
        // Get vehicle info to check price
        VehicleInfo info = GarageManager.Instance.GetVehicleInfoAt(currentIndex);
        if (info == null) 
        {
            Debug.LogError("Vehicle info is null for index: " + currentIndex);
            return;
        }
        
        // Check if player has enough coins
        if (CurrencyManager.Instance == null)
        {
            Debug.LogError("CurrencyManager.Instance is null");
            return;
        }
        
        int currentCoins = CurrencyManager.Instance.GetCoins();
        Debug.Log($"[Purchase] Attempting to buy {info.vehicleName} for {info.price} coins. Player has {currentCoins} coins");
        
        if (currentCoins < info.price)
        {
            Debug.Log($"[Purchase] Not enough coins! Need {info.price}, have {currentCoins}");
            return;
        }
        
        // Attempt purchase
        bool success = GarageManager.Instance.PurchaseVehicle(currentIndex);
        
        if (success)
        {
            Debug.Log($"[Purchase] Successfully purchased {info.vehicleName}!");
            // Auto select after purchase
            GarageManager.Instance.SelectVehicle(currentIndex);
            NotifyStartMenuUpdate();
        }
        else
        {
            Debug.LogError($"[Purchase] Failed to purchase {info.vehicleName}");
        }
        
        UpdateDisplay();
    }

    void SelectCurrentVehicle()
    {
        // Method removed - auto-select when browsing unlocked vehicles
    }

    void UpgradeCurrentVehicle()
    {
        if (GarageManager.Instance == null) return;
        if (!GarageManager.Instance.IsVehicleOwned(currentIndex)) return;
        if (GarageManager.Instance.IsVehicleUpgraded(currentIndex)) return;

        bool success = GarageManager.Instance.UpgradeVehicle(currentIndex);
        if (success)
        {
            // Apply to scene instance if this is the currently selected vehicle
            if (currentIndex == GarageManager.Instance.selectedVehicleIndex)
            {
                GameObject sceneInstance = GameObject.FindGameObjectWithTag("Car");
                if (sceneInstance != null)
                    VehicleUpgrader.ApplyUpgrade(sceneInstance);
            }
            UpdateDisplay();
            NotifyStartMenuUpdate();
        }
    }

    void RepairCurrentVehicle()
    {
        if (GarageManager.Instance == null) return;
        // Only allow if broken and not already in repair queue
        if (GarageManager.Instance.IsVehicleReady(currentIndex)) return;
        if (GarageManager.Instance.GetRepairSecondsRemaining(currentIndex) > 0f) return;

        bool success = GarageManager.Instance.StartRepair(currentIndex);
        if (success) UpdateDisplay();
    }

    void InstantRepairCurrentVehicle()
    {
        if (GarageManager.Instance == null) return;
        if (GarageManager.Instance.IsVehicleReady(currentIndex)) return;

        bool success = GarageManager.Instance.InstantRepair(currentIndex);
        if (success)
        {
            UpdateDisplay();
            NotifyStartMenuUpdate();
        }
    }
    void SwapVehicle()
    {
        // Tìm và xóa xe cũ (tag "Car" - parent object)
        GameObject[] oldCars = GameObject.FindGameObjectsWithTag("Car");
        
        foreach (GameObject oldCar in oldCars)
        {
            Debug.Log($"Destroying car: {oldCar.name}");
            DestroyImmediate(oldCar);
        }
        
        // Spawn xe mới
        GameObject prefab = GarageManager.Instance.GetVehiclePrefabAt(currentIndex);
        if (prefab != null)
        {
            GameObject newVehicle = Instantiate(prefab, spawnPosition, Quaternion.identity);
            newVehicle.tag = "Car"; // Set parent tag to "Car"
            
            // Đảm bảo child CarPhysic có tag "Player" và game over trigger có tag "Car Hit"
            Transform carPhysic = newVehicle.transform.Find("CarPhysic");
            if (carPhysic != null)
            {
                carPhysic.tag = "Player";
                
                // Tìm game over trigger trong CarPhysic và set tag "Car Hit"
                Transform trigger = carPhysic.Find("GameOverTrigger");
                if (trigger != null)
                {
                    trigger.tag = "Car Hit";
                }
            }
            else
            {
                // Tìm child có PlayerPhysics component
                PlayerPhysics physics = newVehicle.GetComponentInChildren<PlayerPhysics>();
                if (physics != null)
                {
                    physics.gameObject.tag = "Player";
                    
                    // Tìm game over trigger và set tag
                    Transform trigger = physics.transform.Find("GameOverTrigger");
                    if (trigger != null)
                    {
                        trigger.tag = "Car Hit";
                    }
                }
            }
            
            // Auto-setup references cho PlayerPhysics
            PlayerPhysics playerPhysics = newVehicle.GetComponentInChildren<PlayerPhysics>();
            if (playerPhysics != null)
            {
                playerPhysics.roadSpawner = FindFirstObjectByType<RoadSpawner>();
                playerPhysics.roadMover = FindFirstObjectByType<RoadMover>();
            }
            
            // Notify các controllers về player mới
            NotifyPlayerChanged(newVehicle);
            
            // Apply upgrade bonus to scene instance
            VehicleUpgrader.ApplyUpgrade(newVehicle);
            
            Debug.Log($"Spawned new vehicle: {newVehicle.name} at {spawnPosition}");
        }
        else
        {
            Debug.LogError("Prefab is null!");
        }
    }
    
    void NotifyPlayerChanged(GameObject newPlayer)
    {
        // Notify CameraFunctions
        CameraFunctions camera = FindFirstObjectByType<CameraFunctions>();
        if (camera != null)
        {
            camera.RefreshPlayerReference();
        }
        
        // Notify NitroUI
        NitroUI nitroUI = FindFirstObjectByType<NitroUI>();
        if (nitroUI != null)
        {
            nitroUI.RefreshPlayerReference();
        }
        
        // Notify RoadMover
        RoadMover roadMover = FindFirstObjectByType<RoadMover>();
        if (roadMover != null)
        {
            roadMover.RefreshPlayerReference();
        }
        
        // Notify ScoreController
        ScoreController scoreController = FindFirstObjectByType<ScoreController>();
        if (scoreController != null)
        {
            scoreController.RefreshPlayerReference();
        }
        
        // Notify GameLogicController
        GameLogicController gameLogic = FindFirstObjectByType<GameLogicController>();
        if (gameLogic != null)
        {
            gameLogic.RefreshPlayerReference();
        }
        
        // Notify EnemyController
        EnemyController enemyController = FindFirstObjectByType<EnemyController>();
        if (enemyController != null)
        {
            enemyController.RefreshPlayerReference();
        }
        
        Debug.Log("Notified all controllers about player change");
    }

    void UpdateDisplay()
    {
        if (GarageManager.Instance == null) return;

        VehicleInfo info = GarageManager.Instance.GetVehicleInfoAt(currentIndex);
        if (info == null) return;

        // Stats: always read from prefab (base stats), apply multiplier if upgraded
        PlayerPhysics physics = GarageManager.Instance.GetVehiclePhysicsAt(currentIndex);
        float displayMultiplier = (info.isUpgraded && GarageManager.Instance != null)
            ? GarageManager.Instance.upgradeMultiplier : 1f;
        
        // Update vehicle info
        if (vehicleNameText != null)
            vehicleNameText.text = info.vehicleName;
        
        // Update stats từ PlayerPhysics
        if (physics != null)
        {
            if (maxSpeedText != null)
                maxSpeedText.text = string.Format(maxSpeedFormat, physics.maxSpeed * displayMultiplier);

            if (accelerationText != null)
                accelerationText.text = string.Format(accelerationFormat, physics.acceleration * displayMultiplier);

            if (steeringText != null)
                steeringText.text = string.Format(steeringFormat, physics.steerSpeed * displayMultiplier);

            if (handlingText != null)
                handlingText.text = string.Format(handlingFormat, physics.maxSteerVelocity * displayMultiplier);
        }
        
        // Update purchase button and status
        bool isOwned = GarageManager.Instance.IsVehicleOwned(currentIndex);
        bool isSelected = currentIndex == GarageManager.Instance.selectedVehicleIndex;
        bool isUpgraded = GarageManager.Instance.IsVehicleUpgraded(currentIndex);
        bool isReady = GarageManager.Instance.IsVehicleReady(currentIndex);
        float repairRemaining = GarageManager.Instance.GetRepairSecondsRemaining(currentIndex);
        bool isInRepairQueue = repairRemaining > 0f;
        
        bool hasEnoughCoins = info != null && CurrencyManager.Instance != null && CurrencyManager.Instance.GetCoins() >= info.price;
        bool hasEnoughDiamonds = info != null && CurrencyManager.Instance != null && CurrencyManager.Instance.GetDiamonds() >= info.upgradeCost;
        bool hasRepairCoins = info != null && CurrencyManager.Instance != null && CurrencyManager.Instance.GetCoins() >= info.GetRepairCost();
        int instantDiamondCost = GarageManager.Instance.GetInstantRepairDiamondCost(currentIndex);
        bool hasInstantDiamonds = CurrencyManager.Instance != null && CurrencyManager.Instance.GetDiamonds() >= instantDiamondCost;
        
        if (purchaseButton != null)
        {
            if (isOwned)
            {
                purchaseButton.gameObject.SetActive(false);
            }
            else
            {
                purchaseButton.gameObject.SetActive(true);
                purchaseButton.interactable = hasEnoughCoins;
                
                TextMeshProUGUI buttonText = purchaseButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                    buttonText.text = hasEnoughCoins ? "Purchase" : "Not Enough Coins";
            }
        }

        // Upgrade button: only show when owned, ready, and not yet upgraded
        if (upgradeButton != null)
        {
            upgradeButton.gameObject.SetActive(isOwned && isReady && !isUpgraded);
            if (isOwned && isReady && !isUpgraded)
            {
                upgradeButton.interactable = hasEnoughDiamonds;
                if (upgradeButtonText != null)
                    upgradeButtonText.text = hasEnoughDiamonds
                        ? $"Upgrade ({info.upgradeCost} 💎)"
                        : $"Need {info.upgradeCost} 💎";
            }
        }

        // Repair buttons: only show when owned and broken
        if (repairButton != null)
        {
            // Show "Pay to Repair" only when broken and NOT already in repair queue
            repairButton.gameObject.SetActive(isOwned && !isReady && !isInRepairQueue);
            if (isOwned && !isReady && !isInRepairQueue)
            {
                repairButton.interactable = hasRepairCoins;
                if (repairButtonText != null)
                    repairButtonText.text = hasRepairCoins
                        ? $"Repair ({info.GetRepairCost()} coins)"
                        : $"Need {info.GetRepairCost()} coins";
            }
        }

        if (instantRepairButton != null)
        {
            // Show instant repair when broken (whether or not in queue)
            instantRepairButton.gameObject.SetActive(isOwned && !isReady);
            if (isOwned && !isReady)
            {
                instantRepairButton.interactable = hasInstantDiamonds;
                if (instantRepairButtonText != null)
                    instantRepairButtonText.text = hasInstantDiamonds
                        ? $"Fix Now ({instantDiamondCost} 💎)"
                        : $"Need {instantDiamondCost} 💎";
            }
        }

        // Repair timer text
        if (repairTimerText != null)
        {
            if (isOwned && !isReady && isInRepairQueue)
            {
                int mins = Mathf.FloorToInt(repairRemaining / 60f);
                int secs = Mathf.FloorToInt(repairRemaining % 60f);
                repairTimerText.text = $"Ready in {mins:D2}:{secs:D2}";
                repairTimerText.gameObject.SetActive(true);
            }
            else
            {
                repairTimerText.gameObject.SetActive(false);
            }
        }

        // Diamond balance display
        if (diamondBalanceText != null && CurrencyManager.Instance != null)
            diamondBalanceText.text = $"💎 {CurrencyManager.Instance.GetDiamonds()}";
        
        // Show status text
        if (priceText != null && info != null)
        {
            if (!isReady && isOwned)
            {
                priceText.text = isInRepairQueue ? "REPAIRING..." : "BROKEN";
                priceText.color = Color.red;
            }
            else if (isUpgraded && isSelected)
            {
                priceText.text = "UPGRADED ★";
                priceText.color = Color.yellow;
            }
            else if (isUpgraded)
            {
                priceText.text = "UPGRADED";
                priceText.color = Color.yellow;
            }
            else if (isSelected)
            {
                priceText.text = selectedText;
                priceText.color = Color.green;
            }
            else if (isOwned)
            {
                priceText.text = unlockedText;
                priceText.color = Color.cyan;
            }
            else
            {
                priceText.text = string.Format(priceFormat, info.price);
                priceText.color = hasEnoughCoins ? Color.white : Color.red;
            }
        }
    }
    
    // Finds first child (recursive) with the given tag
    GameObject FindChildWithTag(GameObject parent, string tag)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(tag)) return child.gameObject;
        }
        return null;
    }

    // Public method để refresh display từ bên ngoài (vd: khi mua thêm coin)
    public void RefreshDisplay()
    {
        UpdateDisplay();
    }
    
    void UpdateRepairTimer()
    {
        if (GarageManager.Instance == null) return;
        bool isReady = GarageManager.Instance.IsVehicleReady(currentIndex);
        if (isReady) return;

        float remaining = GarageManager.Instance.GetRepairSecondsRemaining(currentIndex);
        if (remaining > 0f && repairTimerText != null)
        {
            int mins = Mathf.FloorToInt(remaining / 60f);
            int secs = Mathf.FloorToInt(remaining % 60f);
            repairTimerText.text = $"Ready in {mins:D2}:{secs:D2}";
        }
        else if (remaining <= 0f)
        {
            // Timer done - refresh full display
            UpdateDisplay();
        }
    }
    
    void NotifyStartMenuUpdate()
    {
        StartMenuUI startMenu = FindFirstObjectByType<StartMenuUI>();
        if (startMenu != null)
        {
            startMenu.RefreshDisplay();
        }
    }
}

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
    public Button selectButton;
    
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

    void Start()
    {
        // Setup buttons
        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousVehicle);
        
        if (nextButton != null)
            nextButton.onClick.AddListener(NextVehicle);
        
        if (purchaseButton != null)
            purchaseButton.onClick.AddListener(PurchaseCurrentVehicle);
        
        if (selectButton != null)
            selectButton.onClick.AddListener(SelectCurrentVehicle);
        
        // Load current selection
        if (GarageManager.Instance != null)
        {
            currentIndex = GarageManager.Instance.selectedVehicleIndex;
        }
        
        // Update display immediately
        UpdateDisplay();
    }

    void PreviousVehicle()
    {
        if (GarageManager.Instance == null) return;
        
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = GarageManager.Instance.GetVehicleCount() - 1;
        
        UpdateDisplay();
        SwapVehicle();
    }

    void NextVehicle()
    {
        if (GarageManager.Instance == null) return;
        
        currentIndex++;
        if (currentIndex >= GarageManager.Instance.GetVehicleCount())
            currentIndex = 0;
        
        UpdateDisplay();
        SwapVehicle();
    }

    void PurchaseCurrentVehicle()
    {
        if (GarageManager.Instance == null) return;
        
        bool success = GarageManager.Instance.PurchaseVehicle(currentIndex);
        
        if (success)
        {
            // Auto select after purchase
            GarageManager.Instance.SelectVehicle(currentIndex);
        }
        
        UpdateDisplay();
    }

    void SelectCurrentVehicle()
    {
        if (GarageManager.Instance == null) return;
        
        if (GarageManager.Instance.IsVehicleUnlocked(currentIndex))
        {
            GarageManager.Instance.SelectVehicle(currentIndex);
            UpdateDisplay();
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
        
        // Lấy thông tin từ prefab
        VehicleInfo info = GarageManager.Instance.GetVehicleInfoAt(currentIndex);
        PlayerPhysics physics = GarageManager.Instance.GetVehiclePhysicsAt(currentIndex);
        
        if (info == null)
        {
            Debug.LogError($"Vehicle at index {currentIndex} missing VehicleInfo component!");
            return;
        }
        
        // Update vehicle info
        if (vehicleNameText != null)
            vehicleNameText.text = info.vehicleName;
        
        // Update stats từ PlayerPhysics
        if (physics != null)
        {
            if (maxSpeedText != null)
                maxSpeedText.text = string.Format(maxSpeedFormat, physics.maxSpeed);
            
            if (accelerationText != null)
                accelerationText.text = string.Format(accelerationFormat, physics.acceleration);
            
            if (steeringText != null)
                steeringText.text = string.Format(steeringFormat, physics.steerSpeed);
            
            if (handlingText != null)
                handlingText.text = string.Format(handlingFormat, physics.maxSteerVelocity);
        }
        
        // Update purchase/select buttons
        bool isUnlocked = GarageManager.Instance.IsVehicleUnlocked(currentIndex);
        bool isSelected = currentIndex == GarageManager.Instance.selectedVehicleIndex;
        
        if (purchaseButton != null)
        {
            purchaseButton.gameObject.SetActive(!isUnlocked);
        }
        
        if (selectButton != null)
        {
            selectButton.gameObject.SetActive(isUnlocked && !isSelected);
        }
        
        // Show status text
        if (priceText != null)
        {
            if (isSelected)
            {
                priceText.text = selectedText;
            }
            else if (isUnlocked)
            {
                priceText.text = unlockedText;
            }
            else
            {
                priceText.text = string.Format(priceFormat, info.price);
            }
        }
    }
    
    // Public method để refresh display từ bên ngoài (vd: khi mua thêm coin)
    public void RefreshDisplay()
    {
        UpdateDisplay();
    }
}

using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Controls start menu UI and game start
public class StartMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public Canvas menuCanvas; // Main canvas containing menu UI
    public GameObject menuPanel; // Panel containing menu UI (optional, for backward compatibility)
    public Button startButton;
    public Button quitButton;
    public Button resetDataButton; // Reset data button

    [Header("Display")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI highScoreText;
    public string coinsFormat = "Coins: {0}";
    public string highScoreFormat = "High Score: {0:N0}";

    [Header("References")]
    public GameLogicController gameLogic;
    public EnemyController enemyController;

    void Start()
    {
        // Auto-find menu canvas if not assigned
        if (menuCanvas == null)
            menuCanvas = GetComponentInParent<Canvas>();

        // Auto-find references
        if (gameLogic == null)
            gameLogic = FindFirstObjectByType<GameLogicController>();

        if (enemyController == null)
            enemyController = FindFirstObjectByType<EnemyController>();

        // Setup buttons
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitButtonClicked);

        if (resetDataButton != null)
            resetDataButton.onClick.AddListener(OnResetDataButtonClicked);

        // Show menu and disable spawner
        ShowMenu();
        UpdateDisplay();
        
        // Update display periodically to catch vehicle changes
        InvokeRepeating(nameof(UpdateDisplay), 0.5f, 0.5f);
    }

    void ShowMenu()
    {
        // Show canvas
        if (menuCanvas != null)
            menuCanvas.enabled = true;

        // Show panel (if using panel-based approach)
        if (menuPanel != null)
            menuPanel.SetActive(true);

        // Disable enemy spawner
        if (enemyController != null)
            enemyController.enabled = false;
    }

    void UpdateDisplay()
    {
        // Update coins display
        if (coinsText != null && CurrencyManager.Instance != null)
        {
            coinsText.text = string.Format(coinsFormat, CurrencyManager.Instance.GetCoins());
        }

        // Update high score display
        if (highScoreText != null && CurrencyManager.Instance != null)
        {
            highScoreText.text = string.Format(highScoreFormat, CurrencyManager.Instance.GetHighScore());
        }
        
        // Update start button state
        UpdateStartButton();
    }
    
    // Public method to refresh display when vehicle selection changes
    public void RefreshDisplay()
    {
        UpdateDisplay();
    }
    
    void UpdateStartButton()
    {
        if (startButton == null) return;
        
        GameObject currentVehicle = GameObject.FindGameObjectWithTag("Car");
        bool canStart = false;
        
        if (currentVehicle != null)
        {
            VehicleInfo vehicleInfo = currentVehicle.GetComponent<VehicleInfo>();
            if (vehicleInfo != null)
                canStart = vehicleInfo.isOwned && vehicleInfo.isReady;
        }
        
        startButton.interactable = canStart;
        
        TextMeshProUGUI buttonText = startButton.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null)
        {
            if (canStart)
            {
                buttonText.text = "Start Game";
                buttonText.color = Color.white;
            }
            else
            {
                // Find out why
                if (currentVehicle != null)
                {
                    VehicleInfo vi = currentVehicle.GetComponent<VehicleInfo>();
                    if (vi != null && vi.isOwned && !vi.isReady)
                    {
                        buttonText.text = "Vehicle Needs Repair";
                        buttonText.color = Color.red;
                        return;
                    }
                }
                buttonText.text = "Vehicle Not Owned";
                buttonText.color = Color.red;
            }
        }
    }

    void OnStartButtonClicked()
    {
        GameObject currentVehicle = GameObject.FindGameObjectWithTag("Car");
        
        if (currentVehicle != null)
        {
            VehicleInfo vehicleInfo = currentVehicle.GetComponent<VehicleInfo>();
            if (vehicleInfo == null) return;
            if (!vehicleInfo.isOwned || !vehicleInfo.isReady) return;
        }
        else return;
        
        if (menuCanvas != null)
            menuCanvas.enabled = false;

        if (menuPanel != null)
            menuPanel.SetActive(false);

        if (gameLogic != null)
            gameLogic.StartGame();
    }

    void OnQuitButtonClicked()
    {
        // Quit application
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    void OnResetDataButtonClicked()
    {
        // Reset all player data
        ResetAllPlayerData();
        
        // Update display to show reset values
        UpdateDisplay();
    }

    void ResetAllPlayerData()
    {
        // Reset CurrencyManager data
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ResetData();
        }
        
        // Reset GarageManager data
        if (GarageManager.Instance != null)
        {
            GarageManager.Instance.ResetData();
        }
        
        // Clear all PlayerPrefs to ensure everything is reset
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        
        // Reload data in managers
        if (CurrencyManager.Instance != null)
        {
            CurrencyManager.Instance.ReloadData();
        }
        
        if (GarageManager.Instance != null)
        {
            GarageManager.Instance.ReloadData();
        }
    }
}

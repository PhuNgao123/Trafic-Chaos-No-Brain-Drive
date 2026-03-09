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

        // Show menu and disable spawner
        ShowMenu();
        UpdateDisplay();
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
    }

    void OnStartButtonClicked()
    {
        // Hide entire menu canvas
        if (menuCanvas != null)
            menuCanvas.enabled = false;

        // Also hide panel if using panel-based approach
        if (menuPanel != null)
            menuPanel.SetActive(false);

        // Start game
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
}

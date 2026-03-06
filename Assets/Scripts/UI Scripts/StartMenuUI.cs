using UnityEngine;
using UnityEngine.UI;

// Controls start menu UI and game start
public class StartMenuUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject menuPanel; // Panel containing menu UI
    public Button startButton;

    [Header("References")]
    public GameLogicController gameLogic;
    public EnemyController enemyController;

    void Start()
    {
        // Auto-find references
        if (gameLogic == null)
            gameLogic = FindFirstObjectByType<GameLogicController>();

        if (enemyController == null)
            enemyController = FindFirstObjectByType<EnemyController>();

        // Setup button
        if (startButton != null)
            startButton.onClick.AddListener(OnStartButtonClicked);

        // Show menu and disable spawner
        ShowMenu();
    }

    void ShowMenu()
    {
        if (menuPanel != null)
            menuPanel.SetActive(true);

        // Disable enemy spawner
        if (enemyController != null)
            enemyController.enabled = false;
    }

    void OnStartButtonClicked()
    {
        // Hide menu
        if (menuPanel != null)
            menuPanel.SetActive(false);

        // Start game
        if (gameLogic != null)
            gameLogic.StartGame();
    }
}

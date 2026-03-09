using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// Controls death screen UI shown after game over
// Shows score, coins earned, and restart/menu options
public class DeathScreenUI : MonoBehaviour
{
    [Header("UI References")]
    public Canvas deathCanvas; // Canvas containing death screen
    public GameObject deathPanel; // Panel containing death UI

    [Header("Display")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI coinsEarnedText;
    public TextMeshProUGUI totalCoinsText;
    public TextMeshProUGUI highScoreText;
    public string finalScoreFormat = "Score: {0:N0}";
    public string coinsEarnedFormat = "+{0} Coins";
    public string totalCoinsFormat = "Total: {0} Coins";
    public string highScoreFormat = "High Score: {0:N0}";

    [Header("Buttons")]
    public Button restartButton;
    public Button quitButton;

    [Header("Settings")]
    public float showDelay = 2f; // Delay before showing death screen

    [Header("References")]
    public GameLogicController gameLogic;
    public ScoreController scoreController;

    private bool _isShowing = false;
    private float _gameOverTime = 0f;

    void Start()
    {
        // Auto-find canvas if not assigned
        if (deathCanvas == null)
            deathCanvas = GetComponentInParent<Canvas>();

        // Auto-find references
        if (gameLogic == null)
            gameLogic = FindFirstObjectByType<GameLogicController>();

        if (scoreController == null)
            scoreController = FindFirstObjectByType<ScoreController>();

        // Setup buttons
        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        // Hide death screen initially
        HideDeathScreen();
    }

    void Update()
    {
        // Check if game over and show death screen after delay
        if (!_isShowing && gameLogic != null && gameLogic.isGameOver)
        {
            if (_gameOverTime == 0f)
            {
                _gameOverTime = Time.time;
            }

            if (Time.time - _gameOverTime >= showDelay)
            {
                ShowDeathScreen();
            }
        }
    }

    void HideDeathScreen()
    {
        if (deathCanvas != null)
            deathCanvas.enabled = false;

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    void ShowDeathScreen()
    {
        _isShowing = true;

        // Show canvas and panel
        if (deathCanvas != null)
            deathCanvas.enabled = true;

        if (deathPanel != null)
            deathPanel.SetActive(true);

        // Update display with final stats
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (scoreController == null || CurrencyManager.Instance == null)
            return;

        float finalScore = scoreController.GetCurrentScore();
        int coinsEarned = Mathf.FloorToInt(finalScore / 100f); // 100 score = 1 coin
        int totalCoins = CurrencyManager.Instance.GetCoins();
        float highScore = CurrencyManager.Instance.GetHighScore();

        // Update final score
        if (finalScoreText != null)
        {
            finalScoreText.text = string.Format(finalScoreFormat, finalScore);
        }

        // Update coins earned
        if (coinsEarnedText != null)
        {
            coinsEarnedText.text = string.Format(coinsEarnedFormat, coinsEarned);
        }

        // Update total coins
        if (totalCoinsText != null)
        {
            totalCoinsText.text = string.Format(totalCoinsFormat, totalCoins);
        }

        // Update high score
        if (highScoreText != null)
        {
            highScoreText.text = string.Format(highScoreFormat, highScore);
        }
    }

    void OnRestartClicked()
    {
        // Restart game by reloading scene
        Debug.Log("[DeathScreen] Restart clicked - Reloading scene...");
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnQuitClicked()
    {
        // Quit application
        Debug.Log("[DeathScreen] Quit clicked - Closing game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

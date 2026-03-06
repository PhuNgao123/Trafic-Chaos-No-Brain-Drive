using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Displays score and combo UI with animations
public class ScoreUI : MonoBehaviour
{
    [Header("References")]
    public ScoreController scoreController;

    [Header("Score UI")]
    public GameObject scorePanel; // Main panel containing all score UI
    public TextMeshProUGUI scoreText;
    public string scoreFormat = "Score: {0:N0}";

    [Header("Combo UI")]
    public GameObject comboPanel;
    public TextMeshProUGUI comboText;
    public Image comboTimerBar;
    public string comboFormat = "x{0}";

    [Header("Animation")]
    public float comboScalePunch = 1.2f;
    public float comboScaleDuration = 0.2f;

    private Vector3 _comboOriginalScale;
    private float _comboScaleTimer = 0f;
    private bool _isComboScaling = false;

    void Start()
    {
        // Auto-find references
        if (scoreController == null)
            scoreController = FindFirstObjectByType<ScoreController>();

        // Subscribe to events
        if (scoreController != null)
        {
            scoreController.OnScoreChanged += UpdateScore;
            scoreController.OnComboChanged += UpdateCombo;
            scoreController.OnComboTimerChanged += UpdateComboTimer;
        }

        // Store original scale
        if (comboPanel != null)
        {
            _comboOriginalScale = comboPanel.transform.localScale;
            comboPanel.SetActive(false); // Hide at start
        }

        // Hide score panel until game starts
        if (scorePanel != null)
            scorePanel.SetActive(false);

        // Initialize UI
        UpdateScore(0f);
        
        // Force hide combo UI at start
        if (comboText != null)
            comboText.text = "";
        if (comboTimerBar != null)
            comboTimerBar.fillAmount = 0f;
    }

    void Update()
    {
        // Show score panel when game starts
        if (scorePanel != null && !scorePanel.activeSelf)
        {
            if (GameLogicController.Instance != null && GameLogicController.Instance.isGameStarted)
            {
                scorePanel.SetActive(true);
            }
        }

        // Handle combo scale animation
        if (_isComboScaling)
        {
            _comboScaleTimer += Time.deltaTime;
            float progress = _comboScaleTimer / comboScaleDuration;

            if (progress >= 1f)
            {
                // Animation complete
                comboPanel.transform.localScale = _comboOriginalScale;
                _isComboScaling = false;
            }
            else
            {
                // Scale up then down
                float scale = progress < 0.5f
                    ? Mathf.Lerp(1f, comboScalePunch, progress * 2f)
                    : Mathf.Lerp(comboScalePunch, 1f, (progress - 0.5f) * 2f);

                comboPanel.transform.localScale = _comboOriginalScale * scale;
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (scoreController != null)
        {
            scoreController.OnScoreChanged -= UpdateScore;
            scoreController.OnComboChanged -= UpdateCombo;
            scoreController.OnComboTimerChanged -= UpdateComboTimer;
        }
    }

    // Update score display
    void UpdateScore(float score)
    {
        if (scoreText != null)
        {
            scoreText.text = string.Format(scoreFormat, score);
        }
    }

    // Update combo display with animation
    void UpdateCombo(int combo)
    {
        Debug.Log($"[ScoreUI] UpdateCombo called: {combo}");

        if (combo <= 0)
        {
            // Hide combo panel
            if (comboPanel != null)
            {
                comboPanel.SetActive(false);
                Debug.Log("[ScoreUI] Combo panel hidden");
            }
            
            // Clear text
            if (comboText != null)
                comboText.text = "";
                
            return;
        }

        // Show combo panel
        if (comboPanel != null)
        {
            comboPanel.SetActive(true);
            Debug.Log("[ScoreUI] Combo panel shown");
        }

        // Update combo text - FORCE visible
        if (comboText != null)
        {
            string newText = string.Format(comboFormat, combo);
            comboText.text = newText;
            comboText.enabled = true; // Force enable
            comboText.ForceMeshUpdate(); // Force update mesh
            
            Debug.Log($"[ScoreUI] Combo text updated: {newText} | Enabled: {comboText.enabled} | GameObject active: {comboText.gameObject.activeSelf}");
        }
        else
        {
            Debug.LogError("[ScoreUI] ComboText is NULL!");
        }

        // Trigger scale animation
        TriggerComboAnimation();
    }

    // Update combo timer bar
    void UpdateComboTimer(float current, float max)
    {
        if (comboTimerBar != null)
        {
            float fillAmount = max > 0 ? current / max : 0f;
            comboTimerBar.fillAmount = fillAmount;

            Debug.Log($"[ScoreUI] Timer: {current:F2}/{max:F2} = {fillAmount:F2} | FillAmount set to: {comboTimerBar.fillAmount}");

            // Change bar color based on urgency
            if (fillAmount < 0.3f)
                comboTimerBar.color = Color.red;
            else if (fillAmount < 0.6f)
                comboTimerBar.color = Color.yellow;
            else
                comboTimerBar.color = Color.green;
        }
        else
        {
            Debug.LogError("[ScoreUI] ComboTimerBar is NULL!");
        }
    }

    // Trigger combo scale animation
    void TriggerComboAnimation()
    {
        _isComboScaling = true;
        _comboScaleTimer = 0f;
    }
}
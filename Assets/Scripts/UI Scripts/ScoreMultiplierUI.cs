using UnityEngine;
using TMPro;

/// <summary>
/// Displays score multiplier UI (e.g., "x10" indicator).
/// Shows multiplier value and remaining time.
/// </summary>
public class ScoreMultiplierUI : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    [Tooltip("Player's ScoreMultiplierController")]
    public ScoreMultiplierController multiplierController;

    [Header("=== UI ELEMENTS ===")]
    [Tooltip("Panel containing multiplier UI (shown/hidden)")]
    public GameObject multiplierPanel;

    [Tooltip("Text displaying multiplier value (e.g., 'x10')")]
    public TextMeshProUGUI multiplierText;

    [Tooltip("Text displaying remaining time (optional)")]
    public TextMeshProUGUI timerText;

    [Header("=== FORMATS ===")]
    public string multiplierFormat = "x{0}";
    public string timerFormat = "{0:F1}s";

    void Start()
    {
        // Auto-find multiplier controller
        if (multiplierController == null)
        {
            PlayerPhysics playerPhysics = FindFirstObjectByType<PlayerPhysics>();
            if (playerPhysics != null)
            {
                multiplierController = playerPhysics.GetComponent<ScoreMultiplierController>();
            }
        }

        // Subscribe to events
        if (multiplierController != null)
        {
            multiplierController.OnMultiplierChanged += UpdateMultiplierUI;
        }

        // Hide panel initially
        if (multiplierPanel != null)
        {
            multiplierPanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (multiplierController != null)
        {
            multiplierController.OnMultiplierChanged -= UpdateMultiplierUI;
        }
    }

    /// <summary>
    /// Update multiplier UI display
    /// </summary>
    void UpdateMultiplierUI(float multiplier, bool isActive, float remainingTime)
    {
        // Show/hide panel
        if (multiplierPanel != null)
        {
            multiplierPanel.SetActive(isActive);
        }

        if (isActive)
        {
            // Update multiplier text
            if (multiplierText != null)
            {
                multiplierText.text = string.Format(multiplierFormat, multiplier);
            }

            // Update timer text
            if (timerText != null)
            {
                timerText.text = string.Format(timerFormat, remainingTime);
            }
        }
    }
}

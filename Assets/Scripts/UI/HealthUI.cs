using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays the player vehicle's health on the UI.
/// Updates the health bar and text display when health changes.
/// </summary>
public class HealthUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the PlayerHealth component")]
    public PlayerHealth playerHealth;

    [Tooltip("UI Slider for health bar visualization")]
    public Slider healthSlider;

    [Tooltip("TextMeshProUGUI for numeric health display (optional)")]
    public TextMeshProUGUI healthText;

    void Start()
    {
        // Validate references
        if (playerHealth == null)
        {
            Debug.LogWarning("HealthUI: PlayerHealth reference is not assigned!");
            return;
        }

        if (healthSlider == null)
        {
            Debug.LogWarning("HealthUI: Health Slider reference is not assigned!");
        }

        if (healthText == null)
        {
            Debug.LogWarning("HealthUI: Health Text reference is not assigned (optional).");
        }

        // Subscribe to health change events
        playerHealth.OnHealthChanged += UpdateHealthDisplay;

        // Initialize display with current health
        UpdateHealthDisplay(playerHealth.currentHealth, playerHealth.maxHealth);
    }

    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthDisplay;
        }
    }

    /// <summary>
    /// Updates the health display when health changes.
    /// </summary>
    /// <param name="current">Current health value</param>
    /// <param name="max">Maximum health value</param>
    void UpdateHealthDisplay(float current, float max)
    {
        // Update slider value
        if (healthSlider != null)
        {
            if (max > 0)
            {
                healthSlider.value = current / max;
            }
            else
            {
                healthSlider.value = 0f;
            }
        }

        // Update text display (if assigned)
        if (healthText != null)
        {
            healthText.text = $"{Mathf.Ceil(current)}/{Mathf.Ceil(max)}";
        }
    }
}

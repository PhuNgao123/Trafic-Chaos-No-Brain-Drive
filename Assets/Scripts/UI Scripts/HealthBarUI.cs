using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the health bar UI display using a Slider.
/// Subscribes to PlayerPhysics health events and updates the visual representation.
/// </summary>
public class HealthBarUI : MonoBehaviour
{
    [Header("=== REFERENCES ===")]
    [Tooltip("Reference to the PlayerPhysics component")]
    public PlayerPhysics playerPhysics;

    [Tooltip("The Slider component that represents the health bar")]
    public Slider healthSlider;

    [Tooltip("The Image component for the fill area (optional, for color change)")]
    public Image fillImage;

    [Header("=== VISUAL SETTINGS ===")]
    [Tooltip("Color when health is full")]
    public Color fullHealthColor = Color.green;

    [Tooltip("Color when health is low")]
    public Color lowHealthColor = Color.red;

    [Tooltip("Health percentage threshold for low health color (0-1)")]
    [Range(0f, 1f)]
    public float lowHealthThreshold = 0.3f;

    [Header("=== ANIMATION ===")]
    [Tooltip("Smooth transition speed for health bar changes")]
    public float smoothSpeed = 5f;

    [Header("=== VISIBILITY ===")]
    [Tooltip("Hide health bar until game starts")]
    public bool hideUntilGameStart = true;

    [Tooltip("Panel to hide/show (usually parent of slider)")]
    public GameObject healthPanel;

    private float targetValue = 1f;

    void Start()
    {
        // Auto-find PlayerPhysics if not assigned
        if (playerPhysics == null)
        {
            playerPhysics = FindFirstObjectByType<PlayerPhysics>();
        }

        if (playerPhysics == null)
        {
            Debug.LogError("[HealthBarUI] PlayerPhysics not found!");
            return;
        }

        if (healthSlider == null)
        {
            Debug.LogError("[HealthBarUI] Health Slider not assigned!");
            return;
        }

        // Setup slider
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 1f;
        healthSlider.interactable = false;

        // Auto-find fill image if not assigned
        if (fillImage == null && healthSlider.fillRect != null)
        {
            fillImage = healthSlider.fillRect.GetComponent<Image>();
        }

        // Subscribe to health change events
        playerPhysics.OnHealthChanged += UpdateHealthBar;

        // Initialize health bar
        UpdateHealthBar(playerPhysics.currentHealth, playerPhysics.maxHealth);

        // Hide panel until game starts
        if (hideUntilGameStart && healthPanel != null)
        {
            healthPanel.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (playerPhysics != null)
        {
            playerPhysics.OnHealthChanged -= UpdateHealthBar;
        }
    }

    void Update()
    {
        // Show panel when game starts
        if (hideUntilGameStart && healthPanel != null && !healthPanel.activeSelf)
        {
            if (GameLogicController.Instance != null && GameLogicController.Instance.isGameStarted)
            {
                healthPanel.SetActive(true);
            }
        }

        // Smooth transition for health bar
        if (healthSlider != null && Mathf.Abs(healthSlider.value - targetValue) > 0.001f)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetValue, smoothSpeed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Updates the health bar based on current and max health values
    /// </summary>
    private void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (healthSlider == null) return;

        // Calculate health percentage
        float healthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        targetValue = healthPercentage;

        // Update color based on health percentage
        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, 
                healthPercentage / lowHealthThreshold);
        }
    }

    /// <summary>
    /// Immediately sets the health bar without smooth transition (useful for initialization)
    /// </summary>
    public void SetHealthImmediate(float currentHealth, float maxHealth)
    {
        float healthPercentage = maxHealth > 0 ? currentHealth / maxHealth : 0f;
        targetValue = healthPercentage;
        
        if (healthSlider != null)
        {
            healthSlider.value = healthPercentage;
        }

        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, 
                healthPercentage / lowHealthThreshold);
        }
    }
}

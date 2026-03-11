using UnityEngine;

/// <summary>
/// Manages score multiplier state (e.g., x10 score boost).
/// Attached to player to track active multiplier duration.
/// </summary>
public class ScoreMultiplierController : MonoBehaviour
{
    [Header("=== MULTIPLIER STATE ===")]
    [Tooltip("Current score multiplier (1 = normal, 10 = x10 boost)")]
    public float currentMultiplier = 1f;

    [Tooltip("Is multiplier currently active?")]
    public bool isMultiplierActive = false;

    private float _multiplierTimer = 0f;

    /// <summary>
    /// Event fired when multiplier state changes. Parameters: (multiplier, isActive, remainingTime)
    /// </summary>
    public event System.Action<float, bool, float> OnMultiplierChanged;

    void Update()
    {
        if (isMultiplierActive)
        {
            _multiplierTimer -= Time.deltaTime;

            // Notify UI of timer update
            OnMultiplierChanged?.Invoke(currentMultiplier, true, _multiplierTimer);

            if (_multiplierTimer <= 0f)
            {
                DeactivateMultiplier();
            }
        }
    }

    /// <summary>
    /// Activate score multiplier for a duration
    /// </summary>
    /// <param name="multiplier">Multiplier value (e.g., 10 for x10)</param>
    /// <param name="duration">Duration in seconds</param>
    public void ActivateMultiplier(float multiplier, float duration)
    {
        currentMultiplier = multiplier;
        _multiplierTimer = duration;
        isMultiplierActive = true;

        Debug.Log($"[ScoreMultiplierController] ✓ Multiplier activated! x{multiplier} for {duration}s");

        // Notify listeners
        OnMultiplierChanged?.Invoke(currentMultiplier, true, _multiplierTimer);
    }

    /// <summary>
    /// Deactivate score multiplier
    /// </summary>
    void DeactivateMultiplier()
    {
        currentMultiplier = 1f;
        _multiplierTimer = 0f;
        isMultiplierActive = false;

        Debug.Log("[ScoreMultiplierController] Multiplier deactivated");

        // Notify listeners
        OnMultiplierChanged?.Invoke(currentMultiplier, false, 0f);
    }

    /// <summary>
    /// Get current multiplier value
    /// </summary>
    public float GetMultiplier()
    {
        return isMultiplierActive ? currentMultiplier : 1f;
    }

    /// <summary>
    /// Get remaining time
    /// </summary>
    public float GetRemainingTime()
    {
        return _multiplierTimer;
    }
}

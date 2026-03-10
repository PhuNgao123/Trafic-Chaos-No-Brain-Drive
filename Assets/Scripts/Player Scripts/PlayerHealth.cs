using UnityEngine;

/// <summary>
/// Manages the player vehicle's health state.
/// Tracks current and maximum health, enforces health invariants,
/// and notifies listeners when health changes.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Maximum health value for the player vehicle")]
    public float maxHealth = 100f;

    /// <summary>
    /// Current health value. Always maintained within bounds: 0 <= currentHealth <= maxHealth
    /// </summary>
    public float currentHealth { get; private set; }

    /// <summary>
    /// Event fired when health changes. Parameters: (currentHealth, maxHealth)
    /// </summary>
    public event System.Action<float, float> OnHealthChanged;

    /// <summary>
    /// Event fired when health reaches zero
    /// </summary>
    public event System.Action OnHealthDepleted;

    void Start()
    {
        // Initialize current health to maximum health
        currentHealth = maxHealth;
        
        // Notify listeners of initial health state
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// Applies damage to the player vehicle.
    /// Enforces health invariant: 0 <= currentHealth <= maxHealth
    /// </summary>
    /// <param name="amount">Amount of damage to apply (negative values treated as 0)</param>
    public void TakeDamage(float amount)
    {
        // Treat negative damage as 0
        if (amount < 0)
        {
            amount = 0;
        }

        // Calculate new health value
        float previousHealth = currentHealth;
        currentHealth -= amount;

        // Enforce lower bound: health cannot go below 0
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        // Enforce upper bound: health cannot exceed maxHealth
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Notify listeners if health actually changed
        if (currentHealth != previousHealth)
        {
            OnHealthChanged?.Invoke(currentHealth, maxHealth);

            // Check if health depleted
            if (currentHealth <= 0)
            {
                OnHealthDepleted?.Invoke();
            }
        }
    }

    /// <summary>
    /// Returns the current health as a percentage of maximum health.
    /// </summary>
    /// <returns>Health percentage in range [0, 1]</returns>
    public float GetHealthPercentage()
    {
        if (maxHealth <= 0)
        {
            return 0f;
        }

        return currentHealth / maxHealth;
    }
}

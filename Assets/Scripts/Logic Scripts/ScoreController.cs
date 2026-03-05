using UnityEngine;
using System;

// Manages score calculation based on survival time, speed, and combo multiplier
public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance { get; private set; }

    [Header("References")]
    public PlayerPhysics playerPhysics;
    public GameLogicController gameLogic;

    [Header("Score Settings")]
    public float baseScorePerSecond = 10f;
    public float speedBonusMultiplier = 1.5f; // Bonus when at high speed

    [Header("Combo Settings")]
    public int maxCombo = 10;
    public float baseComboTimer = 3f;
    public float minComboTimer = 1f;
    public float comboTimerDecreasePerLevel = 0.2f;

    [Header("Special Bonuses")]
    public float highSpeedThreshold = 0.8f; // 80% of max speed
    public float highSpeedBonus = 0.5f; // +50% score
    public int perfectOvertakeBonus = 100; // Bonus for 2+ near miss at once

    // Current state
    private float _currentScore = 0f;
    private int _currentCombo = 0;
    private float _comboTimer = 0f;
    private bool _isGameStarted = false;

    // Events for UI updates
    public event Action<float> OnScoreChanged;
    public event Action<int> OnComboChanged;
    public event Action<float, float> OnComboTimerChanged; // current, max

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Auto-find references
        if (playerPhysics == null)
            playerPhysics = FindFirstObjectByType<PlayerPhysics>();

        if (gameLogic == null)
            gameLogic = FindFirstObjectByType<GameLogicController>();
    }

    void Update()
    {
        if (!_isGameStarted || (gameLogic != null && gameLogic.isGameOver))
            return;

        UpdateScore();
        UpdateComboTimer();
    }

    // Start tracking score
    public void StartGame()
    {
        _isGameStarted = true;
        _currentScore = 0f;
        _currentCombo = 0;
        _comboTimer = 0f;

        OnScoreChanged?.Invoke(_currentScore);
        OnComboChanged?.Invoke(_currentCombo);
    }

    // Calculate and add score based on time and speed
    void UpdateScore()
    {
        if (playerPhysics == null) return;

        // Base score per frame
        float scoreThisFrame = baseScorePerSecond * Time.deltaTime;

        // Speed multiplier (0-1 based on current speed)
        float speedRatio = playerPhysics.GetCurrentSpeed() / playerPhysics.maxSpeed;
        scoreThisFrame *= (1f + speedRatio * speedBonusMultiplier);

        // High speed bonus
        if (speedRatio >= highSpeedThreshold)
        {
            scoreThisFrame *= (1f + highSpeedBonus);
        }

        // Combo multiplier
        int comboMultiplier = Mathf.Min(_currentCombo, maxCombo);
        if (comboMultiplier > 0)
        {
            scoreThisFrame *= comboMultiplier;
        }

        _currentScore += scoreThisFrame;
        OnScoreChanged?.Invoke(_currentScore);
    }

    // Update combo timer and reset if expired
    void UpdateComboTimer()
    {
        if (_currentCombo > 0)
        {
            _comboTimer -= Time.deltaTime;

            // Notify UI of timer change
            float maxTimer = GetComboTimerDuration();
            OnComboTimerChanged?.Invoke(_comboTimer, maxTimer);

            // Reset combo if timer expires
            if (_comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
    }

    // Add combo when near miss detected
    public void AddCombo(int amount = 1)
    {
        _currentCombo += amount;
        _currentCombo = Mathf.Min(_currentCombo, maxCombo);

        // Reset timer with new duration
        _comboTimer = GetComboTimerDuration();

        OnComboChanged?.Invoke(_currentCombo);
        OnComboTimerChanged?.Invoke(_comboTimer, _comboTimer);
    }

    // Add bonus score directly (for special events)
    public void AddBonusScore(float bonus)
    {
        _currentScore += bonus;
        OnScoreChanged?.Invoke(_currentScore);
    }

    // Reset combo to 0
    void ResetCombo()
    {
        _currentCombo = 0;
        _comboTimer = 0f;

        OnComboChanged?.Invoke(_currentCombo);
        OnComboTimerChanged?.Invoke(0f, baseComboTimer);
    }

    // Calculate combo timer duration based on current combo level
    float GetComboTimerDuration()
    {
        float duration = baseComboTimer - (_currentCombo * comboTimerDecreasePerLevel);
        return Mathf.Max(minComboTimer, duration);
    }

    // Getters
    public float GetCurrentScore() => _currentScore;
    public int GetCurrentCombo() => _currentCombo;
    public bool IsGameStarted() => _isGameStarted;
}

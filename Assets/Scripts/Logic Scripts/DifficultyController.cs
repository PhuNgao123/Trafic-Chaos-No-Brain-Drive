using UnityEngine;

// Controls difficulty scaling based on survival time
// Gradually increases spawn rate and vehicle speed over time
public class DifficultyController : MonoBehaviour
{
    public static DifficultyController Instance { get; private set; }

    [Header("Difficulty Curve")]
    public float easyDuration = 30f; // Easy phase duration (seconds)
    public float mediumDuration = 60f; // Medium phase duration
    public float hardDuration = 90f; // Hard phase duration
    // After hard phase, difficulty caps at maximum

    [Header("Spawn Rate Multipliers")]
    public float easySpawnMultiplier = 1.5f; // Slower spawns (higher = slower)
    public float mediumSpawnMultiplier = 1.0f; // Normal spawns
    public float hardSpawnMultiplier = 0.7f; // Faster spawns
    public float extremeSpawnMultiplier = 0.5f; // Very fast spawns

    [Header("Speed Multipliers")]
    public float easySpeedMultiplier = 0.8f; // Slower vehicles
    public float mediumSpeedMultiplier = 1.0f; // Normal speed
    public float hardSpeedMultiplier = 1.2f; // Faster vehicles
    public float extremeSpeedMultiplier = 1.4f; // Very fast vehicles

    private float _gameStartTime;
    private float _currentTime;
    private bool _isGameStarted = false;

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

    void Update()
    {
        // Start tracking time when game starts
        if (!_isGameStarted && GameLogicController.Instance != null && GameLogicController.Instance.isGameStarted)
        {
            _isGameStarted = true;
            _gameStartTime = Time.time;
            Debug.Log("[Difficulty] Game started - Difficulty: Easy");
        }

        if (_isGameStarted && GameLogicController.Instance != null && !GameLogicController.Instance.isGameOver)
        {
            float previousTime = _currentTime;
            _currentTime = Time.time - _gameStartTime;

            // Log difficulty changes
            string previousDifficulty = GetDifficultyNameAtTime(previousTime);
            string currentDifficulty = GetDifficultyName();
            
            if (previousDifficulty != currentDifficulty)
            {
                Debug.Log($"[Difficulty] Changed to {currentDifficulty} | Spawn Rate: x{GetSpawnRateMultiplier():F2} | Speed: x{GetSpeedMultiplier():F2}");
            }
        }
    }

    // Helper to get difficulty name at specific time
    string GetDifficultyNameAtTime(float time)
    {
        if (time < easyDuration)
            return "Easy";
        else if (time < easyDuration + mediumDuration)
            return "Medium";
        else if (time < easyDuration + mediumDuration + hardDuration)
            return "Hard";
        else
            return "Extreme";
    }

    // Returns current spawn rate multiplier based on survival time
    public float GetSpawnRateMultiplier()
    {
        if (!_isGameStarted) return easySpawnMultiplier;

        if (_currentTime < easyDuration)
        {
            // Easy phase: lerp from easy to medium
            float t = _currentTime / easyDuration;
            return Mathf.Lerp(easySpawnMultiplier, mediumSpawnMultiplier, t);
        }
        else if (_currentTime < easyDuration + mediumDuration)
        {
            // Medium phase: lerp from medium to hard
            float t = (_currentTime - easyDuration) / mediumDuration;
            return Mathf.Lerp(mediumSpawnMultiplier, hardSpawnMultiplier, t);
        }
        else if (_currentTime < easyDuration + mediumDuration + hardDuration)
        {
            // Hard phase: lerp from hard to extreme
            float t = (_currentTime - easyDuration - mediumDuration) / hardDuration;
            return Mathf.Lerp(hardSpawnMultiplier, extremeSpawnMultiplier, t);
        }
        else
        {
            // Extreme phase: cap at maximum difficulty
            return extremeSpawnMultiplier;
        }
    }

    // Returns current speed multiplier based on survival time
    public float GetSpeedMultiplier()
    {
        if (!_isGameStarted) return easySpeedMultiplier;

        if (_currentTime < easyDuration)
        {
            // Easy phase: lerp from easy to medium
            float t = _currentTime / easyDuration;
            return Mathf.Lerp(easySpeedMultiplier, mediumSpeedMultiplier, t);
        }
        else if (_currentTime < easyDuration + mediumDuration)
        {
            // Medium phase: lerp from medium to hard
            float t = (_currentTime - easyDuration) / mediumDuration;
            return Mathf.Lerp(mediumSpeedMultiplier, hardSpeedMultiplier, t);
        }
        else if (_currentTime < easyDuration + mediumDuration + hardDuration)
        {
            // Hard phase: lerp from hard to extreme
            float t = (_currentTime - easyDuration - mediumDuration) / hardDuration;
            return Mathf.Lerp(hardSpeedMultiplier, extremeSpeedMultiplier, t);
        }
        else
        {
            // Extreme phase: cap at maximum difficulty
            return extremeSpeedMultiplier;
        }
    }

    // Returns current difficulty level name for UI
    public string GetDifficultyName()
    {
        if (!_isGameStarted) return "Easy";

        if (_currentTime < easyDuration)
            return "Easy";
        else if (_currentTime < easyDuration + mediumDuration)
            return "Medium";
        else if (_currentTime < easyDuration + mediumDuration + hardDuration)
            return "Hard";
        else
            return "Extreme";
    }

    // Returns survival time
    public float GetSurvivalTime()
    {
        return _currentTime;
    }
}

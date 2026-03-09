using UnityEngine;

// Manages player currency (coins) with persistent save/load
// Coins are earned from score: 100 score = 1 coin
public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    private const string COIN_KEY = "PlayerCoins";
    private const string HIGHSCORE_KEY = "HighScore";
    private const float SCORE_TO_COIN_RATIO = 100f; // 100 score = 1 coin

    private int _currentCoins;
    private float _currentHighScore;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Load saved data from PlayerPrefs
    void LoadData()
    {
        _currentCoins = PlayerPrefs.GetInt(COIN_KEY, 0);
        _currentHighScore = PlayerPrefs.GetFloat(HIGHSCORE_KEY, 0f);
    }

    // Save data to PlayerPrefs
    void SaveData()
    {
        PlayerPrefs.SetInt(COIN_KEY, _currentCoins);
        PlayerPrefs.SetFloat(HIGHSCORE_KEY, _currentHighScore);
        PlayerPrefs.Save();
    }

    // Add coins to player balance
    public void AddCoins(int amount)
    {
        int previousCoins = _currentCoins;
        _currentCoins += amount;
        SaveData();
        
        Debug.Log($"[Currency] +{amount} coins | Total: {_currentCoins} coins (was {previousCoins})");
    }

    // Spend coins (returns true if successful)
    public bool SpendCoins(int amount)
    {
        if (_currentCoins >= amount)
        {
            _currentCoins -= amount;
            SaveData();
            return true;
        }
        return false;
    }

    // Convert score to coins and add to balance
    public int ConvertScoreToCoins(float score)
    {
        int coins = Mathf.FloorToInt(score / SCORE_TO_COIN_RATIO);
        
        if (coins > 0)
        {
            Debug.Log($"[Currency] Converting {score:F0} score to {coins} coins (ratio: {SCORE_TO_COIN_RATIO}:1)");
            AddCoins(coins);
        }
        else
        {
            Debug.Log($"[Currency] Score {score:F0} is too low to convert (need {SCORE_TO_COIN_RATIO} for 1 coin)");
        }
        
        return coins;
    }

    // Update high score if current score is higher
    public bool UpdateHighScore(float score)
    {
        if (score > _currentHighScore)
        {
            float previousHighScore = _currentHighScore;
            _currentHighScore = score;
            SaveData();
            
            Debug.Log($"[Currency] NEW HIGH SCORE! {score:F0} (previous: {previousHighScore:F0})");
            return true; // New high score!
        }
        
        return false;
    }

    // Getters
    public int GetCoins() => _currentCoins;
    public float GetHighScore() => _currentHighScore;

    // For debugging/testing
    public void ResetData()
    {
        _currentCoins = 0;
        _currentHighScore = 0f;
        SaveData();
    }
}

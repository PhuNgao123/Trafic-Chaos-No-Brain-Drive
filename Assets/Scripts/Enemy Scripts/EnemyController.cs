using UnityEngine;
using System.Collections.Generic;

// Central controller for all enemy vehicle spawners
// Manages spawn rates and vehicle speeds based on player speed and survival time
// All spawners receive settings from this controller
public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public PlayerPhysics playerPhysics;
    public List<VehicleSpawner> spawners = new List<VehicleSpawner>();

    [Header("Vehicle Prefabs")]
    public List<GameObject> vehiclePrefabs = new List<GameObject>();

    [Header("Base Spawn Settings")]
    public float baseMinInterval = 2f;
    public float baseMaxInterval = 4f;
    public float spawnCheckDistance = 10f;

    [Header("Base Speed Settings")]
    public float baseSpeed = 15f;
    public float speedRandomness = 0.2f;
    public float playerSpeedMultiplier = 0.5f;

    [Header("Direction Multipliers")]
    [Tooltip("Direction 1 (opposite) - spawn interval multiplier")]
    public float direction1IntervalMultiplier = 0.7f;
    [Tooltip("Direction 1 (opposite) - speed multiplier")]
    public float direction1SpeedMultiplier = 1.5f;
    
    [Tooltip("Direction -1 (same) - spawn interval multiplier")]
    public float directionMinus1IntervalMultiplier = 1.2f;
    [Tooltip("Direction -1 (same) - speed multiplier")]
    public float directionMinus1SpeedMultiplier = 0.8f;

    [Header("Survival Time Difficulty")]
    [Tooltip("How much survival time affects spawn rate (higher = more aggressive over time)")]
    public float survivalTimeSpawnFactor = 0.02f;
    [Tooltip("How much survival time affects speed (higher = faster vehicles over time)")]
    public float survivalTimeSpeedFactor = 0.01f;
    [Tooltip("Maximum multiplier from survival time")]
    public float maxSurvivalMultiplier = 2f;

    [Header("Player Speed Difficulty")]
    [Tooltip("How much player speed affects spawn rate (higher = more spawn when fast)")]
    public float playerSpeedSpawnFactor = 0.3f;
    [Tooltip("Minimum spawn rate multiplier when player is slow")]
    public float minPlayerSpeedMultiplier = 0.5f;

    [Header("Anti-Wall Spawn Settings")]
    public float minGlobalSpawnInterval = 0.1f;
    [HideInInspector] public float lastGlobalSpawnTime = -999f;

    private float _gameStartTime;
    private bool _gameStarted = false;

    // Public method to stop all spawning
    public void StopAllSpawning()
    {
        foreach (var spawner in spawners)
        {
            if (spawner != null)
                spawner.stopSpawn = true;
        }
    }

    void Start()
    {
        RefreshPlayerReference();
        InjectSettings();
    }

    void Update()
    {
        // Track game start time
        if (!_gameStarted && GameLogicController.Instance != null && GameLogicController.Instance.isGameStarted)
        {
            _gameStarted = true;
            _gameStartTime = Time.time;
            Debug.Log("[EnemyController] Game started - tracking survival time for difficulty");
        }
    }
    
    public void RefreshPlayerReference()
    {
        // Auto-find references if not assigned
        if (playerPhysics == null)
            playerPhysics = FindFirstObjectByType<PlayerPhysics>();

        if (spawners.Count == 0)
            spawners.AddRange(FindObjectsByType<VehicleSpawner>(FindObjectsSortMode.None));
            
        // Re-inject settings to all spawners with new player reference
        InjectSettings();
        
        Debug.Log("EnemyController: Refreshed player references");
    }

    // Injects all settings into spawners so they don't need individual configuration
    void InjectSettings()
    {
        foreach (var spawner in spawners)
        {
            if (spawner == null) continue;

            spawner.playerPhysics = playerPhysics;
            spawner.enemyController = this;
            spawner.vehiclePrefabs = vehiclePrefabs;
            spawner.baseMinInterval = baseMinInterval;
            spawner.baseMaxInterval = baseMaxInterval;
            spawner.spawnCheckDistance = spawnCheckDistance;
            spawner.baseSpeed = baseSpeed;
            spawner.speedRandomness = speedRandomness;
            spawner.playerSpeedMultiplier = playerSpeedMultiplier;
        }
    }

    // Get current survival time
    float GetSurvivalTime()
    {
        if (!_gameStarted) return 0f;
        return Time.time - _gameStartTime;
    }

    // Get current player speed normalized (0-1)
    float GetPlayerSpeedNormalized()
    {
        if (playerPhysics == null) return 0f;
        
        return Mathf.InverseLerp(
            playerPhysics.minSpeed,
            playerPhysics.maxSpeed,
            playerPhysics.GetCurrentSpeed()
        );
    }

    // Calculate difficulty multiplier based on survival time
    float GetSurvivalDifficultyMultiplier()
    {
        float survivalTime = GetSurvivalTime();
        float multiplier = 1f + (survivalTime * survivalTimeSpawnFactor);
        return Mathf.Min(multiplier, maxSurvivalMultiplier);
    }

    // Calculate difficulty multiplier based on player speed
    float GetPlayerSpeedDifficultyMultiplier()
    {
        float speedNormalized = GetPlayerSpeedNormalized();
        float multiplier = minPlayerSpeedMultiplier + (speedNormalized * playerSpeedSpawnFactor);
        return multiplier;
    }

    // Returns spawn interval multiplier based on direction, player speed, and survival time
    public float GetIntervalMultiplier(int direction)
    {
        float baseMultiplier = 1f;
        
        // Apply direction multiplier
        if (direction == 1) 
            baseMultiplier = direction1IntervalMultiplier;
        else if (direction == -1) 
            baseMultiplier = directionMinus1IntervalMultiplier;

        // Apply survival time difficulty (lower multiplier = faster spawn)
        float survivalTime = GetSurvivalTime();
        float survivalMultiplier = 1f + (survivalTime * survivalTimeSpawnFactor);
        survivalMultiplier = Mathf.Min(survivalMultiplier, maxSurvivalMultiplier);
        baseMultiplier /= survivalMultiplier; // Divide to make spawn faster over time

        // Apply player speed difficulty (faster player = more spawn)
        float playerSpeedMultiplier = GetPlayerSpeedDifficultyMultiplier();
        baseMultiplier /= playerSpeedMultiplier; // Divide to make spawn faster when player is fast

        float playerSpeed = GetPlayerSpeedNormalized();
        
        Debug.Log($"[EnemyController] Spawn Interval Dir{direction} - Base: {(direction == 1 ? direction1IntervalMultiplier : directionMinus1IntervalMultiplier):F2}, " +
                  $"Survival: {survivalTime:F1}s (/÷{survivalMultiplier:F2}), PlayerSpeed: {playerSpeed:F2} (/÷{playerSpeedMultiplier:F2}), Final: {baseMultiplier:F2}");

        return Mathf.Max(0.05f, baseMultiplier); // Prevent too fast spawning but allow very dense spawns
    }

    // Returns speed multiplier based on direction, player speed, and survival time
    public float GetSpeedMultiplier(int direction)
    {
        float baseMultiplier = 1f;
        
        // Apply direction multiplier
        if (direction == 1) 
            baseMultiplier = direction1SpeedMultiplier;
        else if (direction == -1) 
            baseMultiplier = directionMinus1SpeedMultiplier;

        // Apply survival time difficulty (higher multiplier = faster vehicles)
        float survivalTime = GetSurvivalTime();
        float survivalSpeedMultiplier = 1f + (survivalTime * survivalTimeSpeedFactor);
        survivalSpeedMultiplier = Mathf.Min(survivalSpeedMultiplier, maxSurvivalMultiplier);
        baseMultiplier *= survivalSpeedMultiplier;

        // Apply player speed influence (faster player = slightly faster enemies)
        float playerSpeedInfluence = 1f + (GetPlayerSpeedNormalized() * 0.2f);
        baseMultiplier *= playerSpeedInfluence;

        Debug.Log($"[EnemyController] Speed Dir{direction} - Base: {(direction == 1 ? direction1SpeedMultiplier : directionMinus1SpeedMultiplier):F2}, " +
                  $"Survival: {survivalTime:F1}s (x{survivalSpeedMultiplier:F2}), PlayerInfluence: x{playerSpeedInfluence:F2}, Final: {baseMultiplier:F2}");

        return baseMultiplier;
    }

    // Get current difficulty info for UI/debugging
    public string GetDifficultyInfo()
    {
        float survivalTime = GetSurvivalTime();
        float playerSpeed = GetPlayerSpeedNormalized();
        
        string difficulty = "Easy";
        if (survivalTime > 60f) difficulty = "Extreme";
        else if (survivalTime > 40f) difficulty = "Hard";
        else if (survivalTime > 20f) difficulty = "Medium";
        
        return $"{difficulty} | Time: {survivalTime:F1}s | Speed: {playerSpeed:F1}";
    }
}

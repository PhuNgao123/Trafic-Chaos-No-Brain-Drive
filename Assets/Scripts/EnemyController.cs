using UnityEngine;
using System.Collections.Generic;

// Central controller for all enemy vehicle spawners
// Manages spawn rates and vehicle speeds based on player speed
// All spawners receive settings from this controller
public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public PlayerPhysics playerPhysics;
    public List<VehicleSpawner> spawners = new List<VehicleSpawner>();

    [Header("Spawn Settings")]
    public float baseMinInterval = 2f;
    public float baseMaxInterval = 4f;
    public float spawnCheckDistance = 10f;

    [Header("Speed Settings")]
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

    void Start()
    {
        // Auto-find references if not assigned
        if (playerPhysics == null)
            playerPhysics = FindFirstObjectByType<PlayerPhysics>();

        if (spawners.Count == 0)
            spawners.AddRange(FindObjectsByType<VehicleSpawner>(FindObjectsSortMode.None));

        // Inject settings into all spawners
        InjectSettings();
    }

    // Injects all settings into spawners so they don't need individual configuration
    void InjectSettings()
    {
        foreach (var spawner in spawners)
        {
            if (spawner == null) continue;

            spawner.playerPhysics = playerPhysics;
            spawner.enemyController = this;
            spawner.baseMinInterval = baseMinInterval;
            spawner.baseMaxInterval = baseMaxInterval;
            spawner.spawnCheckDistance = spawnCheckDistance;
            spawner.baseSpeed = baseSpeed;
            spawner.speedRandomness = speedRandomness;
            spawner.playerSpeedMultiplier = playerSpeedMultiplier;
        }
    }

    // Returns spawn interval multiplier based on vehicle direction
    // Direction 1 (opposite): spawns more frequently
    // Direction -1 (same): spawns less frequently
    public float GetIntervalMultiplier(int direction)
    {
        if (direction == 1) return direction1IntervalMultiplier;
        if (direction == -1) return directionMinus1IntervalMultiplier;
        return 1f;
    }

    // Returns speed multiplier based on vehicle direction
    // Direction 1 (opposite): moves faster (more challenging)
    // Direction -1 (same): moves slower (less challenging)
    public float GetSpeedMultiplier(int direction)
    {
        if (direction == 1) return direction1SpeedMultiplier;
        if (direction == -1) return directionMinus1SpeedMultiplier;
        return 1f;
    }
}

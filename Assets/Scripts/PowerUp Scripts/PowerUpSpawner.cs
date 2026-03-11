using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns power-ups randomly on the road at intervals.
/// Power-ups are placed within lane boundaries and move with the road.
/// </summary>
public class PowerUpSpawner : MonoBehaviour
{
    [Header("=== POWER-UP PREFABS ===")]
    [Tooltip("List of power-up prefabs to spawn")]
    public GameObject[] powerUpPrefabs;

    [Header("=== SPAWN SETTINGS ===")]
    [Tooltip("Minimum time between spawns (seconds)")]
    public float minSpawnInterval = 5f;

    [Tooltip("Maximum time between spawns (seconds)")]
    public float maxSpawnInterval = 10f;

    [Tooltip("Distance ahead of player to spawn (Z position)")]
    public float spawnDistanceAhead = 50f;

    [Tooltip("Height above road to spawn")]
    public float spawnHeight = 6f;

    [Header("=== LANE SETTINGS ===")]
    [Tooltip("Number of lanes on the road")]
    public int numberOfLanes = 3;

    [Tooltip("Width of each lane")]
    public float laneWidth = 3f;

    [Tooltip("Center offset of the road (X position)")]
    public float roadCenterX = 0f;

    [Tooltip("Lanes to exclude from spawning (e.g., middle lane with barrier). Comma separated: 0,1,2")]
    public string excludedLanes = "1"; // Lane 1 is middle lane (0-indexed)

    [Header("=== REFERENCES ===")]
    [Tooltip("Road container to parent power-ups to (so they move with road)")]
    public Transform roadContainer;

    [Tooltip("Player transform to calculate spawn position")]
    public Transform player;

    private float _nextSpawnTime;
    private bool _isSpawning = false;

    void Start()
    {
        // Auto-find references if not set
        if (roadContainer == null)
        {
            RoadMover roadMover = FindFirstObjectByType<RoadMover>();
            if (roadMover != null)
            {
                roadContainer = roadMover.transform;
            }
        }

        if (player == null)
        {
            PlayerPhysics playerPhysics = FindFirstObjectByType<PlayerPhysics>();
            if (playerPhysics != null)
            {
                player = playerPhysics.transform;
            }
        }

        // Schedule first spawn
        ScheduleNextSpawn();
    }

    void Update()
    {
        // Check if game is running
        if (GameLogicController.Instance != null && !GameLogicController.Instance.isGameStarted)
        {
            return;
        }

        // Check if it's time to spawn
        if (_isSpawning && Time.time >= _nextSpawnTime)
        {
            SpawnPowerUp();
            ScheduleNextSpawn();
        }
    }

    /// <summary>
    /// Start spawning power-ups
    /// </summary>
    public void StartSpawning()
    {
        _isSpawning = true;
        ScheduleNextSpawn();
    }

    /// <summary>
    /// Stop spawning power-ups
    /// </summary>
    public void StopSpawning()
    {
        _isSpawning = false;
    }

    /// <summary>
    /// Schedule the next spawn time
    /// </summary>
    void ScheduleNextSpawn()
    {
        float interval = Random.Range(minSpawnInterval, maxSpawnInterval);
        _nextSpawnTime = Time.time + interval;
    }

    /// <summary>
    /// Spawn a random power-up at a random lane position
    /// </summary>
    void SpawnPowerUp()
    {
        Debug.Log("[PowerUpSpawner] SpawnPowerUp called!");
        
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0)
        {
            Debug.LogWarning("[PowerUpSpawner] No power-up prefabs assigned!");
            return;
        }

        Debug.Log($"[PowerUpSpawner] Power-up prefabs count: {powerUpPrefabs.Length}");

        // Try to find player if not set
        if (player == null)
        {
            PlayerPhysics playerPhysics = FindFirstObjectByType<PlayerPhysics>();
            if (playerPhysics != null)
            {
                player = playerPhysics.transform;
                Debug.Log("[PowerUpSpawner] Found player reference!");
            }
            else
            {
                Debug.LogWarning("[PowerUpSpawner] Player reference not found!");
                return;
            }
        }

        // Choose random power-up prefab
        GameObject prefab = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        
        if (prefab == null)
        {
            Debug.LogError("[PowerUpSpawner] Selected prefab is null!");
            return;
        }

        Debug.Log($"[PowerUpSpawner] Selected prefab: {prefab.name}");

        // Get list of valid lanes (excluding barrier lanes)
        List<int> validLanes = GetValidLanes();
        
        if (validLanes.Count == 0)
        {
            Debug.LogWarning("[PowerUpSpawner] No valid lanes to spawn in!");
            return;
        }

        // Choose random valid lane
        int laneIndex = validLanes[Random.Range(0, validLanes.Count)];
        float laneX = GetLaneXPosition(laneIndex);

        // Calculate spawn position (ahead of player, in world space)
        Vector3 spawnPosition = new Vector3(
            laneX,
            spawnHeight,
            player.position.z + spawnDistanceAhead
        );

        Debug.Log($"[PowerUpSpawner] Spawning at position: {spawnPosition}");

        // Spawn power-up
        GameObject powerUp = Instantiate(prefab, spawnPosition, Quaternion.identity);

        // Parent to road container so it moves with the road
        if (roadContainer != null)
        {
            powerUp.transform.SetParent(roadContainer);
            Debug.Log($"[PowerUpSpawner] Parented to road container");
        }
        else
        {
            Debug.LogWarning("[PowerUpSpawner] Road container is null! Power-up won't move with road.");
        }

        Debug.Log($"[PowerUpSpawner] ✓ Spawned {prefab.name} at lane {laneIndex} (X: {laneX}, Z: {spawnPosition.z})");
    }

    /// <summary>
    /// Get list of valid lanes (excluding barrier lanes)
    /// </summary>
    List<int> GetValidLanes()
    {
        List<int> validLanes = new List<int>();
        
        // Parse excluded lanes
        HashSet<int> excluded = new HashSet<int>();
        if (!string.IsNullOrEmpty(excludedLanes))
        {
            string[] parts = excludedLanes.Split(',');
            foreach (string part in parts)
            {
                if (int.TryParse(part.Trim(), out int laneIndex))
                {
                    excluded.Add(laneIndex);
                }
            }
        }

        // Add all non-excluded lanes
        for (int i = 0; i < numberOfLanes; i++)
        {
            if (!excluded.Contains(i))
            {
                validLanes.Add(i);
            }
        }

        return validLanes;
    }

    /// <summary>
    /// Get X position for a specific lane
    /// </summary>
    float GetLaneXPosition(int laneIndex)
    {
        // Calculate X position based on lane index
        // Center lane is at roadCenterX
        float totalWidth = numberOfLanes * laneWidth;
        float startX = roadCenterX - (totalWidth / 2f) + (laneWidth / 2f);
        return startX + (laneIndex * laneWidth);
    }

    /// <summary>
    /// Manually spawn a power-up (for testing)
    /// </summary>
    public void SpawnPowerUpManual()
    {
        SpawnPowerUp();
    }

    // Visualize lanes in editor
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.yellow;

        // Draw lane positions
        for (int i = 0; i < numberOfLanes; i++)
        {
            float laneX = GetLaneXPosition(i);
            Vector3 start = new Vector3(laneX, spawnHeight, -50f);
            Vector3 end = new Vector3(laneX, spawnHeight, 50f);
            Gizmos.DrawLine(start, end);
        }
    }
}

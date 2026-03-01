﻿using UnityEngine;
using System.Collections.Generic;

// Spawns enemy vehicles at calculated intervals
// Only needs to define direction - all other settings injected by EnemyController
public class VehicleSpawner : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public PlayerPhysics playerPhysics;
    [HideInInspector] public EnemyController enemyController;

    [Header("Direction")]
    public int direction = 1; // 1 = opposite direction, -1 = same direction

    [Header("Settings (Injected by EnemyController)")]
    [HideInInspector] public List<GameObject> vehiclePrefabs;
    [HideInInspector] public float baseMinInterval;
    [HideInInspector] public float baseMaxInterval;
    [HideInInspector] public float spawnCheckDistance;
    [HideInInspector] public float baseSpeed;
    [HideInInspector] public float speedRandomness;
    [HideInInspector] public float playerSpeedMultiplier;

    private float _nextSpawnTime;

    void Start()
    {
        ScheduleNext();
    }

    void Update()
    {
        if (Time.time >= _nextSpawnTime)
        {
            // Only spawn if location is clear
            if (CanSpawn())
            {
                Spawn();
                ScheduleNext();
            }
            else
            {
                // Retry soon if blocked
                _nextSpawnTime = Time.time + 0.3f;
            }
        }
    }

    // Calculates next spawn time based on player speed and direction
    // Faster player = shorter intervals (more vehicles)
    // Direction multiplier applied from EnemyController
    void ScheduleNext()
    {
        float speed01 = 0f;

        if (playerPhysics != null)
        {
            speed01 = Mathf.InverseLerp(
                playerPhysics.minSpeed,
                playerPhysics.maxSpeed,
                playerPhysics.GetCurrentSpeed()
            );
        }

        // Lerp intervals based on player speed
        float minInterval = Mathf.Lerp(baseMaxInterval, baseMinInterval, speed01);
        float maxInterval = Mathf.Lerp(baseMaxInterval * 1.3f, baseMinInterval * 1.3f, speed01);

        // Apply direction multiplier from controller
        if (enemyController != null)
        {
            float multiplier = enemyController.GetIntervalMultiplier(direction);
            minInterval *= multiplier;
            maxInterval *= multiplier;
        }

        _nextSpawnTime = Time.time + Random.Range(minInterval, maxInterval);
    }

    // Checks if spawn location is clear using raycast
    // Returns false if another vehicle is in the way
    bool CanSpawn()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 dir = (direction == 1) ? Vector3.back : Vector3.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, spawnCheckDistance))
        {
            if (hit.collider.CompareTag("Vehicle"))
                return false;
        }
        return true;
    }

    // Spawns vehicle with calculated speed and proper rotation
    // Randomly selects prefab from controller's list
    // Speed = base + player speed + direction multiplier + randomness
    void Spawn()
    {
        // Check if prefabs list is valid
        if (vehiclePrefabs == null || vehiclePrefabs.Count == 0)
            return;

        // Randomly select a prefab
        GameObject prefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count)];
        if (prefab == null)
            return;

        GameObject vehicle = Instantiate(prefab, transform.position, Quaternion.identity, transform);

        // Calculate vehicle speed
        float vehicleSpeed = baseSpeed;

        // Add portion of player speed
        if (playerPhysics != null)
            vehicleSpeed += playerPhysics.GetCurrentSpeed() * playerSpeedMultiplier;

        // Apply direction multiplier from controller
        if (enemyController != null)
            vehicleSpeed *= enemyController.GetSpeedMultiplier(direction);

        // Add randomness
        vehicleSpeed *= Random.Range(1f - speedRandomness, 1f + speedRandomness);
        vehicleSpeed = Mathf.Max(5f, vehicleSpeed);

        // Initialize vehicle
        VehicleMove vm = vehicle.GetComponent<VehicleMove>();
        if (vm != null)
            vm.Init(vehicleSpeed, direction);

        // Set rotation based on direction
        vehicle.transform.rotation = (direction == 1) 
            ? Quaternion.Euler(0, 180, 0)  // Opposite direction: face backward
            : Quaternion.identity;          // Same direction: face forward
    }
}

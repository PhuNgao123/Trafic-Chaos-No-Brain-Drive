using UnityEngine;
using System.Collections.Generic;

// Simple pavement spawner without StartPoint / EndPoint
// Spawns segments by fixed length (e.g. 30 units per prefab)
public class PavementSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public List<GameObject> pavementPrefabs;

    [Header("Pavement Settings")]
    public float segmentLength = 30f;   // Length of each prefab
    public int visibleSegments = 25;
    public float deleteDistance = -50f;
    public float menuDeleteDistance = -200f; // Delete distance before game starts
    public float menuStartZ = -200f; // Starting Z position for menu

    [Header("Position Settings")]
    public float xOffset = 0f;
    [Tooltip("1 = normal, -1 = rotated 180 degrees")]
    public int direction = 1;

    private Queue<GameObject> pavements = new Queue<GameObject>();

    void Start()
    {
        direction = direction >= 0 ? 1 : -1;

        // Spawn from menu start position
        float currentZ = menuStartZ;

        for (int i = 0; i < visibleSegments; i++)
        {
            SpawnSegmentAt(currentZ);
            currentZ += segmentLength;
        }
    }

    void Update()
    {
        if (pavements.Count == 0)
            return;

        // Determine delete distance based on game state
        float currentDeleteDistance = (GameLogicController.Instance != null && GameLogicController.Instance.isGameStarted)
            ? deleteDistance
            : menuDeleteDistance;

        GameObject first = pavements.Peek();
        if (first != null && first.transform.position.z < currentDeleteDistance)
        {
            Destroy(pavements.Dequeue());
            
            // Spawn next segment after the last one
            if (pavements.Count > 0)
            {
                GameObject last = pavements.ToArray()[pavements.Count - 1];
                if (last != null)
                {
                    float nextZ = last.transform.position.z + segmentLength;
                    SpawnSegmentAt(nextZ);
                }
            }
        }
    }

    void SpawnSegmentAt(float zPosition)
    {
        if (pavementPrefabs == null || pavementPrefabs.Count == 0)
            return;

        GameObject prefab = pavementPrefabs[Random.Range(0, pavementPrefabs.Count)];
        if (prefab == null)
            return;

        Vector3 spawnPos = new Vector3(
            xOffset * direction,
            0f,
            zPosition
        );

        Quaternion rot = direction == 1
            ? Quaternion.identity
            : Quaternion.Euler(0f, 180f, 0f);

        GameObject pavement = Instantiate(prefab, spawnPos, rot, transform);
        pavements.Enqueue(pavement);
    }
}
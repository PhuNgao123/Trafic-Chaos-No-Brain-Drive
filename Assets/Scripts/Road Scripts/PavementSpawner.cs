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
    public float deleteDistance = -30f;

    [Header("Position Settings")]
    public float xOffset = 0f;
    [Tooltip("1 = normal, -1 = rotated 180 degrees")]
    public int direction = 1;

    private Queue<GameObject> pavements = new Queue<GameObject>();

    void Start()
    {
        direction = direction >= 0 ? 1 : -1;

        for (int i = 0; i < visibleSegments; i++)
        {
            SpawnSegment();
        }
    }

    void Update()
    {
        if (pavements.Count == 0)
            return;

        GameObject first = pavements.Peek();
        if (first != null && first.transform.position.z < deleteDistance)
        {
            Destroy(pavements.Dequeue());
            SpawnSegment();
        }
    }

    void SpawnSegment()
    {
        if (pavementPrefabs == null || pavementPrefabs.Count == 0)
            return;

        GameObject prefab = pavementPrefabs[Random.Range(0, pavementPrefabs.Count)];
        if (prefab == null)
            return;

        // Calculate spawn position based on last pavement
        float spawnZ = 0f;
        if (pavements.Count > 0)
        {
            GameObject last = pavements.ToArray()[pavements.Count - 1];
            if (last != null)
            {
                spawnZ = last.transform.position.z + segmentLength;
            }
        }

        Vector3 spawnPos = new Vector3(
            xOffset * direction,
            0f,
            spawnZ
        );

        Quaternion rot = direction == 1
            ? Quaternion.identity
            : Quaternion.Euler(0f, 180f, 0f);

        GameObject pavement = Instantiate(prefab, spawnPos, rot, transform);
        pavements.Enqueue(pavement);
    }
}
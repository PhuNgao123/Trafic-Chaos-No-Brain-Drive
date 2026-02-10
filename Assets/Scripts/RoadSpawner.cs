using UnityEngine;
using System.Collections.Generic;

public class RoadSpawner : MonoBehaviour
{
    [Header("=== PREFABS ===")]
    public GameObject road1Prefab;
    public GameObject road2Prefab;
    public GameObject roadCrossPrefab;

    [Header("=== ROAD SETTINGS ===")]
    public float segmentLength = 15f;
    public int visibleSegments = 25;

    [Header("=== CURVE (TRÁI PHẢI) ===")]
    public float curveFrequency = 0.02f;
    public float curveAmplitude = 4f;

    [Header("=== ELEVATION (LÊN XUỐNG) ===")]
    public float elevationFrequency = 0.015f;
    public float elevationAmplitude = 2f;
    public float baseHeight = 0f;

    [Header("=== SPAWN PATTERN ===")]
    public int minNormalBeforeCross = 10;
    public int maxNormalBeforeCross = 20;

    Queue<GameObject> roads = new Queue<GameObject>();
    int currentIndex = 0;
    int patternIndex = 0;
    int normalCount = 0;
    int nextCrossAt;
    int crossRemain = 0;

    void Start()
    {
        nextCrossAt = Random.Range(minNormalBeforeCross, maxNormalBeforeCross + 1);

        for (int i = 0; i < visibleSegments; i++)
        {
            SpawnSegment(currentIndex++);
        }
    }

    void SpawnSegment(int index)
    {
        GameObject prefab = GetNextPrefab();
        GameObject road = Instantiate(prefab, transform);

        Vector3 pos = GetPositionAtIndex(index);
        Quaternion rot = GetRotationAtIndex(index);

        road.transform.position = pos;
        road.transform.rotation = rot;

        roads.Enqueue(road);
    }

    Vector3 GetPositionAtIndex(int index)
    {
        float z = index * segmentLength;
        float x = Mathf.Sin(z * curveFrequency) * curveAmplitude;
        float y = Mathf.Sin(z * elevationFrequency) * elevationAmplitude + baseHeight;

        return new Vector3(x, y, z);
    }

    Quaternion GetRotationAtIndex(int index)
    {
        Vector3 current = GetPositionAtIndex(index);
        Vector3 next = GetPositionAtIndex(index + 1);
        Vector3 dir = (next - current).normalized;

        return Quaternion.LookRotation(dir, Vector3.up);
    }

    GameObject GetNextPrefab()
    {
        if (crossRemain > 0)
        {
            crossRemain--;
            normalCount++;
            return roadCrossPrefab;
        }

        if (normalCount >= nextCrossAt && roadCrossPrefab != null)
        {
            normalCount = 0;
            nextCrossAt = Random.Range(minNormalBeforeCross, maxNormalBeforeCross + 1);
            crossRemain = 1;
            return roadCrossPrefab;
        }

        normalCount++;

        GameObject prefab =
            (patternIndex == 0 || patternIndex == 1 || patternIndex == 3 || patternIndex == 4)
            ? road1Prefab
            : road2Prefab;

        patternIndex = (patternIndex + 1) % 6;
        return prefab;
    }

    // === PUBLIC API (PAKO STYLE) ===
    public Vector3 GetPositionAtZ(float z)
    {
        float x = Mathf.Sin(z * curveFrequency) * curveAmplitude;
        float y = Mathf.Sin(z * elevationFrequency) * elevationAmplitude + baseHeight;
        return new Vector3(x, y, z);
    }

    public Vector3 GetDirectionAtZ(float z)
    {
        Vector3 cur = GetPositionAtZ(z);
        Vector3 next = GetPositionAtZ(z + 0.1f);
        return (next - cur).normalized;
    }
}

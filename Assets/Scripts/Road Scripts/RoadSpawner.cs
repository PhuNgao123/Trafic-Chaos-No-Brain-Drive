using UnityEngine;
using System.Collections.Generic;

// Spawns road segments with elevation (up/down slopes)
// Roads are straight (no curves) and spawn seamlessly using StartPoint/EndPoint alignment
public class RoadSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject road1Prefab;
    public GameObject road2Prefab;
    public List<GameObject> specialRoadPrefabs;

    [Header("Road Settings")]
    public float segmentLength = 15f;
    public float overlapOffset = 0.5f;
    public int visibleSegments = 25;
    public float deleteDistance = -30f;

    [Header("Elevation (Up/Down Slopes)")]
    public float elevationFrequency = 0.015f;
    public float elevationAmplitude = 2f;
    public float baseHeight = 0f;

    [Header("Spawn Pattern")]
    public int minNormalBeforeSpecial = 10;
    public int maxNormalBeforeSpecial = 20;

    private Queue<GameObject> _roads = new Queue<GameObject>();
    private Transform _lastEndPoint;
    private int _currentIndex;
    private int _patternIndex;
    private int _normalCount;
    private int _nextSpecialAt;

    void Start()
    {
        _nextSpecialAt = Random.Range(minNormalBeforeSpecial, maxNormalBeforeSpecial + 1);

        // Spawn first road at origin
        GameObject first = Instantiate(road1Prefab, transform);
        first.transform.localPosition = Vector3.zero;
        first.transform.localRotation = Quaternion.identity;
        
        _lastEndPoint = first.transform.Find("EndPoint");
        _roads.Enqueue(first);
        _currentIndex++;

        // Spawn initial visible segments
        for (int i = 1; i < visibleSegments; i++)
        {
            SpawnSegment(_currentIndex++);
        }
    }

    void Update()
    {
        // Continuously cleanup old roads and spawn new ones
        while (_roads.Count > 0)
        {
            GameObject first = _roads.Peek();
            if (first != null && first.transform.position.z < deleteDistance)
            {
                Destroy(_roads.Dequeue());
                SpawnSegment(_currentIndex++);
            }
            else
            {
                break;
            }
        }
    }

    // Spawns a single road segment with proper alignment and elevation
    void SpawnSegment(int index)
    {
        GameObject prefab = GetNextPrefab();
        GameObject road = Instantiate(prefab, transform);

        // Find alignment points
        Transform startPoint = road.transform.Find("StartPoint");
        Transform endPoint = road.transform.Find("EndPoint");

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("Road prefab missing StartPoint or EndPoint!");
            Destroy(road);
            return;
        }

        // Calculate position and rotation with elevation
        Vector3 targetPos = GetPositionAtIndex(index);
        Quaternion targetRot = GetRotationAtIndex(index);

        if (_lastEndPoint != null)
        {
            // Set rotation first
            road.transform.rotation = targetRot;
            
            // Align StartPoint with previous road's EndPoint
            Vector3 offset = startPoint.position - road.transform.position;
            road.transform.position = _lastEndPoint.position - offset;
            
            // Apply overlap offset for seamless connection
            road.transform.position += road.transform.forward * overlapOffset;
        }
        else
        {
            // First road
            road.transform.position = targetPos;
            road.transform.rotation = targetRot;
        }

        _lastEndPoint = endPoint;
        _roads.Enqueue(road);
    }

    // Calculates position at given index with elevation
    Vector3 GetPositionAtIndex(int index)
    {
        float z = index * (segmentLength + overlapOffset);
        float x = 0f; // Straight roads only (no curves)
        float y = Mathf.Sin(z * elevationFrequency) * elevationAmplitude + baseHeight;

        return new Vector3(x, y, z);
    }

    // Calculates rotation at given index based on elevation slope
    Quaternion GetRotationAtIndex(int index)
    {
        Vector3 current = GetPositionAtIndex(index);
        Vector3 next = GetPositionAtIndex(index + 1);
        Vector3 direction = (next - current).normalized;

        // LookRotation automatically calculates pitch (X rotation) for elevation
        return Quaternion.LookRotation(direction, Vector3.up);
    }

    // Returns next prefab based on pattern: R1-R1-R2-R1-R1-R2
    // Special roads spawn every 10-20 normal roads
    GameObject GetNextPrefab()
    {
        // Spawn special road if count reached
        if (specialRoadPrefabs.Count > 0 && _normalCount >= _nextSpecialAt)
        {
            _normalCount = 0;
            _nextSpecialAt = Random.Range(minNormalBeforeSpecial, maxNormalBeforeSpecial + 1);
            return specialRoadPrefabs[Random.Range(0, specialRoadPrefabs.Count)];
        }

        // Spawn normal road following pattern
        _normalCount++;

        GameObject prefab = (_patternIndex == 0 || _patternIndex == 1 || _patternIndex == 3 || _patternIndex == 4)
            ? road1Prefab
            : road2Prefab;

        _patternIndex = (_patternIndex + 1) % 6;
        return prefab;
    }

    // Public API: Get position at specific Z coordinate
    public Vector3 GetPositionAtZ(float z)
    {
        float x = 0f;
        float y = Mathf.Sin(z * elevationFrequency) * elevationAmplitude + baseHeight;
        return new Vector3(x, y, z);
    }

    // Public API: Get direction at specific Z coordinate
    public Vector3 GetDirectionAtZ(float z)
    {
        Vector3 current = GetPositionAtZ(z);
        Vector3 next = GetPositionAtZ(z + 0.5f);
        return (next - current).normalized;
    }

    // Public API: Get elevation (Y) at specific Z coordinate
    public float GetElevationAtZ(float z)
    {
        return Mathf.Sin(z * elevationFrequency) * elevationAmplitude + baseHeight;
    }
}

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
    public float deleteDistance = -50f;
    public float menuDeleteDistance = -200f; // Delete distance before game starts
    public float menuStartZ = -200f; // Starting Z position for menu

    [Header("Elevation (Up/Down Slopes)")]
    public float elevationFrequency = 0.015f;
    public float elevationAmplitude = 2f;
    public float baseHeight = 0f;
    [Tooltip("Adds randomness to elevation using Perlin noise")]
    public float perlinScale = 0.02f; // Scale for Perlin noise
    public float perlinInfluence = 0.5f; // How much Perlin affects elevation (0-1)

    [Header("Spawn Pattern")]
    public int minNormalBeforeSpecial = 10;
    public int maxNormalBeforeSpecial = 20;

    private Queue<GameObject> _roads = new Queue<GameObject>();
    private Transform _lastEndPoint;
    private int _currentIndex;
    private int _patternIndex;
    private int _normalCount;
    private int _nextSpecialAt;
    private bool _elevationEnabled = false; // Disable elevation until game starts
    private float _perlinOffset; // Random offset for Perlin noise

    void Start()
    {
        _nextSpecialAt = Random.Range(minNormalBeforeSpecial, maxNormalBeforeSpecial + 1);
        
        // Random offset for Perlin noise to make each game different
        _perlinOffset = Random.Range(0f, 1000f);

        // Calculate starting index based on menu start position
        // This ensures roads spawn from menuStartZ
        _currentIndex = Mathf.RoundToInt(menuStartZ / (segmentLength + overlapOffset));

        // Spawn initial visible segments
        for (int i = 0; i < visibleSegments; i++)
        {
            SpawnSegment(_currentIndex++);
        }
    }

    void Update()
    {
        // Enable elevation when game starts
        if (!_elevationEnabled && GameLogicController.Instance != null && GameLogicController.Instance.isGameStarted)
        {
            _elevationEnabled = true;
        }

        // Determine delete distance based on game state
        float currentDeleteDistance = (GameLogicController.Instance != null && GameLogicController.Instance.isGameStarted)
            ? deleteDistance
            : menuDeleteDistance;

        // Continuously cleanup old roads and spawn new ones
        while (_roads.Count > 0)
        {
            GameObject first = _roads.Peek();
            if (first != null && first.transform.position.z < currentDeleteDistance)
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

        // Calculate position and rotation with elevation (only if enabled)
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

    // Calculates position at given index with elevation (only if enabled)
    Vector3 GetPositionAtIndex(int index)
    {
        float z = index * (segmentLength + overlapOffset);
        float x = 0f; // Straight roads only (no curves)
        
        // Only apply elevation if game started
        float y = baseHeight;
        if (_elevationEnabled)
        {
            // Combine sine wave with Perlin noise for more varied elevation
            float sineWave = Mathf.Sin(z * elevationFrequency) * elevationAmplitude;
            
            // Perlin noise for randomness (sample at different position for variation)
            float perlin = Mathf.PerlinNoise((z + _perlinOffset) * perlinScale, _perlinOffset);
            perlin = (perlin - 0.5f) * 2f; // Remap from 0-1 to -1 to 1
            float perlinWave = perlin * elevationAmplitude * perlinInfluence;
            
            // Combine both for varied elevation
            y = baseHeight + sineWave * (1f - perlinInfluence) + perlinWave;
        }

        return new Vector3(x, y, z);
    }

    // Calculates rotation at given index based on elevation slope
    Quaternion GetRotationAtIndex(int index)
    {
        // If elevation disabled, return flat rotation
        if (!_elevationEnabled)
            return Quaternion.identity;

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
        float y = baseHeight;
        
        if (_elevationEnabled)
        {
            // Combine sine wave with Perlin noise
            float sineWave = Mathf.Sin(z * elevationFrequency) * elevationAmplitude;
            
            float perlin = Mathf.PerlinNoise((z + _perlinOffset) * perlinScale, _perlinOffset);
            perlin = (perlin - 0.5f) * 2f;
            float perlinWave = perlin * elevationAmplitude * perlinInfluence;
            
            y = baseHeight + sineWave * (1f - perlinInfluence) + perlinWave;
        }
        
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
        if (!_elevationEnabled)
            return baseHeight;
            
        // Combine sine wave with Perlin noise
        float sineWave = Mathf.Sin(z * elevationFrequency) * elevationAmplitude;
        
        float perlin = Mathf.PerlinNoise((z + _perlinOffset) * perlinScale, _perlinOffset);
        perlin = (perlin - 0.5f) * 2f;
        float perlinWave = perlin * elevationAmplitude * perlinInfluence;
        
        return baseHeight + sineWave * (1f - perlinInfluence) + perlinWave;
    }
}

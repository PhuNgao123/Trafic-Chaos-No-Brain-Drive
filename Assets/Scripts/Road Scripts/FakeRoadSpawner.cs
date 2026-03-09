using UnityEngine;
using System.Collections.Generic;

// Spawns fake road segments in the distance with dynamic curves and elevation
// These roads are visual only (no collision) to create illusion of winding roads ahead
// Actual playable roads remain straight
public class FakeRoadSpawner : MonoBehaviour
{
    [Header("References")]
    public RoadSpawner mainRoadSpawner;

    [Header("Prefabs")]
    public GameObject road1Prefab;
    public GameObject road2Prefab;

    [Header("Fake Road Settings")]
    public int fakeSegmentCount = 15;
    public float segmentLength = 15f;
    public float startOffsetDistance = 50f; // Distance from main road end

    [Header("Dynamic Curve (Fake Roads Only)")]
    public float curveChangeInterval = 3f; // Change curve direction every X seconds
    public float curveTransitionSpeed = 2f;
    public float minCurveAmplitude = 3f;
    public float maxCurveAmplitude = 8f;

    [Header("Dynamic Elevation (Fake Roads Only)")]
    public float elevationChangeInterval = 4f; // Change elevation every X seconds
    public float elevationTransitionSpeed = 1.5f;
    public float minElevationAmplitude = 2f;
    public float maxElevationAmplitude = 6f;

    private List<GameObject> _fakeRoads = new List<GameObject>();
    private int _patternIndex;
    
    // Dynamic curve
    private float _currentCurveTarget;
    private float _currentCurveValue;
    private float _nextCurveChangeTime;

    // Dynamic elevation
    private float _currentElevationTarget;
    private float _currentElevationValue;
    private float _nextElevationChangeTime;

    void Start()
    {
        if (mainRoadSpawner == null)
            mainRoadSpawner = FindFirstObjectByType<RoadSpawner>();

        SpawnFakeRoads();
        
        // Initialize random targets
        RandomizeCurveTarget();
        RandomizeElevationTarget();
    }

    void Update()
    {
        // Change curve target periodically
        if (Time.time >= _nextCurveChangeTime)
        {
            RandomizeCurveTarget();
            _nextCurveChangeTime = Time.time + curveChangeInterval;
        }

        // Change elevation target periodically
        if (Time.time >= _nextElevationChangeTime)
        {
            RandomizeElevationTarget();
            _nextElevationChangeTime = Time.time + elevationChangeInterval;
        }

        // Smooth transition to targets
        _currentCurveValue = Mathf.Lerp(_currentCurveValue, _currentCurveTarget, curveTransitionSpeed * Time.deltaTime);
        _currentElevationValue = Mathf.Lerp(_currentElevationValue, _currentElevationTarget, elevationTransitionSpeed * Time.deltaTime);

        // Update fake road positions and rotations
        UpdateFakeRoadCurve();
    }

    // Randomizes curve target: left (-), straight (0), or right (+)
    void RandomizeCurveTarget()
    {
        float[] targets = { 
            -maxCurveAmplitude, 
            -minCurveAmplitude, 
            0f, 
            minCurveAmplitude, 
            maxCurveAmplitude 
        };
        
        _currentCurveTarget = targets[Random.Range(0, targets.Length)];
    }

    // Randomizes elevation target: down (-), flat (0), or up (+)
    void RandomizeElevationTarget()
    {
        float[] targets = { 
            -maxElevationAmplitude, 
            -minElevationAmplitude, 
            0f, 
            minElevationAmplitude, 
            maxElevationAmplitude 
        };
        
        _currentElevationTarget = targets[Random.Range(0, targets.Length)];
    }

    // Spawns all fake road segments once
    // Segments are parented to FakeRoadSpawner (not RoadContainer) so they don't move
    void SpawnFakeRoads()
    {
        // Clear existing fake roads
        foreach (GameObject road in _fakeRoads)
        {
            if (road != null)
                Destroy(road);
        }
        _fakeRoads.Clear();

        // Spawn segments
        for (int i = 0; i < fakeSegmentCount; i++)
        {
            GameObject prefab = GetNextPrefab();
            GameObject fakeRoad = Instantiate(prefab, transform);

            // Initial position (will be updated in UpdateFakeRoadCurve)
            float z = startOffsetDistance + i * segmentLength;
            fakeRoad.transform.localPosition = new Vector3(0f, 0f, z);
            fakeRoad.transform.localRotation = Quaternion.identity;

            // Disable colliders - visual only
            Collider[] colliders = fakeRoad.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = false;
            }

            fakeRoad.name = $"FakeRoad_{i}";
            _fakeRoads.Add(fakeRoad);
        }
    }

    // Updates fake road positions to create smooth curves and elevation changes
    // Roads gradually bend from straight (at start) to target curve/elevation (at end)
    void UpdateFakeRoadCurve()
    {
        if (_fakeRoads.Count == 0) return;

        // Get real road end elevation to connect seamlessly
        float startElevation = GetRealRoadEndElevation();

        // Update each segment
        for (int i = 0; i < _fakeRoads.Count; i++)
        {
            if (_fakeRoads[i] == null) continue;

            // Calculate progress (0 = near, 1 = far)
            float progress = Mathf.Clamp01((float)i / (fakeSegmentCount - 1));
            float smoothProgress = Mathf.SmoothStep(0f, 1f, progress);

            // Calculate X (curve) - gradually bend to target
            float x = Mathf.Lerp(0f, _currentCurveValue, smoothProgress);
            
            // Calculate Y (elevation) - start from real road height, bend to target
            float y = Mathf.Lerp(startElevation, startElevation + _currentElevationValue, smoothProgress);

            // Z position - fixed
            float z = startOffsetDistance + i * segmentLength;

            // Calculate rotation based on direction to next segment
            Quaternion rot = GetSegmentRotation(i, x, y, z, startElevation);

            // Apply position and rotation
            _fakeRoads[i].transform.localPosition = new Vector3(x, y, z);
            _fakeRoads[i].transform.localRotation = rot;
        }
    }

    // Gets elevation at the end of real road to connect fake roads seamlessly
    float GetRealRoadEndElevation()
    {
        if (mainRoadSpawner == null) return 0f;

        float endZ = mainRoadSpawner.visibleSegments * mainRoadSpawner.segmentLength;
        return mainRoadSpawner.GetElevationAtZ(endZ);
    }

    // Calculates rotation for segment to face next segment
    Quaternion GetSegmentRotation(int index, float currentX, float currentY, float currentZ, float startElevation)
    {
        // Last segment faces forward
        if (index >= fakeSegmentCount - 1)
            return Quaternion.identity;

        // Calculate next segment position
        float nextProgress = Mathf.Clamp01((float)(index + 1) / (fakeSegmentCount - 1));
        float nextSmoothProgress = Mathf.SmoothStep(0f, 1f, nextProgress);
        
        float nextX = Mathf.Lerp(0f, _currentCurveValue, nextSmoothProgress);
        float nextY = Mathf.Lerp(startElevation, startElevation + _currentElevationValue, nextSmoothProgress);
        float nextZ = startOffsetDistance + (index + 1) * segmentLength;

        // Calculate direction
        Vector3 current = new Vector3(currentX, currentY, currentZ);
        Vector3 next = new Vector3(nextX, nextY, nextZ);
        Vector3 direction = (next - current).normalized;

        if (direction == Vector3.zero)
            return Quaternion.identity;

        return Quaternion.LookRotation(direction, Vector3.up);
    }

    // Returns next prefab following pattern: R1-R1-R2
    GameObject GetNextPrefab()
    {
        GameObject prefab = (_patternIndex == 0 || _patternIndex == 1) ? road1Prefab : road2Prefab;
        _patternIndex = (_patternIndex + 1) % 3;
        return prefab;
    }

    // Public method to refresh fake roads (e.g., when settings change)
    public void RefreshFakeRoads()
    {
        SpawnFakeRoads();
    }
}

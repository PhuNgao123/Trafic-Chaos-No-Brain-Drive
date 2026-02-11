using UnityEngine;
using System.Collections.Generic;

public class RoadSpawner : MonoBehaviour
{
    [Header("=== PREFABS ===")]
    public GameObject road1Prefab;
    public GameObject road2Prefab;
    public List<GameObject> specialRoadPrefabs;

    [Header("=== ROAD SETTINGS ===")]
    public float segmentLength = 15f;
    public float overlapOffset = 0.5f;
    public int visibleSegments = 25;
    public float deleteDistance = -30f;

    [Header("=== CURVE (TRÁI PHẢI) ===")]
    public float curveFrequency = 0.02f;
    public float curveAmplitude = 4f;

    [Header("=== ELEVATION (LÊN XUỐNG) ===")]
    public float elevationFrequency = 0.015f;
    public float elevationAmplitude = 2f;
    public float baseHeight = 0f;

    [Header("=== SPAWN PATTERN ===")]
    public int minNormalBeforeSpecial = 10;
    public int maxNormalBeforeSpecial = 20;

    Queue<GameObject> roads = new Queue<GameObject>();
    Transform lastEndPoint = null;
    int currentIndex = 0;
    int patternIndex = 0;
    int normalCount = 0;
    int nextSpecialAt;

    void Start()
    {
        nextSpecialAt = Random.Range(minNormalBeforeSpecial, maxNormalBeforeSpecial + 1);

        // Spawn road đầu tiên tại origin
        GameObject first = Instantiate(road1Prefab, transform);
        first.transform.localPosition = Vector3.zero;
        first.transform.localRotation = Quaternion.identity;
        
        lastEndPoint = first.transform.Find("EndPoint");
        roads.Enqueue(first);
        currentIndex++;

        // Spawn các road tiếp theo
        for (int i = 1; i < visibleSegments; i++)
        {
            SpawnSegment(currentIndex++);
        }
    }

    void Update()
    {
        // Cleanup và spawn liên tục (check world position vì RoadContainer đang di chuyển)
        while (roads.Count > 0)
        {
            GameObject first = roads.Peek();
            if (first != null && first.transform.position.z < deleteDistance)
            {
                Destroy(roads.Dequeue());
                SpawnSegment(currentIndex++);
            }
            else
            {
                break;
            }
        }
    }

    void SpawnSegment(int index)
    {
        GameObject prefab = GetNextPrefab();
        GameObject road = Instantiate(prefab, transform);

        // Tính rotation dựa trên curve
        Quaternion targetRot = GetRotationAtIndex(index);

        // Tìm StartPoint và EndPoint
        Transform startPoint = road.transform.Find("StartPoint");
        Transform endPoint = road.transform.Find("EndPoint");

        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("Road prefab thiếu StartPoint hoặc EndPoint!");
            Destroy(road);
            return;
        }

        if (lastEndPoint != null)
        {
            // Đặt rotation trước
            road.transform.rotation = targetRot;
            
            // Align StartPoint với EndPoint của road trước
            Vector3 offset = startPoint.position - road.transform.position;
            road.transform.position = lastEndPoint.position - offset;
            
            // Apply overlap offset (đẩy road vào trong một chút)
            road.transform.position += road.transform.forward * overlapOffset;
        }
        else
        {
            // Road đầu tiên
            road.transform.position = Vector3.zero;
            road.transform.rotation = targetRot;
        }

        lastEndPoint = endPoint;
        roads.Enqueue(road);
    }

    Vector3 GetPositionAtIndex(int index)
    {
        float z = index * (segmentLength + overlapOffset);
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
        // Spawn special road mỗi 10-20 roads
        if (specialRoadPrefabs.Count > 0 && normalCount >= nextSpecialAt)
        {
            normalCount = 0;
            nextSpecialAt = Random.Range(minNormalBeforeSpecial, maxNormalBeforeSpecial + 1);
            
            // Random 1 special road từ list
            return specialRoadPrefabs[Random.Range(0, specialRoadPrefabs.Count)];
        }

        // Spawn normal road theo pattern: R1-R1-R2-R1-R1-R2
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

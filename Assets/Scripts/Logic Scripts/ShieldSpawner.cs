using UnityEngine;

/// <summary>
/// Tự động spawn shield power-ups trong game theo interval.
/// Có thể spawn tại các vị trí định sẵn hoặc random trong khu vực.
/// </summary>
public class ShieldSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField]
    [Tooltip("Shield prefab để spawn")]
    private GameObject shieldPrefab;

    [SerializeField]
    [Tooltip("Thời gian giữa mỗi lần spawn (giây)")]
    private float spawnInterval = 15f;

    [SerializeField]
    [Tooltip("Số lượng shield tối đa có thể tồn tại cùng lúc")]
    private int maxActiveShields = 3;

    [Header("Spawn Locations")]
    [SerializeField]
    [Tooltip("Danh sách các điểm spawn cố định")]
    private Transform[] spawnPoints;

    [SerializeField]
    [Tooltip("Nếu true, spawn random trong bounds thay vì dùng spawn points")]
    private bool useRandomSpawn = false;

    [SerializeField]
    [Tooltip("Khu vực spawn random (chỉ dùng khi useRandomSpawn = true)")]
    private Vector3 spawnAreaMin = new Vector3(-10, 0, 0);

    [SerializeField]
    [Tooltip("Khu vực spawn random (chỉ dùng khi useRandomSpawn = true)")]
    private Vector3 spawnAreaMax = new Vector3(10, 0, 50);

    [Header("Optional")]
    [SerializeField]
    [Tooltip("Delay trước lần spawn đầu tiên (giây)")]
    private float initialDelay = 5f;

    [SerializeField]
    [Tooltip("Nếu true, spawn ngay khi game start")]
    private bool spawnOnStart = false;

    private float timer;
    private int currentActiveShields = 0;
    private bool hasStarted = false;

    void Start()
    {
        if (shieldPrefab == null)
        {
            Debug.LogError("[ShieldSpawner] Shield prefab not assigned!");
            enabled = false;
            return;
        }

        if (!useRandomSpawn && (spawnPoints == null || spawnPoints.Length == 0))
        {
            Debug.LogError("[ShieldSpawner] No spawn points assigned!");
            enabled = false;
            return;
        }

        timer = spawnOnStart ? spawnInterval : initialDelay;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            if (currentActiveShields < maxActiveShields)
            {
                SpawnShield();
            }
            timer = 0f;
        }
    }

    /// <summary>
    /// Spawn một shield tại vị trí ngẫu nhiên
    /// </summary>
    private void SpawnShield()
    {
        Vector3 spawnPosition = GetSpawnPosition();
        GameObject shield = Instantiate(shieldPrefab, spawnPosition, Quaternion.identity);

        // Track số lượng shields
        currentActiveShields++;

        // Subscribe để giảm count khi shield bị destroy
        ShieldPowerUp powerUp = shield.GetComponent<ShieldPowerUp>();
        if (powerUp != null)
        {
            // Thêm callback khi shield bị destroy
            shield.AddComponent<ShieldDestroyTracker>().Initialize(this);
        }

        Debug.Log($"[ShieldSpawner] Spawned shield at {spawnPosition}. Active shields: {currentActiveShields}");
    }

    /// <summary>
    /// Lấy vị trí spawn (random hoặc từ spawn points)
    /// </summary>
    private Vector3 GetSpawnPosition()
    {
        if (useRandomSpawn)
        {
            // Random trong bounds
            float x = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
            float y = Random.Range(spawnAreaMin.y, spawnAreaMax.y);
            float z = Random.Range(spawnAreaMin.z, spawnAreaMax.z);
            return new Vector3(x, y, z);
        }
        else
        {
            // Chọn random từ spawn points
            int randomIndex = Random.Range(0, spawnPoints.Length);
            return spawnPoints[randomIndex].position;
        }
    }

    /// <summary>
    /// Được gọi khi một shield bị destroy
    /// </summary>
    public void OnShieldDestroyed()
    {
        currentActiveShields--;
        if (currentActiveShields < 0)
        {
            currentActiveShields = 0;
        }
    }

    /// <summary>
    /// Spawn shield ngay lập tức (có thể gọi từ code khác)
    /// </summary>
    public void ForceSpawn()
    {
        if (currentActiveShields < maxActiveShields)
        {
            SpawnShield();
        }
    }

    // Vẽ gizmos để visualize spawn area trong editor
    void OnDrawGizmosSelected()
    {
        if (useRandomSpawn)
        {
            Gizmos.color = Color.cyan;
            Vector3 center = (spawnAreaMin + spawnAreaMax) / 2f;
            Vector3 size = spawnAreaMax - spawnAreaMin;
            Gizmos.DrawWireCube(center, size);
        }
        else if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform point in spawnPoints)
            {
                if (point != null)
                {
                    Gizmos.DrawWireSphere(point.position, 0.5f);
                }
            }
        }
    }
}

/// <summary>
/// Helper component để track khi shield bị destroy
/// </summary>
public class ShieldDestroyTracker : MonoBehaviour
{
    private ShieldSpawner spawner;

    public void Initialize(ShieldSpawner spawner)
    {
        this.spawner = spawner;
    }

    void OnDestroy()
    {
        if (spawner != null)
        {
            spawner.OnShieldDestroyed();
        }
    }
}

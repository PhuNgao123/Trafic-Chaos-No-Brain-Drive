using UnityEngine;
using System.Collections;

// Pursuit police car system - fully independent movement, no VehicleMove dependency.
// Attach to a persistent GameObject in the scene (e.g. Enemy Controller).
public class PursuitPoliceController : MonoBehaviour
{
    public static PursuitPoliceController Instance { get; private set; }

    [Header("Prefab")]
    public GameObject pursuitPolicePrefab;

    [Header("Spawn Settings")]
    public float spawnZ = -15f;      // Spawn Z behind player
    public float followZ = -5f;      // Z to hold during follow phase
    public float despawnZ = -20f;    // Z to despawn when retreating

    [Header("Movement")]
    public float approachSpeed = 12f;   // Phase 1: move from spawnZ to followZ
    public float xFollowSpeed = 3f;     // Phase 2: lerp X toward player
    public float retreatSpeed = 8f;     // Phase 3 (no hit): move back to despawnZ
    public float ramSpeed = 25f;        // Phase 3 (hit): move forward toward player

    [Header("Timing")]
    public float spawnDelay = 2f;       // Delay before spawning after player hits police
    public float pursuitDuration = 5f;  // Seconds to follow before giving up
    public float respawnCooldown = 5f;

    public static bool isAlive = false;

    private GameObject _car;
    private PursuitPoliceCollision _collision;
    private Coroutine _routine;
    private bool _onCooldown = false;
    private float _cooldownTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (_onCooldown)
        {
            _cooldownTimer -= Time.deltaTime;
            if (_cooldownTimer <= 0f) _onCooldown = false;
        }
    }

    // Called when player hits any regular police car
    public void OnPlayerHitPolice()
    {
        if (isAlive || _onCooldown) return;
        if (GameLogicController.Instance == null || GameLogicController.Instance.isGameOver) return;
        if (pursuitPolicePrefab == null) return;
        _routine = StartCoroutine(Run());
    }

    // Called when player hits police WHILE pursuit car is already alive
    public void NotifyPlayerHitPoliceDuringPursuit()
    {
        if (_collision != null)
            _collision.playerHitPoliceDuringPursuit = true;
    }

    // Returns true if the given GameObject is the active pursuit car
    public bool IsPursuitCar(GameObject obj) => _car != null && obj == _car;

    IEnumerator Run()
    {
        isAlive = true;

        // Wait before spawning so the police car player just hit has time to clear
        yield return new WaitForSeconds(spawnDelay);

        if (IsGameOver()) { EndWithCooldown(); yield break; }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) { EndWithCooldown(); yield break; }

        // --- Spawn ---
        _car = Instantiate(pursuitPolicePrefab,
            new Vector3(player.transform.position.x, 0f, spawnZ),
            Quaternion.identity);

        // Disable all AI scripts so only we control movement
        foreach (var vm in _car.GetComponents<VehicleMove>()) vm.enabled = false;

        // Freeze rigidbody - we move via Transform
        Rigidbody rb = _car.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.linearVelocity = Vector3.zero; }

        // Mark as pursuit car so PoliceVehicle.OnCollisionEnter is skipped
        PoliceVehicle pv = _car.GetComponent<PoliceVehicle>();
        if (pv != null) pv.isPursuitCar = true;

        // Attach collision detector
        _collision = _car.AddComponent<PursuitPoliceCollision>();
        _collision.controller = this;

        // --- Phase 1: Approach ---
        Debug.Log("[Pursuit] Phase 1: Approaching...");
        while (_car != null && _car.transform.position.z < followZ)
        {
            if (IsGameOver()) { Despawn(); yield break; }
            _car.transform.position += Vector3.forward * approachSpeed * Time.deltaTime;
            yield return null;
        }
        if (_car == null) { EndWithCooldown(); yield break; }

        // --- Phase 2: Follow X for pursuitDuration seconds ---
        Debug.Log("[Pursuit] Phase 2: Following player...");
        float timer = 0f;
        while (timer < pursuitDuration)
        {
            if (_car == null) { EndWithCooldown(); yield break; }
            if (IsGameOver()) { Despawn(); yield break; }
            if (_collision != null && _collision.playerHitPoliceDuringPursuit) break;

            Vector3 pos = _car.transform.position;
            pos.x = Mathf.Lerp(pos.x, player.transform.position.x, xFollowSpeed * Time.deltaTime);
            pos.z = followZ; // hold Z
            _car.transform.position = pos;

            timer += Time.deltaTime;
            yield return null;
        }
        if (_car == null) { EndWithCooldown(); yield break; }

        // --- Phase 3 ---
        if (_collision != null && _collision.playerHitPoliceDuringPursuit)
        {
            // Ram: move straight forward (Z+) toward player
            Debug.Log("[Pursuit] Phase 3: RAMMING!");

            // Unfreeze rigidbody so crash physics work
            if (rb != null) rb.isKinematic = false;

            float elapsed = 0f;
            while (_car != null && elapsed < 4f)
            {
                if (IsGameOver()) { Despawn(); yield break; }
                _car.transform.position += Vector3.forward * ramSpeed * Time.deltaTime;
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // Retreat: move backward off screen
            Debug.Log("[Pursuit] Phase 3: Retreating...");
            while (_car != null && _car.transform.position.z > despawnZ)
            {
                if (IsGameOver()) { Despawn(); yield break; }
                _car.transform.position += Vector3.back * retreatSpeed * Time.deltaTime;
                yield return null;
            }
        }

        Despawn();
    }

    void StopRoutine()
    {
        if (_routine != null) { StopCoroutine(_routine); _routine = null; }
    }

    void Despawn()
    {
        if (_car != null) { Destroy(_car); _car = null; }
        _collision = null;
        EndWithCooldown();
    }

    void EndWithCooldown()
    {
        isAlive = false;
        _onCooldown = true;
        _cooldownTimer = respawnCooldown;
    }

    bool IsGameOver() =>
        GameLogicController.Instance != null && GameLogicController.Instance.isGameOver;

    void OnDestroy() { isAlive = false; }
}

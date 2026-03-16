using UnityEngine;
using System.Collections.Generic;

// Detects near miss events when player passes close to vehicles
// Attach to a child object with SphereCollider (isTrigger = true)
public class NearMissDetector : MonoBehaviour
{
    [Header("Visual Feedback")]
    public GameObject nearMissEffectPrefab;
    public AudioClip nearMissSound;

    [Header("Debug")]
    public bool showDebugLogs = false;

    private HashSet<GameObject> _triggeredVehicles = new HashSet<GameObject>();
    private AudioSource _audioSource;
    private int _simultaneousNearMiss = 0;
    private int _simultaneousPoliceNearMiss = 0; // Track police near misses separately
    private float _simultaneousWindow = 0.2f; // Time window for simultaneous detection
    private float _lastNearMissTime = 0f;
    private int _totalPoliceNearMiss = 0; // Total police near misses in game

    public static NearMissDetector Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SetupAudioSource();
    }

    void Update()
    {
        // Check if simultaneous near miss window expired
        if (_simultaneousNearMiss > 0 && Time.time - _lastNearMissTime > _simultaneousWindow)
        {
            ProcessSimultaneousNearMiss();
        }
    }

    // Setup audio source for sound effects
    void SetupAudioSource()
    {
        _audioSource = GetComponent<AudioSource>();
        
        if (_audioSource == null)
        {
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 0f; // 2D sound
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Only detect vehicles and police
        if (!other.CompareTag("Vehicle") && !other.CompareTag("Police"))
            return;

        GameObject vehicle = other.gameObject;

        // Only count each vehicle once
        if (_triggeredVehicles.Contains(vehicle))
            return;

        _triggeredVehicles.Add(vehicle);

        bool isPolice = other.CompareTag("Police");
        
        // Count police near misses
        if (isPolice)
        {
            _totalPoliceNearMiss++;
            Debug.Log($"[NearMiss] Police near miss! Total: {_totalPoliceNearMiss}");
        }

        // Trigger camera shake
        CameraFunctions cam = FindFirstObjectByType<CameraFunctions>();
        if (cam != null)
            cam.TriggerNearMissShake();

        // Check if within simultaneous window
        if (Time.time - _lastNearMissTime <= _simultaneousWindow)
        {
            _simultaneousNearMiss++;
            if (isPolice)
                _simultaneousPoliceNearMiss++;
        }
        else
        {
            // Process previous simultaneous near miss if any
            if (_simultaneousNearMiss > 0)
            {
                ProcessSimultaneousNearMiss();
            }

            // Start new near miss
            _simultaneousNearMiss = 1;
            _simultaneousPoliceNearMiss = isPolice ? 1 : 0;
        }

        _lastNearMissTime = Time.time;

        // Visual and audio feedback
        PlayNearMissEffect(vehicle.transform.position);
    }

    void OnTriggerExit(Collider other)
    {
        // Remove vehicle/police from triggered set when it exits
        if (other.CompareTag("Vehicle") || other.CompareTag("Police"))
        {
            _triggeredVehicles.Remove(other.gameObject);
        }
    }

    // Process simultaneous near miss and add combo
    void ProcessSimultaneousNearMiss()
    {
        if (ScoreController.Instance == null)
            return;

        // Calculate combo multiplier (Police gives x2)
        int totalCombo = _simultaneousNearMiss + _simultaneousPoliceNearMiss; // Police counted twice
        
        // Add combo based on number of simultaneous near misses
        ScoreController.Instance.AddCombo(totalCombo);

        Debug.Log($"[NearMiss] Processed: {_simultaneousNearMiss} total, {_simultaneousPoliceNearMiss} police, final combo: {totalCombo}");

        // Perfect overtake bonus (2+ vehicles at once)
        if (_simultaneousNearMiss >= 2)
        {
            float bonus = ScoreController.Instance.perfectOvertakeBonus * _simultaneousNearMiss;
            ScoreController.Instance.AddBonusScore(bonus);
        }

        // Reset counters
        _simultaneousNearMiss = 0;
        _simultaneousPoliceNearMiss = 0;
    }

    // Get total police near misses for penalty calculation
    public int GetTotalPoliceNearMiss()
    {
        return _totalPoliceNearMiss;
    }

    // Reset police near miss count (called when game starts)
    public void ResetPoliceNearMiss()
    {
        _totalPoliceNearMiss = 0;
        Debug.Log("[NearMiss] Reset police near miss count");
    }

    // Play visual and audio feedback
    void PlayNearMissEffect(Vector3 position)
    {
        // Spawn particle effect
        if (nearMissEffectPrefab != null)
        {
            GameObject effect = Instantiate(nearMissEffectPrefab, position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // Play sound
        if (nearMissSound != null && _audioSource != null)
        {
            _audioSource.PlayOneShot(nearMissSound);
        }
    }

    // Visualize trigger radius in editor
    void OnDrawGizmosSelected()
    {
        SphereCollider col = GetComponent<SphereCollider>();
        if (col != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, col.radius * transform.lossyScale.x);
        }
    }
}

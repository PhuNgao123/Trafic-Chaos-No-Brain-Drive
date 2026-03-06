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
    private float _simultaneousWindow = 0.2f; // Time window for simultaneous detection
    private float _lastNearMissTime = 0f;

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
        // Only detect vehicles
        if (!other.CompareTag("Vehicle"))
            return;

        GameObject vehicle = other.gameObject;

        // Only count each vehicle once
        if (_triggeredVehicles.Contains(vehicle))
            return;

        _triggeredVehicles.Add(vehicle);

        // Trigger camera shake
        CameraFunctions cam = FindFirstObjectByType<CameraFunctions>();
        if (cam != null)
            cam.TriggerNearMissShake();

        // Check if within simultaneous window
        if (Time.time - _lastNearMissTime <= _simultaneousWindow)
        {
            _simultaneousNearMiss++;
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
        }

        _lastNearMissTime = Time.time;

        // Visual and audio feedback
        PlayNearMissEffect(vehicle.transform.position);
    }

    void OnTriggerExit(Collider other)
    {
        // Remove vehicle from triggered set when it exits
        if (other.CompareTag("Vehicle"))
        {
            _triggeredVehicles.Remove(other.gameObject);
        }
    }

    // Process simultaneous near miss and add combo
    void ProcessSimultaneousNearMiss()
    {
        if (ScoreController.Instance == null)
            return;

        // Add combo based on number of simultaneous near misses
        ScoreController.Instance.AddCombo(_simultaneousNearMiss);

        // Perfect overtake bonus (2+ vehicles at once)
        if (_simultaneousNearMiss >= 2)
        {
            float bonus = ScoreController.Instance.perfectOvertakeBonus * _simultaneousNearMiss;
            ScoreController.Instance.AddBonusScore(bonus);
        }

        // Reset counter
        _simultaneousNearMiss = 0;
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

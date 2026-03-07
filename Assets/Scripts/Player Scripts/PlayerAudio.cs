using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    private AudioSource engineAudioSource;
    public AudioClip engineStartClip;
    public AudioClip engineRevClip;

    [Header("Engine Pitch Settings")]
    public float minPitch = 0.8f;
    public float maxPitch = 2.0f;
    
    private PlayerPhysics playerPhysics;
    private bool hasStartedAccelerating = false;

    void Start()
    {
        engineAudioSource = GetComponent<AudioSource>();
        playerPhysics = GetComponent<PlayerPhysics>();

        StartCoroutine(StartEngineSequence());
    }

    private IEnumerator StartEngineSequence()
    {
        if (engineStartClip != null)
        {
            engineAudioSource.clip = engineStartClip;
            engineAudioSource.loop = false; 
            engineAudioSource.spatialBlend = 0f; 
            engineAudioSource.Play();
            
            // Wait for the exact length of the start clip
            yield return new WaitForSeconds(engineStartClip.length);
        }

        hasStartedAccelerating = true;

        if (engineRevClip != null)
        {
            engineAudioSource.clip = engineRevClip;
            engineAudioSource.loop = true; // Rev sound continuously loops
            engineAudioSource.Play();
        }
    }

    void Update()
    {
        if (playerPhysics == null) return;

        // Stop sound if the game is over (car crashed)
        if (GameLogicController.Instance != null && GameLogicController.Instance.isGameOver)
        {
            if (engineAudioSource.isPlaying)
            {
                engineAudioSource.Stop();
            }
            return;
        }

        // Only pitch bend once the start clip finishes
        if (!hasStartedAccelerating) return;

        // Pitch bending based on speed
        float currentSpeed = playerPhysics.GetCurrentSpeed();
        float maxSpeed = playerPhysics.maxSpeed;

        float pitchMultiplier = currentSpeed / maxSpeed;
        engineAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, pitchMultiplier);
    }
}

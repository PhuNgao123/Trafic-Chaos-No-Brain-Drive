using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class VehicleAudio : MonoBehaviour
{
    private AudioSource engineAudioSource;
    public AudioClip engineClip;

    [Header("Engine Pitch Settings")]
    public float minPitch = 0.8f;
    public float maxPitch = 1.5f;

    void Start()
    {
        engineAudioSource = GetComponent<AudioSource>();
        if (engineClip != null)
        {
            engineAudioSource.clip = engineClip;
            engineAudioSource.loop = true;
            engineAudioSource.spatialBlend = 1f; // 3D sound for enemies
            engineAudioSource.minDistance = 2f;
            engineAudioSource.maxDistance = 30f;
            
            engineAudioSource.pitch = Random.Range(minPitch, maxPitch);
            
            engineAudioSource.Play();
        }
    }
}

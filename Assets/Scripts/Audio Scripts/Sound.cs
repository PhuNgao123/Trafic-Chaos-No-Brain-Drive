using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;            // Name of the sound (e.g., "MainTheme", "CarCrash")
    public AudioClip clip;         // The actual audio file

    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;

    [HideInInspector]
    public AudioSource source;     // The AudioSource component that will play this clip
}

using UnityEngine;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Configurations")]
    public Sound[] bgmSounds;
    public Sound[] sfxSounds;

    private Coroutine playlistCoroutine;
    private int currentBgmIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize BGM AudioSources
        foreach (Sound s in bgmSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }

        // Initialize SFX AudioSources
        foreach (Sound s in sfxSounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        // Optionally, do nothing here if you want music to only start when the game starts.
    }

    // --- SEQUENTIAL BGM PLAYLIST METHODS ---

    public void PlayBGMPlaylist()
    {
        if (bgmSounds.Length == 0) return;
        
        // Force loop to false on all BGMs so they can move to the next song
        foreach(Sound s in bgmSounds) 
        {
            s.source.loop = false;
            s.source.Stop(); // Stop any currently playing music
        }

        if (playlistCoroutine != null) StopCoroutine(playlistCoroutine);
        playlistCoroutine = StartCoroutine(PlaylistRoutine());
    }

    private IEnumerator PlaylistRoutine()
    {
        while (true)
        {
            Sound s = bgmSounds[currentBgmIndex];
            s.source.Play();

            // Wait until the current clip finishes playing (checking time or isPlaying)
            yield return new WaitWhile(() => s.source.isPlaying);

            // Move to the next song in the array
            currentBgmIndex++;
            if (currentBgmIndex >= bgmSounds.Length)
            {
                currentBgmIndex = 0; // loop back to the first song
            }
        }
    }

    public void StopBGMPlaylist()
    {
        if (playlistCoroutine != null) StopCoroutine(playlistCoroutine);
        foreach(Sound s in bgmSounds) s.source.Stop();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("SFX Sound: " + name + " not found!");
            return;
        }
        s.source.PlayOneShot(s.clip, s.volume);
    }
}

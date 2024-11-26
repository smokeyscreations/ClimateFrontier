using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioPooler : MonoBehaviour
{
    public static AudioPooler Instance;

    [Header("Audio Pool Settings")]
    public AudioSource audioSourcePrefab; // Assign the PooledAudioSource prefab here
    public int poolSize = 20; // Adjust based on expected concurrent sounds

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private Queue<AudioSource> poolQueue = new Queue<AudioSource>();

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Initialize the pool with inactive AudioSources
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource newSource = Instantiate(audioSourcePrefab, transform);
            newSource.gameObject.SetActive(false);
            newSource.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            newSource.spatialBlend = 0.0f; // Ensure the sound is 2D
            newSource.playOnAwake = false;
            poolQueue.Enqueue(newSource);
        }
    }

    // Fetch an available AudioSource from the pool
    public AudioSource GetAudioSource()
    {
        AudioSource source;
        if (poolQueue.Count > 0)
        {
            source = poolQueue.Dequeue();
        }
        else
        {
            // Optional: Expand the pool if needed
            source = Instantiate(audioSourcePrefab, transform);
            source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            source.spatialBlend = 0.0f; // Ensure the sound is 2D
            source.playOnAwake = false;
            Debug.LogWarning("AudioPooler: Pool exhausted, instantiated a new AudioSource.");
        }

        source.gameObject.SetActive(true);
        return source;
    }

    // Return the AudioSource back to the pool
    public void ReturnAudioSource(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.gameObject.SetActive(false);
        poolQueue.Enqueue(source);
    }

    /// <summary>
    /// Sets the global SFX volume via Audio Mixer.
    /// </summary>
    /// <param name="volume">Volume level between 0 and 1.</param>
    public void SetSFXVolume(float volume)
    {
        float mixerVolume = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20; // Convert to dB
        audioMixer.SetFloat("SFXVolume", mixerVolume);
        Debug.Log($"AudioPooler: SFX Volume set to {volume}");
    }

    /// <summary>
    /// Plays a sound effect at full volume as a 2D sound.
    /// </summary>
    /// <param name="clip">AudioClip to play.</param>
    /// <param name="pitchRange">Range for random pitch variation.</param>
    public void PlaySound(AudioClip clip, Vector3 position, Vector2 pitchRange = default)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioPooler: Attempted to play a null AudioClip.");
            return;
        }

        AudioSource source = GetAudioSource();
        source.clip = clip;

        // For 2D sounds, position is irrelevant. Ensure spatialBlend is 0.
        source.transform.position = Vector3.zero; // Position is irrelevant for 2D sounds
        source.spatialBlend = 0.0f; // Ensure the sound is 2D

        source.pitch = 1.0f;
        source.volume = 1.0f; // Ensure full volume

        source.Play();

        // Return the AudioSource to the pool after the clip finishes
        StartCoroutine(ReturnSourceAfterDelay(source, clip.length));
    }

    /// <summary>
    /// Coroutine to return AudioSource after a delay.
    /// </summary>
    /// <param name="source">AudioSource to return.</param>
    /// <param name="delay">Delay in seconds.</param>
    private IEnumerator ReturnSourceAfterDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnAudioSource(source);
    }
}

using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Background Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] musicClips;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer; // Assign the MasterAudioMixer here

    private const string MusicVolumeParameter = "MusicVolume"; // Must match the exposed parameter name

    private void Awake()
    {
        // Implement Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
            InitializeAudioSource();
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
            return;
        }
    }

    private void InitializeAudioSource()
    {
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("AudioManager: AudioSource component added.");
        }

        // Assign the Audio Mixer group to the musicSource
        if (audioMixer != null)
        {
            AudioMixerGroup[] musicGroups = audioMixer.FindMatchingGroups("Music");
            if (musicGroups.Length > 0)
            {
                musicSource.outputAudioMixerGroup = musicGroups[0];
                Debug.Log("AudioManager: Music AudioSource routed to Music group.");
            }
            else
            {
                Debug.LogError("AudioManager: No Music group found in the Audio Mixer.");
            }
        }
        else
        {
            Debug.LogError("AudioManager: Audio Mixer not assigned.");
        }

        musicSource.loop = true;
        // Remove direct volume control to use Audio Mixer instead
        // musicSource.volume = musicVolume;
    }

    /// <summary>
    /// Plays a specific music track by index.
    /// </summary>
    /// <param name="clipIndex">Index of the music clip in the musicClips array.</param>
    public void PlayMusic(int clipIndex)
    {
        if (musicClips == null || musicClips.Length == 0)
        {
            Debug.LogWarning("AudioManager: No music clips assigned.");
            return;
        }

        if (clipIndex < 0 || clipIndex >= musicClips.Length)
        {
            Debug.LogWarning($"AudioManager: Music clip index {clipIndex} is out of bounds.");
            return;
        }

        // Check if the desired music is already playing
        if (musicSource.clip == musicClips[clipIndex] && musicSource.isPlaying)
        {
            Debug.Log($"AudioManager: Music track '{musicClips[clipIndex].name}' is already playing.");
            return;
        }

        if (musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("AudioManager: Stopped current music.");
        }

        musicSource.clip = musicClips[clipIndex];
        musicSource.Play();
        Debug.Log($"AudioManager: Playing music track {musicClips[clipIndex].name}");
    }


    /// <summary>
    /// Stops the currently playing music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
            Debug.Log("AudioManager: Music stopped.");
        }
    }

    /// <summary>
    /// Adjusts the music volume via the Audio Mixer.
    /// </summary>
    /// <param name="volume">Volume level between 0 and 1.</param>
    public void SetMusicVolume(float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogError("AudioManager: Audio Mixer not assigned.");
            return;
        }

        float clampedVolume = Mathf.Clamp01(volume);
        // Convert linear volume (0-1) to decibels (-80 dB to 0 dB)
        float dB = Mathf.Log10(Mathf.Clamp(clampedVolume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(MusicVolumeParameter, dB);
        Debug.Log($"AudioManager: Music volume set to {clampedVolume} ({dB} dB)");
    }
}

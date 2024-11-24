using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class OptionsMenuController : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer; // Assign the MasterAudioMixer here

    private const string MasterVolumeParameter = "MasterVolume"; // Must match the exposed parameter name
    private const string MusicVolumeParameter = "MusicVolume";   // Must match the exposed parameter name
    private const string SFXVolumeParameter = "SFXVolume";       // Must match the exposed parameter name

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private void Start()
    {
        // Load saved volume settings or set defaults
        float savedMasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1.0f);

        masterVolumeSlider.value = savedMasterVolume;
        musicVolumeSlider.value = savedMusicVolume;
        sfxVolumeSlider.value = savedSFXVolume;

        SetMasterVolume(savedMasterVolume);
        SetMusicVolume(savedMusicVolume);
        SetSFXVolume(savedSFXVolume);

        // Add listeners to handle value changes
        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    /// <summary>
    /// Sets the master volume in Audio Mixer and saves the setting.
    /// </summary>
    /// <param name="volume">Volume level between 0 and 1.</param>
    public void SetMasterVolume(float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogError("OptionsMenuController: Audio Mixer not assigned.");
            return;
        }

        float clampedVolume = Mathf.Clamp01(volume);
        // Convert linear volume to decibels
        float dB = Mathf.Log10(Mathf.Clamp(clampedVolume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(MasterVolumeParameter, dB);
        PlayerPrefs.SetFloat(MasterVolumeKey, clampedVolume);
        Debug.Log($"OptionsMenu: Master volume set to {clampedVolume} ({dB} dB)");
    }

    /// <summary>
    /// Sets the music volume in Audio Mixer and saves the setting.
    /// </summary>
    /// <param name="volume">Volume level between 0 and 1.</param>
    public void SetMusicVolume(float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogError("OptionsMenuController: Audio Mixer not assigned.");
            return;
        }

        float clampedVolume = Mathf.Clamp01(volume);
        // Convert linear volume to decibels
        float dB = Mathf.Log10(Mathf.Clamp(clampedVolume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(MusicVolumeParameter, dB);
        PlayerPrefs.SetFloat(MusicVolumeKey, clampedVolume);
        Debug.Log($"OptionsMenu: Music volume set to {clampedVolume} ({dB} dB)");
    }

    /// <summary>
    /// Sets the SFX volume in Audio Mixer and saves the setting.
    /// </summary>
    /// <param name="volume">Volume level between 0 and 1.</param>
    public void SetSFXVolume(float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogError("OptionsMenuController: Audio Mixer not assigned.");
            return;
        }

        float clampedVolume = Mathf.Clamp01(volume);
        // Convert linear volume to decibels
        float dB = Mathf.Log10(Mathf.Clamp(clampedVolume, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(SFXVolumeParameter, dB);
        PlayerPrefs.SetFloat(SFXVolumeKey, clampedVolume);
        Debug.Log($"OptionsMenu: SFX volume set to {clampedVolume} ({dB} dB)");
    }

    private void OnDestroy()
    {
        // Remove listeners to prevent memory leaks
        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(SetSFXVolume);
    }

    /// <summary>
    /// Call this method when clicking the Back button to return to the main menu.
    /// </summary>
    public void OnBackButtonPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

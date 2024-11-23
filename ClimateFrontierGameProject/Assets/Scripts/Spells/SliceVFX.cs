using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliceVFX : MonoBehaviour, IPoolable
{
    private ParticleSystem particleSystem;
    public AudioClip[] impactSounds; // Array of impact sound variants

    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();

        if (particleSystem == null)
        {
            Debug.LogError("SliceVFX requires a ParticleSystem component.");
        }

        if (impactSounds == null || impactSounds.Length == 0)
        {
            Debug.LogError("SliceVFX requires at least one impact sound.");
        }
    }

    public void OnObjectSpawn()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }

        PlayImpactSound();
    }

    public void OnObjectReturn()
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
        }
    }

    private void PlayImpactSound()
    {
        if (impactSounds.Length == 0)
            return;

        // Select a random sound
        int soundIndex = Random.Range(0, impactSounds.Length);
        AudioClip clip = impactSounds[soundIndex];

        // Get an AudioSource from the pool
        AudioSource source = AudioPooler.Instance.GetAudioSource();

        // Configure the AudioSource
        source.clip = clip;
        source.transform.position = transform.position; // Position the sound at the impact location
        source.pitch = Random.Range(0.95f, 1.05f); // Slight pitch variation for realism
        source.volume = Random.Range(0.9f, 1.0f); // Slight volume variation
        source.spatialBlend = 1.0f; // 3D sound
        source.Play();

        // Return the AudioSource to the pool after the clip finishes
        StartCoroutine(ReturnSourceAfterDelay(source, clip.length));
    }

    private IEnumerator ReturnSourceAfterDelay(AudioSource source, float delay)
    {
        yield return new WaitForSeconds(delay);
        AudioPooler.Instance.ReturnAudioSource(source);
    }
}

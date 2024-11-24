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

        // Use AudioPooler's PlaySound method
        AudioPooler.Instance.PlaySound(clip, transform.position, new Vector2(0.95f, 1.05f));
    }
}

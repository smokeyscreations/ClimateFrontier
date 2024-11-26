using UnityEngine;
using System.Collections;

public class SlashVFX : MonoBehaviour, IPoolable
{
    [Header("Particle Systems")]
    private ParticleSystem[] particleSystems;

    [Header("Audio Settings")]
    public AudioClip spawnSound;        // Sound played when SlashVFX is spawned

    private Coroutine deactivateCoroutine;

    private void Awake()
    {
        // Retrieve all child ParticleSystem components
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void OnObjectSpawn()
    {
        // Play all particle systems
        foreach (var ps in particleSystems)
        {
            ps.Clear();
            ps.Play();
        }

        // Play the spawn sound using AudioPooler
        PlaySpawnSound();

        // Start the coroutine to deactivate after a set duration
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterDelay(1.5f)); // Adjust duration as needed
    }

    public void OnObjectReturn()
    {
        // Stop all particle systems
        foreach (var ps in particleSystems)
        {
            ps.Stop();
        }

        // Stop the coroutine if it's still running
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        // Return this object to the pool
        if (ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.ReturnToPool("SlashVFX", gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void PlaySpawnSound()
    {
        if (spawnSound != null)
        {
            // Play the spawn sound as a 2D sound by passing Vector3.zero
            AudioPooler.Instance.PlaySound(spawnSound, Vector3.zero, new Vector2(0.95f, 1.05f));
        }
        else
        {
            Debug.LogWarning("SlashVFX: Spawn sound is not assigned.");
        }
    }
}

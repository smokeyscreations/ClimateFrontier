using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour, IPoolable
{
    private ParticleSystem particleSystem;
    private float effectDuration = 1.5f; // Duration before returning to pool

    private Coroutine returnCoroutine; // Reference to the running coroutine

    void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        if (particleSystem == null)
        {
            Debug.LogError($"{gameObject.name}: HitEffect requires a ParticleSystem component.");
        }
    }

    // Called when the object is spawned from the pool
    public void OnObjectSpawn()
    {
        if (particleSystem != null)
        {
            // Stop any existing particle playback to reset the system
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            // Play the particle effect
            particleSystem.Play();

            // Start coroutine to return to pool after effect duration
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
            }
            returnCoroutine = StartCoroutine(ReturnToPoolAfterDelay(effectDuration));
        }

        // Ensure the object is active
        gameObject.SetActive(true);

        Debug.Log($"{gameObject.name}: HitEffect spawned and Particle System started.");
    }

    // Called when the object is returned to the pool
    public void OnObjectReturn()
    {
        if (particleSystem != null)
        {
            // Stop the particle system to prevent it from playing while pooled
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        // Ensure the object is inactive
        gameObject.SetActive(false);

        // Stop any running coroutine to prevent it from trying to return the object again
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        Debug.Log($"{gameObject.name}: HitEffect returned to 'HitEffect' pool.");
    }

    // Coroutine to return the HitEffect to the pool after a delay
    private IEnumerator ReturnToPoolAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ObjectPooler.Instance.ReturnToPool("HitEffect", gameObject);
        Debug.Log($"{gameObject.name}: HitEffect automatically returned to 'HitEffect' pool after {delay} seconds.");
    }
}

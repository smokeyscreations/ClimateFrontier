using UnityEngine;
using System.Collections;

public class SlashVFX : MonoBehaviour, IPoolable
{
    private ParticleSystem[] particleSystems;
    private Coroutine deactivateCoroutine;

    private void Awake()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    public void OnObjectSpawn()
    {
        Debug.Log($"[SlashVFX] OnObjectSpawn called for {gameObject.name}");
        // Play all particle systems
        foreach (var ps in particleSystems)
        {
            ps.Clear();
            ps.Play();
        }

        // Start the coroutine to deactivate after a set duration
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterDelay(1.5f)); // Set your desired duration here
    }

    public void OnObjectReturn()
    {
        Debug.Log($"[SlashVFX] OnObjectReturn called for {gameObject.name}");
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
        Debug.Log($"[SlashVFX] DeactivateAfterDelay started for {gameObject.name}");
        yield return new WaitForSecondsRealtime(delay);
        Debug.Log($"[SlashVFX] DeactivateAfterDelay completed for {gameObject.name}");
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        Debug.Log($"[SlashVFX] Returning to pool: {gameObject.name}");
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
}

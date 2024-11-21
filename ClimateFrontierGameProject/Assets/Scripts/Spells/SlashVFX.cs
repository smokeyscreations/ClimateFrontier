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
}

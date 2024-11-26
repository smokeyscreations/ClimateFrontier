// Assets/Scripts/Boss/Beam.cs
using UnityEngine;

public class Beam : MonoBehaviour, IPoolable
{
    [Tooltip("Duration before the beam is returned to the pool.")]
    public float duration = 4f;

    private float timer = 0f;

    void OnEnable()
    {
        // Reset timer when enabled
        timer = 0f;
        Debug.Log($"{gameObject.name}: Beam enabled and timer reset.");
    }

    public void OnObjectSpawn()
    {
        timer = 0f;
        // Initialize or reset any properties if needed
        Debug.Log($"{gameObject.name}: OnObjectSpawn called.");
    }

    public void OnObjectReturn()
    {
        // Clean up or reset properties before returning to the pool
        Debug.Log($"{gameObject.name}: OnObjectReturn called.");
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            ObjectPooler.Instance.ReturnToPool("Beam", gameObject);
            Debug.Log($"{gameObject.name}: Beam duration ended. Returning to pool.");
        }
    }
}

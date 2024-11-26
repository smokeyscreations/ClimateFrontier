// Assets/Scripts/Boss/Claw.cs
using UnityEngine;

public class Claw : MonoBehaviour, IPoolable
{
    [Tooltip("Duration before the claw is returned to the pool.")]
    public float duration = 1.5f;

    private float timer = 0f;

    void OnEnable()
    {
        // Reset timer when enabled
        timer = 0f;
        Debug.Log($"{gameObject.name}: Claw enabled and timer reset.");
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
            ObjectPooler.Instance.ReturnToPool("Claw", gameObject);
            Debug.Log($"{gameObject.name}: Claw duration ended. Returning to pool.");
        }
    }
}

using UnityEngine;
using System.Collections;

public class MagicProjectile : MonoBehaviour, IPoolable
{
    [Header("Projectile Settings")]
    public float speed = 2f;
    public int damage = 10;

    private Transform target;
    private Vector3 direction;
    private float lifetime = 3f; // Time before the projectile is returned to the pool

    private Coroutine returnCoroutine; // Reference to the running coroutine

    void Awake()
    {
        // Ensure the Projectile has a Collider set as Trigger
        Collider collider = GetComponent<Collider>();
        if (collider != null && !collider.isTrigger)
        {
            collider.isTrigger = true;
            Debug.LogWarning($"{gameObject.name}: Collider was not set as Trigger. It has been corrected.");
        }
        else if (collider == null)
        {
            Debug.LogError($"{gameObject.name}: Collider component is missing.");
        }
    }

    // Initialization method called when spawning from pool
    public void Initialize(Transform targetTransform, float projectileSpeed, int projectileDamage)
    {
        target = targetTransform;
        speed = projectileSpeed;
        damage = projectileDamage;

        // Calculate direction towards the target
        direction = (target.position - transform.position).normalized;

        // Align rotation with direction
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            Debug.Log($"{gameObject.name} initialized with direction {direction} and rotation {transform.rotation.eulerAngles}");
        }

        // Start the lifetime countdown
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }
        returnCoroutine = StartCoroutine(ReturnToPoolAfterLifetime(lifetime));
    }

    void Update()
    {
        if (target == null)
        {
            // Return to pool if no target
            ReturnToPool();
            return;
        }

        // Move towards the target
        transform.position += direction * speed * Time.deltaTime;

        // Optionally, deactivate after reaching the target
        if (Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            HitTarget();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"{gameObject.name} hit the player for {damage} damage.");
            }

            // Trigger Hit Effect
            TriggerHitEffect();

            // Return to pool
            ReturnToPool();
        }
    }

    void HitTarget()
    {
        if (target != null)
        {
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"{gameObject.name} hit the player for {damage} damage.");
            }

            // Trigger Hit Effect
            TriggerHitEffect();
        }

        // Return to pool
        ReturnToPool();
    }

    void TriggerHitEffect()
    {
        // Spawn HitEffect from the pool at the projectile's position
        GameObject hitEffect = ObjectPooler.Instance.SpawnFromPool("HitEffect", transform.position, Quaternion.identity);
        if (hitEffect == null)
        {
            Debug.LogError($"{gameObject.name}: Failed to spawn HitEffect from pool.");
        }
    }

    private IEnumerator ReturnToPoolAfterLifetime(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
        ObjectPooler.Instance.ReturnToPool("MagicProjectile", gameObject);
    }

    // IPoolable implementation
    public void OnObjectSpawn()
    {
        // Reset any necessary variables or states
        target = null;
        direction = Vector3.zero;

        // Ensure the projectile is active
        gameObject.SetActive(true);
    }

    public void OnObjectReturn()
    {
        // Clean up or reset states before returning to the pool
        target = null;
        direction = Vector3.zero;
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }
    }
}

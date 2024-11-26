// Assets/Scripts/Enemies/SlashProjectile.cs
using UnityEngine;

public class SlashProjectile : MonoBehaviour, IPoolable
{
    [HideInInspector]
    public float speed = 5f; // Movement speed, set by JumpAttackVFXHandler

    [Header("Projectile Settings")]
    [Tooltip("Time before projectile is returned to pool.")]
    public float lifetime = 3f;

    [Tooltip("Damage to apply to the player upon collision.")]
    public int damage = 10;

    private Vector3 direction; // Movement direction
    private float timer = 0f;  // Timer to track lifetime

    void OnEnable()
    {
        // Reset timer when the projectile is enabled
        timer = 0f;
        Debug.Log($"{gameObject.name}: Enabled and timer reset.");
    }

    /// <summary>
    /// Initialize the projectile's direction.
    /// </summary>
    /// <param name="shootDirection">The direction to shoot the projectile.</param>
    public void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection.normalized;
        transform.forward = direction; // Align projectile's forward direction
        Debug.Log($"{gameObject.name}: Initialized with direction {direction} and speed {speed}.");
    }

    /// <summary>
    /// Called by the ObjectPooler when the projectile is spawned.
    /// Reset any necessary state here.
    /// </summary>
    public void OnObjectSpawn()
    {
        timer = 0f;
        Debug.Log($"{gameObject.name}: OnObjectSpawn called.");
    }

    /// <summary>
    /// Called by the ObjectPooler when the projectile is returned to the pool.
    /// Clean up any states here.
    /// </summary>
    public void OnObjectReturn()
    {
        Debug.Log($"{gameObject.name}: OnObjectReturn called.");
    }

    void Update()
    {
        // Move the projectile forward
        transform.position += direction * speed * Time.deltaTime;

        // Increment the timer
        timer += Time.deltaTime;

        // Check if lifetime has expired
        if (timer >= lifetime)
        {
            Debug.Log($"{gameObject.name}: Lifetime expired. Returning to pool.");
            ReturnToPool();
        }
    }

    /// <summary>
    /// Handles collision with player or ground.
    /// </summary>
    /// <param name="other">The collider the projectile has hit.</param>
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"{gameObject.name}: OnTriggerEnter with {other.gameObject.name}.");

        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"{gameObject.name}: Applied {damage} damage to player.");
            }
            else
            {
                Debug.LogWarning($"{gameObject.name}: PlayerHealth component not found on player.");
            }

            // Return to pool
            ReturnToPool();
        }
        else if (other.CompareTag("Ground"))
        {
            // Optional: Instantiate additional VFX on ground hit
            // Example: Instantiate(groundShatterVFXPrefab, transform.position, Quaternion.identity);
            Debug.Log($"{gameObject.name}: Hit the ground. Returning to pool.");

            // Return to pool
            ReturnToPool();
        }
    }

    /// <summary>
    /// Returns the projectile to the pool.
    /// </summary>
    private void ReturnToPool()
    {
        // Ensure that the ObjectPooler instance exists
        if (ObjectPooler.Instance == null)
        {
            Debug.LogError($"{gameObject.name}: ObjectPooler instance is null. Cannot return to pool.");
            return;
        }

        // Return to pool
        ObjectPooler.Instance.ReturnToPool("SlashProjectile", gameObject);
        Debug.Log($"{gameObject.name}: Returned to pool.");
    }
}

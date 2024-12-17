using UnityEngine;
using System.Collections.Generic;

public class WolfHomingRFXProjectile : MonoBehaviour, IPoolable
{
    [Header("Projectile Settings")]
    public float speed = 10f;               // Speed of the projectile
    public float maxDistance = 30f;         // Max distance to travel before returning to pool
    public float lifeTime = 5f;            // Time after which projectile auto-returns if it doesn’t collide
    public float collisionRadius = 0.5f;    // Radius for simple collision check (if using Raycast)
    public LayerMask collisionLayers;       // Layers this projectile should collide with (e.g. Enemy)

    [Header("Damage Settings")]
    public float damage = 10f;             // Damage dealt upon collision

    [Header("Effects")]
    public GameObject[] collisionEffects;   // Any VFX objects to spawn at collision
    public float destroyEffectsDelay = 3f;  // Time to keep collision effect before returning to pool
    public bool collisionEffectInWorldSpace = true;

    [HideInInspector] public GameObject Target;

    // Optional: If you need the projectile to do a wiggle or random offset, you can keep that from your original script.
    // For simplicity, we skip RandomMoveRadius/RandomMoveSpeed etc.

    private float distanceTraveled;
    private float timeActive;
    private Vector3 lastPosition;

    // For pooling
    private string poolTag = "WolfProjectile";  // optional: store the tag used to spawn from pool, for returning later.

    public void OnObjectSpawn()
    {
        // Reset state
        distanceTraveled = 0f;
        timeActive = 0f;
        lastPosition = transform.position;
    }

    public void OnObjectReturn()
    {
        // Clean up projectile if needed
        Target = null;  // clear reference
    }

    void OnEnable()
    {
        // If the projectile was re-enabled from the pool, also re-initialize
        OnObjectSpawn();
    }

    void Update()
    {
        if (Target == null)
        {
            // If no target, just go forward or return to pool
            StraightFlight();
            return;
        }

        timeActive += Time.deltaTime;
        if (timeActive >= lifeTime)
        {
            ReturnToPool();
            return;
        }

        // Move toward Target
        Vector3 direction = (Target.transform.position - transform.position).normalized;
        float moveStep = speed * Time.deltaTime;

        transform.position += direction * moveStep;
        transform.LookAt(transform.position + direction);

        distanceTraveled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (distanceTraveled >= maxDistance)
        {
            ReturnToPool();
            return;
        }

        // Optional: Raycast for collision
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, collisionRadius, direction, out hit, moveStep, collisionLayers))
        {
            OnHit(hit.point, hit.normal, hit.collider);
        }
    }

    private void StraightFlight()
    {
        timeActive += Time.deltaTime;
        if (timeActive >= lifeTime)
        {
            ReturnToPool();
            return;
        }

        Vector3 direction = transform.forward;
        float moveStep = speed * Time.deltaTime;
        transform.position += direction * moveStep;

        distanceTraveled += moveStep;
        if (distanceTraveled >= maxDistance)
        {
            ReturnToPool();
        }

        // SphereCast for collision
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, collisionRadius, direction, out hit, moveStep, collisionLayers))
        {
            OnHit(hit.point, hit.normal, hit.collider);
        }
    }

    void OnHit(Vector3 hitPoint, Vector3 hitNormal, Collider hitCollider)
    {
        // Deal damage to whatever we collided with
        IDamageable enemy = hitCollider.GetComponent<IDamageable>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Spawn collision effects
        if (collisionEffects != null && collisionEffects.Length > 0)
        {
            foreach (var effectPrefab in collisionEffects)
            {
                GameObject effectInstance = Instantiate(
                    effectPrefab,
                    hitPoint + hitNormal * 0.1f, // offset slightly
                    Quaternion.LookRotation(hitNormal)
                );
                if (!collisionEffectInWorldSpace)
                {
                    // Parent the effect to the projectile if desired
                    effectInstance.transform.parent = this.transform;
                }
                Destroy(effectInstance, destroyEffectsDelay);
            }
        }

        ReturnToPool();
    }

    // If you prefer using OnTriggerEnter instead of Raycast, make sure your projectile has a trigger collider
    // and the enemy object has a collider with the right layer. Then do something like:
    /*
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & collisionLayers) != 0)
        {
            // We collided with a valid layer
            OnHit(transform.position, -transform.forward, other);
        }
    }
    */

    private void ReturnToPool()
    {
        // If we spawned from a pool with a known tag, return it
        // Or if you store the pool tag externally (like “WolfProjectile”), pass that in
        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }

    // Optional: If you need to store the poolTag at spawn time
    public void SetPoolTag(string tag)
    {
        poolTag = tag;
    }
}

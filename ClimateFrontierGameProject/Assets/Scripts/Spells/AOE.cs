using UnityEngine;
using System.Collections;

public class AOE : MonoBehaviour, IPoolable, ISpell
{
    [Header("AOE Settings")]
    public float speed = 20f;           // Speed at which the AOE moves forward
    public float range = 10f;           // Maximum range the AOE can travel
    public LayerMask enemyLayerMask;    // Layer mask to identify enemies
    public float activeDuration = 1f;   // Duration the AOE remains active

    [Header("Audio Settings")]
    public AudioClip spawnSound;        // Sound played when AOE is spawned

    private Vector3 startPosition;
    private float spellDamage;          // Damage assigned from SpellData
    private string poolTag;             // Tag to use when returning to the pool

    private Collider aoeCollider;

    private Coroutine deactivateCoroutine;

    private Transform target;

    private void Awake()
    {
        // Get the Collider component on the same GameObject
        aoeCollider = GetComponent<Collider>();
        if (aoeCollider == null)
        {
            Debug.LogError("AOE script requires a Collider component on the same GameObject.");
        }
        else if (!aoeCollider.isTrigger)
        {
            Debug.LogWarning("AOE Collider should be set as Trigger.");
            aoeCollider.isTrigger = true;
        }
    }

    public void Initialize(SpellData spellData, BasePlayer player, Transform target)
    {
        spellDamage = spellData.damage;
        range = spellData.spellAttackRange;
        enemyLayerMask = player.characterData.enemyLayerMask;
        activeDuration = spellData.activeDuration;
        poolTag = spellData.tag; // Store the tag for returning to the pool
        this.target = target;
        startPosition = transform.position;

        Debug.Log($"AOE Initialized with Damage: {spellDamage}, Range: {range}, Speed: {speed}, Active Duration: {activeDuration}, Pool Tag: {poolTag}");
    }

    public void OnObjectSpawn()
    {
        // Reset position when the object is activated
        startPosition = transform.position;

        // Play the spawn sound
        PlaySpawnSound();

        // Start the delayed deactivation coroutine
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterDelay(activeDuration));
    }

    public void OnObjectReturn()
    {
        // Stop the coroutine if it's still running
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }
    }

    private void Update()
    {
        // Move the AOE forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        DeactivateAndReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is within the enemy layer mask
        if (((1 << other.gameObject.layer) & enemyLayerMask) != 0)
        {
            // Attempt to get any component that implements IDamageable
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(Mathf.RoundToInt(spellDamage));
            }
            else
            {
                Debug.LogWarning($"Object {other.gameObject.name} does not implement IDamageable.");
            }
        }
        else
        {
            Debug.Log($"Object {other.gameObject.name} is not on an enemy layer.");
        }
    }



    private void DeactivateAndReturnToPool()
    {
        if (!string.IsNullOrEmpty(poolTag) && ObjectPooler.Instance != null)
        {
            ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
        }
        else
        {
            Debug.LogWarning("Pool tag is not set or ObjectPooler instance is null. Cannot return to pool.");
            gameObject.SetActive(false); // Fallback to just deactivating
        }
    }

    private void PlaySpawnSound()
    {
        if (spawnSound != null)
        {
            // Play the spawn sound as a 2D sound by ignoring position
            AudioPooler.Instance.PlaySound(spawnSound, Vector3.zero, new Vector2(0.95f, 1.05f));
        }
        else
        {
            Debug.LogWarning("AOE: Spawn sound is not assigned.");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (aoeCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}

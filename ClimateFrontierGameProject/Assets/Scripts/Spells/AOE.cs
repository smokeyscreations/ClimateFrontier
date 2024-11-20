using UnityEngine;
using System.Collections;

public class AOE : MonoBehaviour, IPoolable, ISpell
{
    [Header("AOE Settings")]
    public float speed = 20f;           // Speed at which the AOE moves forward
    public float range = 10f;           // Maximum range the AOE can travel
    public LayerMask enemyLayerMask;    // Layer mask to identify enemies
    public float activeDuration = 1f;   // Duration the AOE remains active

    private Vector3 startPosition;
    private float spellDamage;          // Damage assigned from SpellData
    private string poolTag;             // Tag to use when returning to the pool

    private Collider aoeCollider;

    private Coroutine deactivateCoroutine;

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

    /// <summary>
    /// Initializes the AOE effect with data from SpellData.
    /// This method is called externally after spawning the AOE from the pool.
    /// </summary>
    public void Initialize(SpellData spellData, BasePlayer player)
    {
        spellDamage = spellData.damage;
        range = spellData.spellAttackRange;
        enemyLayerMask = player.characterData.enemyLayerMask;
        activeDuration = spellData.activeDuration;
        poolTag = spellData.tag; // Store the tag for returning to the pool

        startPosition = transform.position;

        Debug.Log($"AOE Initialized with Damage: {spellDamage}, Range: {range}, Speed: {speed}, Active Duration: {activeDuration}, Pool Tag: {poolTag}");
    }

    /// <summary>
    /// Called when the object is spawned from the pool.
    /// Implements the IPoolable interface.
    /// </summary>
    public void OnObjectSpawn()
    {
        // Reset position when the object is activated
        startPosition = transform.position;

        // Start the delayed deactivation coroutine
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterDelay(activeDuration));

        Debug.Log("AOE OnObjectSpawn called.");
    }

    /// <summary>
    /// Called when the object is returned to the pool.
    /// Implements the IPoolable interface.
    /// </summary>
    public void OnObjectReturn()
    {
        Debug.Log("AOE OnObjectReturn called.");

        // Stop the coroutine if it's still running
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }

        // Reset any necessary state here
        // For example, reset Particle Systems, animations, etc.
    }

    private void Update()
    {
        // Move the AOE forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);

        // Optional: If you still want range-based deactivation, uncomment the following lines
        /*
        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            Debug.Log("AOE Range exceeded. Deactivating.");
            DeactivateAndReturnToPool();
        }
        */
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("AOE Duration elapsed. Deactivating.");

        DeactivateAndReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Debug log for collision
        Debug.Log($"AOE collided with {other.gameObject.name} on layer {other.gameObject.layer}");

        // Check if the collided object is within the enemy layer mask
        if (((1 << other.gameObject.layer) & enemyLayerMask) != 0)
        {
            // Attempt to get the enemy's health component
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(Mathf.RoundToInt(spellDamage)); // Use damage from SpellData
                Debug.Log($"AOE hit {enemy.gameObject.name} for {spellDamage} damage.");
            }
            else
            {
                Debug.LogWarning($"Object {other.gameObject.name} does not have a BaseEnemy component.");
            }
        }
        else
        {
            Debug.Log($"Object {other.gameObject.name} is not on an enemy layer.");
        }
    }

    private void DeactivateAndReturnToPool()
    {
        Debug.Log("AOE Deactivating and returning to pool.");

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

    private void OnDrawGizmosSelected()
    {
        if (aoeCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}

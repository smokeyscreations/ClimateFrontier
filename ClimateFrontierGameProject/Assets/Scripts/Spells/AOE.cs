using UnityEngine;
using System.Collections;

public class AOE : MonoBehaviour
{
    [Header("AOE Settings")]
    public float speed = 20f;           // Speed at which the AOE moves forward
    public float range = 10f;           // Maximum range the AOE can travel
    public LayerMask enemyLayerMask;    // Layer mask to identify enemies
    public float activeDuration = 1f;   // Duration the AOE remains active

    private Vector3 startPosition;
    private float spellDamage;          // Damage assigned from SpellData

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
    /// </summary>
    /// <param name="dmg">Damage to apply upon collision.</param>
    /// <param name="rng">Maximum range before the AOE is disabled.</param>
    /// <param name="enemyMask">Layer mask to identify enemy layers.</param>
    /// <param name="duration">Duration the AOE remains active.</param>
    public void Initialize(float dmg, float rng, LayerMask enemyMask, float duration)
    {
        spellDamage = dmg;              // Set damage from SpellData
        range = rng;                    // Set range from SpellData
        enemyLayerMask = enemyMask;     // Set layer mask from SpellData
        activeDuration = duration;      // Set active duration from SpellData

        startPosition = transform.position;

        Debug.Log($"AOE Initialized with Damage: {spellDamage}, Range: {range}, Speed: {speed}, Active Duration: {activeDuration}");
    }

    private void OnEnable()
    {
        // Reset position when the object is activated
        startPosition = transform.position;

        // Start the delayed deactivation coroutine
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
        deactivateCoroutine = StartCoroutine(DeactivateAfterDelay(activeDuration));

        Debug.Log("AOE Enabled.");
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
            gameObject.SetActive(false);
        }
        */
    }

    private IEnumerator DeactivateAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("AOE Duration elapsed. Deactivating.");
        gameObject.SetActive(false);
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

    private void OnDisable()
    {
        Debug.Log("AOE Disabled. Returning to pool.");
        // Notify the pooler to return this object to the pool
        if (AOEPooler.Instance != null)
        {
            // Assuming the pool tag is "AOE"
            AOEPooler.Instance.ReturnToPool("AOE", gameObject);
        }
        else
        {
            Debug.LogError("AOEPooler instance not found!");
        }

        // Stop the coroutine if it's still running
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
            deactivateCoroutine = null;
        }

        // Reset the AOE's state before returning to the pool
    
    }

    /// <summary>
    /// Resets the AOE's position, rotation, and other attributes to their default states.
    /// </summary>

    private void OnDrawGizmosSelected()
    {
        if (aoeCollider != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, range);
        }
    }
}

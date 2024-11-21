using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlashCircleSpell : MonoBehaviour, IPoolable, ISpell
{
    public float damagePerTick = 10f;
    public float tickInterval = 1f;
    public float duration = 10f;
    public LayerMask enemyLayerMask;

    private float spellRadius;
    private List<BaseEnemy> enemiesInRange = new List<BaseEnemy>();
    private Coroutine damageCoroutine;
    private string poolTag;
    
    private void Awake()
    {
        // Ensure the Collider is set as a trigger
        Collider collider = GetComponent<Collider>();
        if (collider == null)
        {
            Debug.LogError("SlashCircleSpell requires a Collider component.");
        }
        else if (!collider.isTrigger)
        {
            collider.isTrigger = true;
        }
    }

    public void Initialize(SpellData spellData, BasePlayer player, Transform target)
    {
        Debug.Log($"enemyLayerMask value: {enemyLayerMask.value}");
        damagePerTick = spellData.damage;
        duration = spellData.activeDuration;
        spellRadius = spellData.spellAttackRange;
        enemyLayerMask = player.EnemyLayerMask;
        poolTag = spellData.tag;

        // Set the size of the collider
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.radius = spellRadius;
        }

        // Start the damage over time coroutine
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
        }
        damageCoroutine = StartCoroutine(DamageOverTime());

        // Start the duration countdown
        StartCoroutine(SpellDuration());
    }

    public void OnObjectSpawn()
    {
        // Reset any necessary variables
        enemiesInRange.Clear();
    }

    public void OnObjectReturn()
    {
        // Clean up
        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
        enemiesInRange.Clear();
    }

    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);
            Debug.Log($"Damage tick occurred. Enemies in range: {enemiesInRange.Count}");
            foreach (BaseEnemy enemy in enemiesInRange)
            {
                if (enemy != null)
                {
                    enemy.TakeDamage(damagePerTick);
                    Debug.Log($"Dealt {damagePerTick} damage to {enemy.gameObject.name}");
                }
                else
                {
                    Debug.Log("Enemy in list is null.");
                }
            }
        }
    }


    private IEnumerator SpellDuration()
    {
        yield return new WaitForSeconds(duration);
        DeactivateAndReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter with {other.gameObject.name}");
        if (((1 << other.gameObject.layer) & enemyLayerMask) != 0)
        {
            if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                if (!enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Add(enemy);
                    Debug.Log($"Added {enemy.gameObject.name} to enemiesInRange");
                }
            }
            else
            {
                Debug.LogWarning($"{other.gameObject.name} does not have BaseEnemy component.");
            }
        }
        else
        {
            Debug.Log($"{other.gameObject.name} is not on the enemy layer.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"OnTriggerExit with {other.gameObject.name}");
        if (((1 << other.gameObject.layer) & enemyLayerMask) != 0)
        {
            if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                if (enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Remove(enemy);
                    Debug.Log($"Removed {enemy.gameObject.name} from enemiesInRange");
                }
            }
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
}

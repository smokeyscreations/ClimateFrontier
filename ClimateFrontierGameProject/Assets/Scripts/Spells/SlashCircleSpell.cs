using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SlashCircleSpell : MonoBehaviour, IPoolable, ISpell
{
    public float damagePerTick = 10f;
    public float tickInterval = 0.1f;
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

        // Unsubscribe from all enemies' OnEnemyDeath events
        foreach (var enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                enemy.OnEnemyDeath -= RemoveEnemyFromList;
            }
        }

        enemiesInRange.Clear();
    }

    private IEnumerator DamageOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(tickInterval);

            // Iterate over a copy of the list to prevent modification during iteration
            List<BaseEnemy> enemiesToDamage = new List<BaseEnemy>(enemiesInRange);

            foreach (BaseEnemy enemy in enemiesToDamage)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                {
                    enemy.TakeDamage(damagePerTick);
                }
                else
                {
                    // remove the enemy from the list if it's inactive
                    RemoveEnemyFromList(enemy);
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

        if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
        {
            if (!enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
                // Subscribe to the enemy's OnEnemyDeath event
                enemy.OnEnemyDeath += RemoveEnemyFromList;
            }
        }
        else
        {
            Debug.LogWarning($"{other.gameObject.name} does not have BaseEnemy component.");
        }
    }


    private void OnTriggerExit(Collider other)
    {

        if (((1 << other.gameObject.layer) & enemyLayerMask) != 0)
        {
            if (other.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
            {
                if (enemiesInRange.Contains(enemy))
                {
                    enemiesInRange.Remove(enemy);
                    // Unsubscribe from the enemy's OnEnemyDeath event
                    enemy.OnEnemyDeath -= RemoveEnemyFromList;
                }
            }
        }
    }

    private void RemoveEnemyFromList(BaseEnemy enemy)
    {
        if (enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Remove(enemy);
            // Unsubscribe from the event
            enemy.OnEnemyDeath -= RemoveEnemyFromList;
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

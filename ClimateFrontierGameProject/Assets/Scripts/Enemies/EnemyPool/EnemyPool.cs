using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    [Header("Pool Setup")]
    public List<EnemyPoolData> enemyTypes; // Defined in the Inspector

    private Dictionary<string, EnemyPoolData> poolDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            poolDictionary = new Dictionary<string, EnemyPoolData>();
            // Initialize each enemy type
            foreach (var data in enemyTypes)
            {
                string key = data.prefab.gameObject.name;
                data.queue = new Queue<BaseEnemy>();
                poolDictionary[key] = data;

                Debug.Log($"EnemyPool: Registered {key} with maxCount = {data.maxCount}.");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Attempts to spawn an enemy of the given prefab type if under maxCount.
    /// </summary>
    public BaseEnemy GetFromPool(BaseEnemy prefab)
    {
        string key = prefab.gameObject.name;

        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogError($"EnemyPool: No pool data found for {key}.");
            return null;
        }

        EnemyPoolData data = poolDictionary[key];

        // Check if activeCount < maxCount
        if (data.activeCount >= data.maxCount)
        {
            Debug.LogWarning($"EnemyPool: Max active count reached for {key}. Spawn denied.");
            return null;
        }

        // If there's an inactive enemy in queue, reuse it
        if (data.queue.Count > 0)
        {
            BaseEnemy enemy = data.queue.Dequeue();
            data.activeCount++;
            return enemy;
        }
        else
        {
            // Instantiate a new enemy if queue is empty
            BaseEnemy newEnemy = Instantiate(prefab);
            newEnemy.gameObject.name = key; // Keep consistent naming
            data.activeCount++;
            return newEnemy;
        }
    }

    /// <summary>
    /// Return the enemy to the pool, decrement active count.
    /// </summary>
    public void ReturnToPool(BaseEnemy enemy)
    {
        string key = enemy.gameObject.name;
        if (!poolDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"EnemyPool: No pool entry found for {key}. Creating new entry...");
            // Optionally create a new entry or just destroy
            return;
        }

        EnemyPoolData data = poolDictionary[key];

        // Decrement active count
        data.activeCount = Mathf.Max(0, data.activeCount - 1);

        // Deactivate and enqueue
        enemy.gameObject.SetActive(false);
        data.queue.Enqueue(enemy);
    }

    public void ReturnToPoolWithDelay(BaseEnemy enemy, float delay)
    {
        StartCoroutine(ReturnRoutine(enemy, delay));
    }

    private IEnumerator ReturnRoutine(BaseEnemy enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool(enemy);
    }

}

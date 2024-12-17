using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public List<Transform> spawnPoints;
    public float spawnInterval = 2f;

    private float spawnTimer;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Count == 0) return;

        var spawnDataList = EnemyPool.Instance.enemyTypes;
        if (spawnDataList == null || spawnDataList.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: No enemy types available in the EnemyPool.");
            return;
        }

        // Gather only the types not at maxCount
        List<EnemyPoolData> validTypes = new List<EnemyPoolData>();
        foreach (var data in spawnDataList)
        {
            if (data.activeCount < data.maxCount)
                validTypes.Add(data);
        }

        if (validTypes.Count == 0)
        {
            // All enemy types are at max. No spawns possible this cycle.
            Debug.Log("All enemy types have reached maxCount. Spawning stopped.");
            return;
        }

        // Choose from valid types only
        int randomIndex = Random.Range(0, validTypes.Count);
        EnemyPoolData chosenData = validTypes[randomIndex];

        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        CreateEnemy(chosenData.prefab, spawnPoint.position, spawnPoint.rotation);
    }


    BaseEnemy CreateEnemy(BaseEnemy prefab, Vector3 position, Quaternion rotation)
    {
        // Use EnemyPool to spawn enemy
        BaseEnemy enemy = EnemyPool.Instance.GetFromPool(prefab);
        if (enemy == null)
        {
            // Could not spawn (maybe maxCount reached for this prefab)
            return null;
        }

        enemy.transform.position = position;
        enemy.transform.rotation = rotation;
        enemy.gameObject.SetActive(true);
        enemy.ResetEnemy();

        enemy.OnEnemyDeath += HandleEnemyDeath;
        return enemy;
    }

    void HandleEnemyDeath(BaseEnemy enemy)
    {
        EnemyPool.Instance.ReturnToPool(enemy);
        enemy.OnEnemyDeath -= HandleEnemyDeath;
    }
}

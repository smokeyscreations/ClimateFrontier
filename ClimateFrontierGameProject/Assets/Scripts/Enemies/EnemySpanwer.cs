using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public List<Transform> spawnPoints;
    public List<BaseEnemy> enemyPrefabs;
    public float spawnInterval = 2f;
    public int maxEnemies = 1;

    private float spawnTimer;
    private int currentEnemyCount = 0;

    void Start()
    {
        spawnTimer = spawnInterval;
    }

    void Update()
    {
        if (currentEnemyCount >= maxEnemies)
            return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            SpawnEnemy();
            spawnTimer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Count == 0 || enemyPrefabs.Count == 0)
            return;

        // Select a random spawn point and enemy type
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        BaseEnemy enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        // Use CreateEnemy() to fetch from pool instead of Instantiate()
        BaseEnemy newEnemy = CreateEnemy(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        // Subscribe to enemy's death event to decrement the enemy count
        newEnemy.OnEnemyDeath += HandleEnemyDeath;
    }

    BaseEnemy CreateEnemy(BaseEnemy prefab, Vector3 position, Quaternion rotation)
    {
        // Fetch from pool instead of instantiating
        BaseEnemy enemy = EnemyPool.Instance.GetFromPool(prefab);

        // Set position and rotation
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;

        // Activate the enemy
        enemy.gameObject.SetActive(true);

        // Reset the enemy's state after activation
        enemy.ResetEnemy();

        currentEnemyCount++;
        return enemy;
    }


    void HandleEnemyDeath(BaseEnemy enemy)
    {
        currentEnemyCount--;

        // Unsubscribe from the enemy's death event
        enemy.OnEnemyDeath -= HandleEnemyDeath;
    }
}

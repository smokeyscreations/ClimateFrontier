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


        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        BaseEnemy enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        BaseEnemy newEnemy = CreateEnemy(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    BaseEnemy CreateEnemy(BaseEnemy prefab, Vector3 position, Quaternion rotation)
    {

        BaseEnemy enemy = EnemyPool.Instance.GetFromPool(prefab);
        enemy.transform.position = position;
        enemy.transform.rotation = rotation;

        // Activate the enemy
        enemy.gameObject.SetActive(true);

        // Reset the enemy's state before activation
        enemy.ResetEnemy();

        // Subscribe to enemy's death event to decrement the enemy count
        enemy.OnEnemyDeath += HandleEnemyDeath;

        return enemy;
    }

    void HandleEnemyDeath(BaseEnemy enemy)
    {
        currentEnemyCount--;
        enemy.OnEnemyDeath -= HandleEnemyDeath;
    }
}

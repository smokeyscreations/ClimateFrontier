using UnityEngine;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    private Dictionary<string, Queue<BaseEnemy>> poolDictionary;

    void Awake()
    {
        Instance = this;
        poolDictionary = new Dictionary<string, Queue<BaseEnemy>>();
        Debug.Log("EnemyPool Instance has been initialized.");
    }

    public BaseEnemy GetFromPool(BaseEnemy prefab)
    {
        string key = prefab.gameObject.name;

        if (poolDictionary.ContainsKey(key) && poolDictionary[key].Count > 0)
        {
            BaseEnemy enemy = poolDictionary[key].Dequeue();
            enemy.gameObject.SetActive(true);
            return enemy;
        }
        else
        {
            // Instantiate new enemy if pool is empty
            BaseEnemy newEnemy = Instantiate(prefab);
            newEnemy.gameObject.name = key; // Ensure the name matches the key
            return newEnemy;
        }
    }

    public void ReturnToPool(BaseEnemy enemy)
    {
        string key = enemy.gameObject.name;

        if (!poolDictionary.ContainsKey(key))
        {
            poolDictionary[key] = new Queue<BaseEnemy>();
        }

        poolDictionary[key].Enqueue(enemy);
        enemy.gameObject.SetActive(false);
    }

}

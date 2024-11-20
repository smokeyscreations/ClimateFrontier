using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;             // Unique identifier for the pool
        public GameObject prefab;      // Prefab to instantiate
        public int size;               // Initial size of the pool
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize the pool dictionary
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        // Create the pools
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            // Instantiate the initial pool size
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.parent = this.transform; // Optional: organize pooled objects under the pooler
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary>
    /// Spawns an object from the specified pool.
    /// </summary>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            return null;
        }

        Queue<GameObject> objectPool = poolDictionary[tag];

        if (objectPool.Count == 0)
        {
            Debug.LogWarning($"No available objects in pool '{tag}'. Consider increasing pool size.");
            // Optionally, instantiate a new object if the pool is empty
            Pool pool = pools.Find(p => p.tag == tag);
            if (pool != null)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                obj.transform.parent = this.transform; // Optional
                objectPool.Enqueue(obj);
            }
            else
            {
                return null;
            }
        }

        GameObject objectToSpawn = objectPool.Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        // Call OnObjectSpawn if the object implements IPoolable
        IPoolable poolable = objectToSpawn.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnObjectSpawn();
        }

        return objectToSpawn;
    }

    /// <summary>
    /// Returns an object back to its pool.
    /// </summary>
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            Destroy(objectToReturn);
            return;
        }

        IPoolable poolable = objectToReturn.GetComponent<IPoolable>();
        if (poolable != null)
        {
            poolable.OnObjectReturn();
        }

        objectToReturn.SetActive(false);
        poolDictionary[tag].Enqueue(objectToReturn);

        Debug.Log($"[ObjectPooler] Object '{objectToReturn.name}' returned to pool '{tag}'. Pool size: {poolDictionary[tag].Count}");
    }

}

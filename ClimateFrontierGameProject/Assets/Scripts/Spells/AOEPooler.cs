using System.Collections.Generic;
using UnityEngine;

public class AOEPooler : MonoBehaviour
{
    public static AOEPooler Instance;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    /// <summary>
    /// Spawns an object from the specified pool at the given position and rotation.
    /// </summary>
    /// <param name="tag">Tag of the pool.</param>
    /// <param name="position">Spawn position.</param>
    /// <param name="rotation">Spawn rotation.</param>
    /// <returns>The spawned GameObject.</returns>
    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            return null;
        }

        if (poolDictionary[tag].Count == 0)
        {
            Debug.LogWarning($"No available objects in pool '{tag}'. Consider increasing pool size.");
            return null;
        }

        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        return objectToSpawn;
    }

    /// <summary>
    /// Returns an object back to its pool.
    /// </summary>
    /// <param name="tag">Tag of the pool.</param>
    /// <param name="objectToReturn">GameObject to return.</param>
    public void ReturnToPool(string tag, GameObject objectToReturn)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist.");
            return;
        }

        // Ensure the object belongs to the pool before enqueuing
        // This prevents accidental pooling of unintended objects
        Pool pool = pools.Find(p => p.tag == tag);
        if (pool == null || objectToReturn == null)
        {
            Debug.LogWarning($"Object or Pool '{tag}' is invalid.");
            return;
        }

        // Optionally, reset object state here if needed

        poolDictionary[tag].Enqueue(objectToReturn);
    }
}

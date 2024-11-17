using UnityEngine;
using System.Collections.Generic;

public class ExperienceOrbPool : MonoBehaviour
{
    public static ExperienceOrbPool Instance;

    public GameObject orbPrefab; // Assign the ExperienceOrb prefab in the Inspector
    public int initialPoolSize = 50; // Adjust based on expected usage

    private List<GameObject> orbPool;

    void Awake()
    {
        // Singleton pattern for easy access
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        // Initialize the pool
        orbPool = new List<GameObject>(initialPoolSize);
        for (int i = 0; i < initialPoolSize; i++)
        {
            CreateNewOrb();
        }
    }

    // Creates a new orb, adds it to the pool, and deactivates it
    private GameObject CreateNewOrb()
    {
        GameObject orb = Instantiate(orbPrefab);
        orb.SetActive(false);
        orbPool.Add(orb);
        return orb;
    }

    // Retrieves an orb from the pool or creates a new one if none are available
    public GameObject GetOrb()
    {
        foreach (var orb in orbPool)
        {
            if (!orb.activeInHierarchy)
            {
                orb.SetActive(true);
                return orb;
            }
        }
        // No available orb, create a new one
        return CreateNewOrb();
    }

    // Returns an orb to the pool
    public void ReturnOrb(GameObject orb)
    {
        // Reset the orb before deactivating it
        ExperienceOrb experienceOrb = orb.GetComponent<ExperienceOrb>();
        if (experienceOrb != null)
        {
            experienceOrb.ResetOrb();
        }
        orb.SetActive(false);
    }
}

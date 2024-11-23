using System.Collections.Generic;
using UnityEngine;

public class AudioPooler : MonoBehaviour
{
    public static AudioPooler Instance;

    [Header("Audio Pool Settings")]
    public AudioSource audioSourcePrefab; // Assign the PooledAudioSource prefab here
    public int poolSize = 20; // Adjust based on expected concurrent sounds

    private Queue<AudioSource> poolQueue = new Queue<AudioSource>();

    private void Awake()
    {
        // Singleton Pattern for easy access
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Initialize the pool with inactive AudioSources
    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource newSource = Instantiate(audioSourcePrefab, transform);
            newSource.gameObject.SetActive(false);
            poolQueue.Enqueue(newSource);
        }
    }

    // Fetch an available AudioSource from the pool
    public AudioSource GetAudioSource()
    {
        if (poolQueue.Count > 0)
        {
            AudioSource source = poolQueue.Dequeue();
            source.gameObject.SetActive(true);
            return source;
        }
        else
        {
            // Optional: Expand the pool if needed
            AudioSource newSource = Instantiate(audioSourcePrefab, transform);
            return newSource;
        }
    }

    // Return the AudioSource back to the pool
    public void ReturnAudioSource(AudioSource source)
    {
        source.Stop();
        source.gameObject.SetActive(false);
        poolQueue.Enqueue(source);
    }
}

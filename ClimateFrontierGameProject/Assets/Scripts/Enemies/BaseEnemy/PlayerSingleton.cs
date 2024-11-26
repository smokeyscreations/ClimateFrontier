// Assets/Scripts/Player/PlayerSingleton.cs
using UnityEngine;

public class PlayerSingleton : MonoBehaviour
{
    public static PlayerSingleton Instance { get; private set; }

    public Transform PlayerTransform { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("PlayerSingleton: Duplicate instance detected. Destroying duplicate.");
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
            PlayerTransform = this.transform;
            Debug.Log("PlayerSingleton: Instance set and PlayerTransform assigned.");
        }
    }
}

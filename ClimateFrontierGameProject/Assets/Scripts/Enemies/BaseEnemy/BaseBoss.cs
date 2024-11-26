using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;
public class BaseBoss : MonoBehaviour
{
    public Transform player;
    public GameInitializer gameInitializer;

    protected virtual void Awake()
    {
        // Initialization handled here if needed
    }

    protected virtual void Start()
    {
        // Find the TestInitializer in the scene
        gameInitializer = FindAnyObjectByType<GameInitializer>();
        if (gameInitializer != null)
        {
            // Subscribe to the OnPlayerInstantiated event
            gameInitializer.OnPlayerInstantiated += SetPlayer;

            // Check if the player is already instantiated
            GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
            if (existingPlayer != null)
            {
                SetPlayer(existingPlayer);
            }
            else
            {
                Debug.LogWarning("BaseBoss: Player not found yet. Waiting for OnPlayerInstantiated event.");
            }
        }
        else
        {
            Debug.LogError("BaseBoss: TestInitializer not found in the scene.");
        }
    }

    protected virtual void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        GameInitializer testInitializer = FindAnyObjectByType<GameInitializer>();
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated -= SetPlayer;
        }
    }

    /// <summary>
    /// Callback method invoked when the player is instantiated.
    /// </summary>
    /// <param name="playerObj">The instantiated player GameObject.</param>
    protected virtual void SetPlayer(GameObject playerObj)
    {
        player = playerObj.transform;

        Debug.Log("BaseBoss: Player reference set.");
    }
}

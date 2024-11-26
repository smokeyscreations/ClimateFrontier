// Assets/Scripts/Boss/BaseBoss.cs
using UnityEngine;
using BehaviorDesigner.Runtime;

public class BaseBoss : MonoBehaviour
{
    protected Transform player;
    protected TestInitializer testInitializer;

    [Header("Behavior Designer Shared Variables")]
    public SharedTransform sharedPlayerTransform; // Assign via Inspector

    protected virtual void Awake()
    {
        // Initialization handled here if needed
    }

    protected virtual void Start()
    {
        // Find the TestInitializer in the scene
        testInitializer = FindAnyObjectByType<TestInitializer>();
        if (testInitializer != null)
        {
            // Subscribe to the OnPlayerInstantiated event
            testInitializer.OnPlayerInstantiated += SetPlayer;

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
        if (testInitializer != null)
        {
            testInitializer.OnPlayerInstantiated -= SetPlayer;
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

        // Assign to the shared variable
        if (sharedPlayerTransform != null)
        {
            sharedPlayerTransform.Value = player;
            Debug.Log("BaseBoss: Shared playerTransform assigned.");
        }
        else
        {
            Debug.LogWarning("BaseBoss: Shared playerTransform is not assigned in the Inspector.");
        }
    }
}

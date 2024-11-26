using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class EnemyConditional : Conditional
{
    protected Rigidbody body;
    protected Animator animator;
    protected Transform target;
    protected NavMeshAgent agent;

    protected GameInitializer gameInitializer;

    public override void OnAwake()
    {
        body = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        // Find the TestInitializer in the scene
        gameInitializer = GameObject.FindObjectOfType<GameInitializer>();
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated += SetTarget;
            Debug.Log("EnemyConditional: Subscribed to OnPlayerInstantiated event.");
        }
        else
        {
            Debug.LogError("EnemyConditional: TestInitializer not found in the scene.");
        }
    }

    protected void OnDestroy()
    {
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated -= SetTarget;
            Debug.Log("EnemyConditional: Unsubscribed from OnPlayerInstantiated event.");
        }
    }

    private void SetTarget(GameObject player)
    {
        if (player != null)
        {
            target = player.transform;
            Debug.Log($"EnemyConditional: Target set to {player.name}");
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (target == null)
        {
            // Try to find the player if already instantiated
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                Debug.Log("EnemyConditional: Player found and target set.");
            }
            else
            {
                Debug.LogWarning("EnemyConditional: Player not found. Waiting for player to be instantiated.");
                return TaskStatus.Failure;
            }
        }

        if (target == null)
        {
            // Target not yet assigned. Depending on condition, return Running or Failure
            return TaskStatus.Failure;
        }

        // Base class does not implement specific condition
        return TaskStatus.Failure;
    }
}

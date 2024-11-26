using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class EnemyAction : Action
{
    protected Rigidbody body;
    protected Animator animator;
    public Transform target;
    protected NavMeshAgent agent;

    protected GameInitializer gameInitializer;

    public float stoppingDistance = 6f; // Distance at which the enemy stops chasing the player

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
            Debug.Log("EnemyAction: Subscribed to OnPlayerInstantiated event.");
        }
        else
        {
            Debug.LogError("EnemyAction: GameInitializer not found in the scene.");
        }
    }

    // Do NOT use 'override' here
    protected void OnDestroy()
    {
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated -= SetTarget;
            Debug.Log("EnemyAction: Unsubscribed from OnPlayerInstantiated event.");
        }
    }

    private void SetTarget(GameObject player)
    {
        if (player != null)
        {
            target = player.transform;
            Debug.Log($"EnemyAction: Target set to {player.name}");
        }
    }

    public override void OnStart()
    {
        if (target == null)
        {
            // Try to find the player if already instantiated
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                Debug.Log("EnemyAction: Player found and target set.");
            }
            else
            {
                Debug.LogWarning("EnemyAction: Player not found. Waiting for player to be instantiated.");
            }
        }

        if (target == null)
        {
            // Target not found yet. Optionally, stop the agent
            if (agent != null)
            {
                agent.isStopped = true;
                Debug.Log("EnemyAction: NavMeshAgent stopped due to no target.");
            }
            return;
        }

        if (agent == null)
        {
            Debug.LogError("EnemyAction: NavMeshAgent component not found on the enemy.");
        }
        else
        {
            agent.stoppingDistance = stoppingDistance; // Set the stopping distance for the NavMeshAgent
            agent.updateRotation = false; // Disable automatic rotation
            agent.isStopped = false; // Ensure the agent is active
            Debug.Log("EnemyAction: NavMeshAgent initialized and active.");
        }

        if (animator != null)
        {
            animator.SetBool("IsRunning", true); // Start running animation by default
            Debug.Log("EnemyAction: Set IsRunning to true.");
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (target == null)
        {
            // Target not yet assigned
            Debug.Log("EnemyAction: Target not assigned yet.");
            return TaskStatus.Running;
        }

        // Base class does not implement specific behavior
        return TaskStatus.Success;
    }
}

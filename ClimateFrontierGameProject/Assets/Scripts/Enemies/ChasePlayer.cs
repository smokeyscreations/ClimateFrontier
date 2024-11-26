using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;
using static UnityEngine.GraphicsBuffer;

public class ChasePlayer : EnemyAction
{
    public SharedTransform playerTransform; // Assign via Behavior Designer
    private float stoppingDistance = 6f;
    public float rotationSpeed = 720f; // Degrees per second
    [SerializeField] private float pathUpdateInterval = 0.2f;
    private float pathUpdateTimer;

    public override void OnStart()
    {
        base.OnStart();

        if (playerTransform.Value == null)
        {
            Debug.LogWarning("ChasePlayer: PlayerTransform is not assigned. Waiting for assignment.");
            if (agent != null)
            {
                agent.isStopped = true;
                animator.SetBool("IsRunning", false);
            }
            return;
        }

        target = playerTransform.Value;
        Debug.Log("ChasePlayer: Target set from SharedTransform.");

        if (animator != null)
        {
            animator.SetBool("IsRunning", true); // Start running animation
            Debug.Log("ChasePlayer: Set IsRunning to true.");
        }
    }

    public override TaskStatus OnUpdate()
    {
        if (target == null || agent == null)
        {
            Debug.LogError("ChasePlayer: Target or NavMeshAgent is missing.");
            return TaskStatus.Failure;
        }

        // Check the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        Debug.Log($"ChasePlayer: Distance to player = {distanceToPlayer}");

        if (distanceToPlayer <= stoppingDistance)
        {
            agent.isStopped = true; // Stop the NavMeshAgent
            if (animator != null)
            {
                // Do not set IsRunning to false here
                Debug.Log("ChasePlayer: Reached stopping distance. Preparing to JumpAttack.");
            }
            Debug.Log("ChasePlayer: Reached stopping distance. Returning Success.");
            return TaskStatus.Success; // Indicate that the task has completed successfully
        }

        // Continue chasing the player
        
        if(Time.time >= pathUpdateTimer)
        {
            pathUpdateTimer = Time.time + pathUpdateInterval;
            if (target != null)
                agent.SetDestination(target.position);
        }
        agent.isStopped = false;
        if (animator != null && !animator.GetBool("IsRunning"))
        {
            animator.SetBool("IsRunning", true); // Ensure running animation is active
            Debug.Log("ChasePlayer: Set IsRunning to true.");
        }
        Debug.Log("ChasePlayer: Chasing the player.");

        // Rotate to face the player
        RotateTowardsPlayer();

        return TaskStatus.Running; // Indicate that the task is ongoing
    }

    /// <summary>
    /// Rotates the enemy to face the player smoothly.
    /// </summary>
    private void RotateTowardsPlayer()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        direction.y = 0f; // Keep rotation on the horizontal plane

        if (direction == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}

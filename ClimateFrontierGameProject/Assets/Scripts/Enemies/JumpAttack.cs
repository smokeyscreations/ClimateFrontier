using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;
using BehaviorDesigner.Runtime;

public class JumpAttack : EnemyAction
{
    [Header("Jump Settings")]
    public float jumpSpeed = 10f;           // Speed of the jump
    public float attackDuration = 1.6f;     // Duration of the jump attack (aligned with animation length)
    public float rotationSpeed = 360f;      // Degrees per second for rotation
    public float rotationThreshold = 5f;    // Degrees within which rotation is considered complete

    private bool isRotating = false;        // Tracks if the enemy is rotating to face the player
    private bool isJumping = false;         // Tracks if the enemy is in the jump state
    private float attackTimer = 0f;         // Timer for attack completion
    private float originalSpeed = 6f;       // Original speed to reset after attack

    public override void OnStart()
    {
        base.OnStart();

        if (target == null)
        {
            Debug.LogWarning("JumpAttack: Target not set. Waiting for target to be assigned.");
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger("JumpAttack"); // Play jump attack animation on AttackLayer
            // Do NOT set IsRunning to false here
            Debug.Log("JumpAttack: JumpAttack trigger set.");
        }

        isRotating = true;    // Start by rotating towards the player
        isJumping = false;    // Not jumping yet
        attackTimer = 0f;     // Reset timer

        if (agent != null)
        {
            agent.isStopped = true; // Stop NavMeshAgent during rotation
            agent.speed = jumpSpeed; // Temporarily set agent speed to jumpSpeed
            Debug.Log("JumpAttack: NavMeshAgent stopped and speed set for jump.");
        }

        // Do NOT set IsRunning to false to keep Running animation active in Base Layer
    }

    public override TaskStatus OnUpdate()
    {
        if (target == null)
        {
            Debug.LogError("JumpAttack: Player target not assigned or found. Ensure the player has the correct tag.");
            return TaskStatus.Failure; // Fail the task if the target is missing
        }

        if (isRotating)
        {
            RotateTowardsPlayer();
            float angleToPlayer = Vector3.Angle(transform.forward, target.position - transform.position);


            if (angleToPlayer <= rotationThreshold)
            {
                isRotating = false;
                isJumping = true;
                Debug.Log("JumpAttack: Rotation complete. Starting jump.");

                // Set a temporary destination towards the player
                if (agent != null)
                {
                    agent.isStopped = false; // Resume NavMeshAgent
                    agent.SetDestination(target.position);
                    Debug.Log("JumpAttack: NavMeshAgent destination set to player.");
                }
            }

            return TaskStatus.Running;
        }

        if (isJumping)
        {
            attackTimer += Time.deltaTime;

            // Adjust NavMeshAgent's speed if necessary
            if (agent != null)
            {
                agent.speed = jumpSpeed; // Ensure agent speed is set
            }

            if (attackTimer >= attackDuration)
            {
                isJumping = false;
                if (animator != null)
                {
                    animator.ResetTrigger("JumpAttack"); // Reset the trigger to prevent looping
                    agent.Warp(transform.position);
                    Debug.Log("JumpAttack: JumpAttack trigger reset.");
                }
                Debug.Log("JumpAttack: Attack completed.");

                if (agent != null)
                {
                    agent.speed = originalSpeed; // Reset agent speed to original value
                    Debug.Log("JumpAttack: NavMeshAgent speed reset.");
                }

                return TaskStatus.Success; // Successfully completed the jump attack
            }

            return TaskStatus.Running;
        }

        // If not rotating or jumping, complete the task
        return TaskStatus.Success;
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

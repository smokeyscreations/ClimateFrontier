// Assets/Scripts/Boss/ClawAttack.cs
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class ClawAttack : EnemyAction
{
    [Header("Claw Attack Settings")]
    public float clawDuration = 1.5f;      // Duration of the claw attack
    public float clawCooldown = 3f;        // Cooldown period before the next claw attack

    private float attackTimer = 0f;        // Timer to track attack duration
    private bool isAttacking = false;      // Flag to indicate if the boss is currently attacking
    private float lastAttackTime = -Mathf.Infinity; // Timestamp of the last attack

    // Reference to the ClawAttackVFXHandler
    private ClawAttackVFXHandler clawVFXHandler;

    public override void OnStart()
    {
        base.OnStart();

        if (target == null)
        {
            Debug.LogWarning("ClawAttack: Target not set. Waiting for target to be assigned.");
            return;
        }

        // Initialize attack variables
        attackTimer = 0f;
        isAttacking = true;
        lastAttackTime = Time.time;

        // Trigger claw attack animation
        if (animator != null)
        {
            animator.SetTrigger("ClawAttack"); // Ensure you have a corresponding trigger in your Animator
            Debug.Log("ClawAttack: ClawAttack trigger set.");
        }

        // Stop the NavMeshAgent during the attack
        if (agent != null)
        {
            agent.isStopped = true;
            Debug.Log("ClawAttack: NavMeshAgent stopped.");
        }

        // Get the ClawAttackVFXHandler component
        clawVFXHandler = GetComponent<ClawAttackVFXHandler>();
        if (clawVFXHandler == null)
        {
            Debug.LogError("ClawAttack: ClawAttackVFXHandler component not found on the boss.");
        }
        // Note: LaunchClaw is now called via animation event
    }

    public override TaskStatus OnUpdate()
    {
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= clawDuration)
            {
                isAttacking = false;
                attackTimer = 0f;

                // Resume NavMeshAgent
                if (agent != null)
                {
                    agent.isStopped = false;
                    Debug.Log("ClawAttack: NavMeshAgent resumed.");
                }

                // Reset animation trigger
                if (animator != null)
                {
                    animator.ResetTrigger("ClawAttack");
                    Debug.Log("ClawAttack: ClawAttack trigger reset.");
                }

                // Finish the task successfully
                Debug.Log("ClawAttack: Claw attack completed.");
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        // Check for cooldown
        if (Time.time - lastAttackTime < clawCooldown)
        {
            Debug.Log($"ClawAttack: Cooldown active. Next attack in {clawCooldown - (Time.time - lastAttackTime):F2} seconds.");
            return TaskStatus.Failure;
        }

        return TaskStatus.Running;
    }
}

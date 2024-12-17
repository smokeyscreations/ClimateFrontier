// Assets/Scripts/Boss/BeamAttack.cs
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using UnityEngine.AI;

public class BeamAttack : EnemyAction
{
    [Header("Beam Attack Settings")]
    public float beamDuration = 2f;      // Duration of the beam attack
    public float beamCooldown = 5f;      // Cooldown period before the next beam attack

    private float attackTimer = 0f;      // Timer to track attack duration
    private bool isAttacking = false;    // Flag to indicate if the boss is currently attacking
    private float lastAttackTime = -Mathf.Infinity; // Timestamp of the last attack

    // Reference to the BeamAttackVFXHandler
    private BeamAttackVFXHandler beamVFXHandler;

    public override void OnStart()
    {
        base.OnStart();

        if (target == null)
        {
            Debug.LogWarning("BeamAttack: Target not set. Waiting for target to be assigned.");
            return;
        }

        // Fallback to find player by tag
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) target = playerObj.transform;

        if (target == null)
        {
            Debug.LogError("BeamAttack: Player is still null at OnStart. Cannot proceed.");
            return; // Prevent starting attack
        }

        // Initialize attack variables
        attackTimer = 0f;
        isAttacking = true;
        lastAttackTime = Time.time;

        // Trigger beam attack animation
        if (animator != null)
        {
            animator.SetTrigger("BeamAttack"); // Ensure you have a corresponding trigger in your Animator
            Debug.Log("BeamAttack: BeamAttack trigger set.");
        }

        // Stop the NavMeshAgent during the attack
        if (agent != null)
        {
            agent.isStopped = true;
            Debug.Log("BeamAttack: NavMeshAgent stopped.");
        }

        // Get the BeamAttackVFXHandler component
        beamVFXHandler = GetComponent<BeamAttackVFXHandler>();
        if (beamVFXHandler == null)
        {
            Debug.LogError("BeamAttack: BeamAttackVFXHandler component not found on the boss.");
        }
        // Note: LaunchBeam is now called via animation event
    }

    public override TaskStatus OnUpdate()
    {
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= beamDuration)
            {
                isAttacking = false;
                attackTimer = 0f;

                // Resume NavMeshAgent
                if (agent != null)
                {
                    agent.isStopped = false;
                    Debug.Log("BeamAttack: NavMeshAgent resumed.");
                }

                // Reset animation trigger
                if (animator != null)
                {
                    animator.ResetTrigger("BeamAttack");
                    Debug.Log("BeamAttack: BeamAttack trigger reset.");
                }

                // Finish the task successfully
                Debug.Log("BeamAttack: Beam attack completed.");
                return TaskStatus.Success;
            }

            return TaskStatus.Running;
        }

        // Check for cooldown
        if (Time.time - lastAttackTime < beamCooldown)
        {
            Debug.Log($"BeamAttack: Cooldown active. Next attack in {beamCooldown - (Time.time - lastAttackTime):F2} seconds.");
            return TaskStatus.Failure;
        }

        return TaskStatus.Running;
    }
}

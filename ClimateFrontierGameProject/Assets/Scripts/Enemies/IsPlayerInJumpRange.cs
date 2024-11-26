using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class IsPlayerInJumpRange : EnemyConditional
{
    public float jumpRange = 2f; // The range at which the jump attack is triggered

    public override TaskStatus OnUpdate()
    {
        if (target == null)
        {
            Debug.LogError("IsPlayerInJumpRange: Player target not assigned or found. Ensure the player has the correct tag.");
            return TaskStatus.Failure;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        Debug.Log($"IsPlayerInJumpRange: Distance to player = {distanceToPlayer}");

        if (distanceToPlayer <= jumpRange)
        {
            Debug.Log("IsPlayerInJumpRange: Player is within jump range.");
            return TaskStatus.Success;
        }

        Debug.Log("IsPlayerInJumpRange: Player is out of jump range.");
        return TaskStatus.Failure;
    }
}

using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class IsPlayerInChaseRange : EnemyConditional
{
    public float chaseRange = 10f; // The range at which the enemy starts chasing the player

    public override TaskStatus OnUpdate()
    {
        if (target == null)
        {
            Debug.LogError("IsPlayerInChaseRange: Player target not assigned or found. Ensure the player has the correct tag.");
            return TaskStatus.Failure;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        Debug.Log($"IsPlayerInChaseRange: Distance to player = {distanceToPlayer}");

        if (distanceToPlayer <= chaseRange)
        {
            Debug.Log("IsPlayerInChaseRange: Player is within chase range.");
            return TaskStatus.Success;
        }

        Debug.Log("IsPlayerInChaseRange: Player is out of chase range.");
        return TaskStatus.Failure;
    }
}

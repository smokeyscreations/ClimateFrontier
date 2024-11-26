// Assets/Scripts/Boss/IsPlayerInBeamRange.cs
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;

public class IsPlayerInClawRange : EnemyConditional
{
    [Header("Beam Range Settings")]
    public float clawRange = 10f; // Range within which the beam attack is triggered

    public override TaskStatus OnUpdate()
    {
        if (target == null)
        {
            Debug.LogError("IsPlayerInBeamRange: Player target not assigned or found. Ensure the player has the correct tag.");
            return TaskStatus.Failure;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, target.position);
        Debug.Log($"IsPlayerInBeamRange: Distance to player = {distanceToPlayer}");

        if (distanceToPlayer <= clawRange)
        {
            Debug.Log("IsPlayerInBeamRange: Player is within beam range.");
            return TaskStatus.Success;
        }

        Debug.Log("IsPlayerInBeamRange: Player is out of beam range.");
        return TaskStatus.Failure;
    }
}

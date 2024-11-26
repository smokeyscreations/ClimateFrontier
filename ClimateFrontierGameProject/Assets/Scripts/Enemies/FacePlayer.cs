// Assets/Scripts/Boss/FacePlayer.cs
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime;

public class FacePlayer : EnemyAction
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 720f;
    public float rotationThreshold = 5f;
    public bool lockYAxis = true;

    public override void OnStart()
    {
        base.OnStart(); // Ensure EnemyAction's OnStart is called
        Debug.Log("FacePlayer: OnStart called.");
    }

    public override TaskStatus OnUpdate()
    {
        if (target == null)
        {
            Debug.LogError("FacePlayer: Player Transform is not assigned.");
            return TaskStatus.Failure;
        }

        Vector3 direction = target.position - transform.position;
        if (lockYAxis)
        {
            direction.y = 0f; // Lock Y axis to rotate only on the horizontal plane
        }

        if (direction == Vector3.zero)
        {
            // Player is at the same position as the boss; no rotation needed
            Debug.Log("FacePlayer: Player is at the same position as the boss. Rotation not required.");
            return TaskStatus.Success;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        float angleDifference = Quaternion.Angle(transform.rotation, targetRotation);
        Debug.Log($"FacePlayer: Current angle difference = {angleDifference}");

        if (angleDifference <= rotationThreshold)
        {
            // Rotation is complete
            Debug.Log("FacePlayer: Rotation complete.");
            return TaskStatus.Success;
        }

        // Still rotating
        Debug.Log("FacePlayer: Rotating towards player.");
        return TaskStatus.Running;
    }
}

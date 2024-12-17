using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using System;

public class EnemyConditional : Conditional
{
    protected Transform target;
    protected GameInitializer gameInitializer;

    public override void OnAwake()
    {
        // Moved subscription logic to OnStart or OnEnable to ensure player is ready.
    }

    public override void OnStart()
    {
        Debug.Log("EnemyConditional: OnStart called.");

        // Try subscribing to the OnPlayerInstantiated event here
        gameInitializer = GameObject.FindAnyObjectByType<GameInitializer>();
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated += SetTarget;
            Debug.Log("EnemyConditional: Subscribed to OnPlayerInstantiated event in OnStart.");
        }
        else
        {
            Debug.LogError("EnemyConditional: GameInitializer not found in OnStart.");
        }

        // Also try to find the player immediately here, if already spawned
        TryFindPlayerByTag();
    }

    public override void OnEnd()
    {
        // Unsubscribe when the task ends
        if (gameInitializer != null)
        {
            gameInitializer.OnPlayerInstantiated -= SetTarget;
            Debug.Log("EnemyConditional: Unsubscribed from OnPlayerInstantiated event in OnEnd.");
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
            Debug.LogWarning("EnemyConditional: Target not assigned, attempting to find player by tag in OnUpdate.");
            if (!TryFindPlayerByTag())
            {
                Debug.LogError("EnemyConditional: Player target not assigned or found. Ensure the player has the correct tag.");
                return TaskStatus.Failure;
            }
        }

        // If you’re extending this class, return TaskStatus based on conditions
        return TaskStatus.Failure;
    }

    private bool TryFindPlayerByTag()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            target = playerObj.transform;
            Debug.Log("EnemyConditional: Player found by tag in OnUpdate.");
            return true;
        }
        return false;
    }
}

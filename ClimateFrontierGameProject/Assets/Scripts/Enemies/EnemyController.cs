using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform target; // Assign this to the player's transform in the Inspector
    private EnemyReferences enemyReferences;

    private float pathUpdateDeadline; //track when we can update the path
    private float attackRange;

    void Start()
    {
        enemyReferences = GetComponent<EnemyReferences>();
        attackRange = enemyReferences.agent.stoppingDistance;
    }

    void Update()
    {
        if (target != null)
        {
            // Check if the player is within attack range
            bool inRange = Vector3.Distance(transform.position, target.position) <= attackRange;
            enemyReferences.animator.SetBool("Attack", inRange);

            if (inRange)
            {
                // Continue facing the player while attacking
                LookAtTarget();
                enemyReferences.agent.isStopped = true; // Stop moving when in attack range
            }
            else
            {
                // Chase the player if not in attack range
                UpdatePath();
                enemyReferences.agent.isStopped = false;
            }

            // Update speed for blend tree animation control
            enemyReferences.animator.SetFloat("speed", enemyReferences.agent.velocity.magnitude);
        }
    }

    private void LookAtTarget()
    {
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0; // Keep the rotation on the horizontal plane
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 8f); // Increase the rotation speed for faster alignment
    }

    private void UpdatePath()
    {
        if (Time.time >= pathUpdateDeadline)
        {
            pathUpdateDeadline = Time.time + enemyReferences.pathUpdateDelay;
            enemyReferences.agent.SetDestination(target.position);
        }
    }
}

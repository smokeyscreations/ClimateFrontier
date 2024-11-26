using UnityEngine;

public class ClawAttackVFXHandler : BaseBoss
{
    [Header("Claw Attack Settings")]
    [Tooltip("Claw attack prefab tag defined in ObjectPooler.")]
    public string clawTag = "Claw"; // Ensure this matches the tag in ObjectPooler

    [Tooltip("Reference to the Claw Spawn Point on the boss.")]
    public Transform clawSpawnPoint; // Assign in the Inspector

    /// <summary>
    /// This method is called by the Animation Event during the ClawAttack animation.
    /// </summary>
    public void OnClawAttack()
    {
        Debug.Log("ClawAttackVFXHandler: OnClawAttack called.");
        LaunchClaw();
    }

    /// <summary>
    /// Spawns the Claw VFX from the spawn point.
    /// </summary>
    private void LaunchClaw()
    {
        Debug.Log("ClawAttackVFXHandler: LaunchClaw called.");

        // Validate claw tag
        if (string.IsNullOrEmpty(clawTag))
        {
            Debug.LogError("ClawAttackVFXHandler: Claw tag is not assigned.");
            return;
        }

        // Validate spawn point reference
        if (clawSpawnPoint == null)
        {
            Debug.LogError("ClawAttackVFXHandler: ClawSpawnPoint reference is missing.");
            return;
        }

        // Spawn position and rotation based on spawn point
        Vector3 spawnPosition = clawSpawnPoint.position;
        Quaternion spawnRotation = clawSpawnPoint.rotation;

        // Spawn the claw from the pool
        GameObject claw = ObjectPooler.Instance.SpawnFromPool(clawTag, spawnPosition, spawnRotation);

        if (claw != null)
        {
            Debug.Log("ClawAttackVFXHandler: Claw spawned successfully.");
            // Additional initialization if needed
        }
        else
        {
            Debug.LogError($"ClawAttackVFXHandler: Failed to spawn claw with tag '{clawTag}'.");
        }
    }

    /// <summary>
    /// Handles dealing damage when the claw attack is complete.
    /// </summary>
    private void DealClawDamage()
    {
        if (player == null)
        {
            Debug.LogError("ClawAttackVFXHandler: Player reference is missing. Cannot deal damage.");
            return;
        }

        // Implement damage logic here
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(50); // Example damage value
            Debug.Log("ClawAttackVFXHandler: Applied 50 damage to player.");
        }
        else
        {
            Debug.LogWarning("ClawAttackVFXHandler: PlayerHealth component not found on player.");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (clawSpawnPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(clawSpawnPoint.position, 0.5f);
        }
    }
}

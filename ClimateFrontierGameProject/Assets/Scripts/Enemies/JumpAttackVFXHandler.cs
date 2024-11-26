// Assets/Scripts/Boss/JumpAttackVFXHandler.cs
using UnityEngine;

public class JumpAttackVFXHandler : BaseBoss
{
    [Header("Projectile Settings")]
    [Tooltip("Tag defined in ObjectPooler for the SlashProjectile.")]
    public string projectileTag = "SlashProjectile";

    [Tooltip("Transform representing the ground spawn point.")]
    public Transform groundSpawnPoint;

    [Tooltip("Speed at which the projectile moves.")]
    public float projectileSpeed = 5f;

    /// <summary>
    /// This method is called by the Animation Event at the end of the JumpAttack animation.
    /// </summary>
    public void OnJumpAttackComplete()
    {
        Debug.Log("JumpAttackVFXHandler: OnJumpAttackComplete called.");
        LaunchSlashProjectile();
    }

    /// <summary>
    /// Spawns and initializes a SlashProjectile from the ground spawn point.
    /// </summary>
    private void LaunchSlashProjectile()
    {
        Debug.Log("JumpAttackVFXHandler: LaunchSlashProjectile called.");

        // Validate projectile tag
        if (string.IsNullOrEmpty(projectileTag))
        {
            Debug.LogError("JumpAttackVFXHandler: Projectile tag is not assigned.");
            return;
        }

        // Validate ground spawn point
        if (groundSpawnPoint == null)
        {
            Debug.LogError("JumpAttackVFXHandler: Ground Spawn Point is not assigned.");
            return;
        }

        // Validate player reference
        if (player == null)
        {
            Debug.LogError("JumpAttackVFXHandler: Player reference is missing.");
            return;
        }

        // Calculate direction towards player
        Vector3 shootDirection = (player.position - groundSpawnPoint.position).normalized;
        Debug.Log($"JumpAttackVFXHandler: Calculated shoot direction: {shootDirection}");

        // Spawn the projectile from the pool
        GameObject projectile = ObjectPooler.Instance.SpawnFromPool(projectileTag, groundSpawnPoint.position, Quaternion.LookRotation(shootDirection));

        if (projectile != null)
        {
            Debug.Log("JumpAttackVFXHandler: Projectile spawned successfully.");

            // Initialize the projectile's direction and speed
            SlashProjectile slashProjectile = projectile.GetComponent<SlashProjectile>();
            if (slashProjectile != null)
            {
                slashProjectile.Initialize(shootDirection);
                slashProjectile.speed = projectileSpeed;
                Debug.Log($"JumpAttackVFXHandler: Projectile initialized with speed {projectileSpeed}.");
            }
            else
            {
                Debug.LogError("JumpAttackVFXHandler: SlashProjectile script not found on the spawned projectile.");
            }
        }
        else
        {
            Debug.LogError($"JumpAttackVFXHandler: Failed to spawn projectile with tag '{projectileTag}'.");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundSpawnPoint != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundSpawnPoint.position, 0.5f);
        }
    }
}

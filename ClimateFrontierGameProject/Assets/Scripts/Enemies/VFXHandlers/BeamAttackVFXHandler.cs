// Assets/Scripts/Boss/BeamAttackVFXHandler.cs
using UnityEngine;

public class BeamAttackVFXHandler : BaseBoss
{
    [Header("Beam Settings")]
    [Tooltip("Beam prefab tag defined in ObjectPooler.")]
    public string beamTag = "Beam"; // Ensure this matches the tag in ObjectPooler

    [Tooltip("Vertical offset from the player's position.")]
    public float verticalOffset = 0f; // Adjust as needed to position the beam near the player's feet

    [Tooltip("Horizontal offset to prevent beam from spawning directly on the player.")]
    public float horizontalOffset = -15f; // Adjust to make beam spawning forgiving

    /// <summary>
    /// This method is called by the Animation Event at the start of the BeamAttack animation.
    /// </summary>
    public void OnBeamAttack()
    {
        Debug.Log("BeamAttackVFXHandler: OnBeamAttack called.");
        LaunchBeam();
        // Removed DealBeamDamage() from here
    }

    /// <summary>
    /// This method is called by the Animation Event when the beam attack is complete.
    /// </summary>
    public void OnBeamAttackComplete()
    {
        Debug.Log("BeamAttackVFXHandler: OnBeamAttackComplete called.");
        DealBeamDamage();
    }

    /// <summary>
    /// Spawns the Beam VFX near the player's position with specified offsets.
    /// </summary>
    private void LaunchBeam()
    {
        Debug.Log("BeamAttackVFXHandler: LaunchBeam called.");

        // Validate beam tag
        if (string.IsNullOrEmpty(beamTag))
        {
            Debug.LogError("BeamAttackVFXHandler: Beam tag is not assigned.");
            return;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;

            if (player == null)
            {
                Debug.LogError("BeamAttackVFXHandler: Player reference is missing and could not be found by tag.");
                return;
            }
        }

        // Calculate spawn position with forgiveness
        Vector3 spawnPosition = player.position + player.forward * horizontalOffset + Vector3.up * verticalOffset;
        Quaternion spawnRotation = Quaternion.Euler(0, 0, 0); // Adjust if necessary

        // Spawn the beam from the pool
        GameObject beam = ObjectPooler.Instance.SpawnFromPool(beamTag, spawnPosition, spawnRotation);

        if (beam != null)
        {
            Debug.Log("BeamAttackVFXHandler: Beam spawned successfully.");
            // Additional initialization if needed
        }
        else
        {
            Debug.LogError($"BeamAttackVFXHandler: Failed to spawn beam with tag '{beamTag}'.");
        }
    }

    /// <summary>
    /// Handles dealing damage when the beam attack is complete.
    /// </summary>
    private void DealBeamDamage()
    {
        if (player == null)
        {
            Debug.LogError("BeamAttackVFXHandler: Player reference is missing. Cannot deal damage.");
            return;
        }

        // Implement damage logic here
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(50); // Corrected to apply 50 damage
            Debug.Log("BeamAttackVFXHandler: Applied 50 damage to player.");
        }
        else
        {
            Debug.LogWarning("BeamAttackVFXHandler: PlayerHealth component not found on player.");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            Gizmos.color = Color.red;
            Vector3 spawnPosition = player.position + player.forward * horizontalOffset + Vector3.up * verticalOffset;
            Gizmos.DrawWireSphere(spawnPosition, 0.5f);
        }
    }
}

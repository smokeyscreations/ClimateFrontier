using UnityEngine;

[CreateAssetMenu(fileName = "FlickerStrikeData", menuName = "Spell Creation/Flicker Strike Data")]
public class FlickerStrikeData : SpellData
{
    public float teleportRange = 10f;       // Range to search for enemies
    public float attackInterval = 0.2f;     // Time between attacks
    public int maxTargets = 10;             // Maximum number of enemies to attack
    public float dashDuration = 0.1f;       // Duration of the dash movement
    public GameObject dashVFXPrefab;        // Reference to the dash VFX prefab
    public GameObject sliceVFXPrefab;

    public override bool CanCast(BasePlayer player)
    {
        Collider[] colliders = Physics.OverlapSphere(player.transform.position, teleportRange, player.EnemyLayerMask);
        return colliders.Length > 0;
    }
}

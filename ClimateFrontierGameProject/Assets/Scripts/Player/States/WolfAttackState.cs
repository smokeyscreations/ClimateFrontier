using UnityEngine;
using System.Collections.Generic;
using PlayerStates;

namespace PlayerStates
{
    public class WolfAttackState : PlayerAttackState
    {
        private float attackRange;
        private LayerMask enemyLayerMask;

        // The Object Pool tag for your WolfHomingRFXProjectile prefab
        private string projectileVFXTag = "WolfProjectile";
        private float projectileSpawnOffset = 1f;

        public WolfAttackState(AncientWarrior player, float range) : base(player)
        {
            attackRange = range;
            enemyLayerMask = player.characterData.enemyLayerMask;

            // If your CharacterData has a special projectile tag, you could do:
            // projectileVFXTag = player.characterData.basicAttackVFXTag;
            // projectileSpawnOffset = player.characterData.basicAttackVFXOffset;
        }

        public override void OnEnter()
        {
            Debug.Log("Entering WolfAttackState (Ranged)");
            player.animator.SetTrigger("Attack");

            AttemptRangedAttack();
        }

        private void AttemptRangedAttack()
        {
            Transform closestEnemy = FindClosestEnemy();
            if (closestEnemy == null)
            {
                Debug.Log("No enemy found in range. Ranged attack does nothing.");
                return;
            }

            // Spawn the projectile from the ObjectPooler
            Vector3 spawnPosition = player.transform.position + player.transform.forward * projectileSpawnOffset + Vector3.up * 1f;
            Quaternion spawnRotation = Quaternion.LookRotation(player.transform.forward, Vector3.up);

            // This call actually reuses or creates from the pool, rather than always instantiating:
            GameObject projectile = ObjectPooler.Instance.SpawnFromPool(projectileVFXTag, spawnPosition, spawnRotation);
            if (projectile == null)
            {
                Debug.LogWarning($"Failed to spawn projectile with tag '{projectileVFXTag}'.");
                return;
            }

            // Get our WolfHomingRFXProjectile script
            WolfHomingRFXProjectile projectileScript = projectile.GetComponent<WolfHomingRFXProjectile>();
            if (projectileScript != null)
            {
                // If your WolfHomingRFXProjectile has a method to set the tag, do it now:
                projectileScript.SetPoolTag(projectileVFXTag);

                // Assign the target
                projectileScript.Target = closestEnemy.gameObject;

                Debug.Log($"WolfAttackState: Assigned {closestEnemy.name} as target to WolfHomingRFXProjectile.");
            }
            else
            {
                Debug.LogWarning("WolfHomingRFXProjectile not found on projectile. The projectile won't track the target.");
            }
        }

        private Transform FindClosestEnemy()
        {
            Collider[] colliders = Physics.OverlapSphere(player.transform.position, attackRange, enemyLayerMask);
            Transform closest = null;
            float minDist = Mathf.Infinity;

            foreach (var col in colliders)
            {
                float dist = Vector3.Distance(player.transform.position, col.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = col.transform;
                }
            }
            return closest;
        }

        public override void Tick()
        {
            // No repeating logic needed for a single-shot ranged attack
        }

        public override void OnExit()
        {
            // Optional cleanup
        }
    }
}

using UnityEngine;

namespace PlayerStates
{
    public class QueenAttackState : PlayerAttackState
    {
        private float attackRange; // Attack range for detecting enemies
        private float attackAngle = 360f;
        private int maxColliders = 20; // Adjust this number based on expected maximum enemies
        private Collider[] hitColliders;

        private LayerMask enemyLayerMask; // Layer mask for filtering enemies
        private string slashVFXTag = "SlashVFX"; // Tag for the slash VFX in the ObjectPooler
        private float slashVFXOffset = 1f; // Distance in front of the player to spawn the VFX

        public QueenAttackState(AncientQueen player, float range) : base(player)
        {
            attackRange = range; // Assign the attack range from the player

            // Initialize the hitColliders array
            hitColliders = new Collider[maxColliders];

            // Get the enemy layer mask from the player
            enemyLayerMask = player.characterData.enemyLayerMask;

            // Get VFX settings from CharacterData
            slashVFXTag = player.characterData.basicAttackVFXTag;
            slashVFXOffset = player.characterData.basicAttackVFXOffset;
        }

        public override void OnEnter()
        {
            Debug.Log("Entering QueenAttackState");
            player.animator.SetTrigger("Attack"); // Trigger the Queen's attack animation

            // Spawn the slash VFX
            SpawnSlashVFX();
            // Perform the attack check
            AttemptAttack();
        }

        protected override internal void AttemptAttack()
        {
            // Use OverlapSphereNonAlloc to avoid allocations
            int numColliders = Physics.OverlapSphereNonAlloc(player.transform.position, attackRange, hitColliders, enemyLayerMask);

            for (int i = 0; i < numColliders; i++)
            {
                Collider collider = hitColliders[i];

                // Check if the collider has any IDamageable component
                if (collider.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    // Calculate the direction to the enemy
                    Vector3 directionToEnemy = (collider.transform.position - player.transform.position).normalized;

                    // Check if the enemy is within the cone angle in front of the player
                    float angleToEnemy = Vector3.Angle(player.transform.forward, directionToEnemy);
                    if (angleToEnemy <= attackAngle * 0.5f) // Divide by 2 because angle is spread equally on both sides
                    {
                        // Apply damage to the enemy using the updated BaseAttackDamage
                        damageable.TakeDamage(player.BaseAttackDamage);
                        Debug.Log($"{player.gameObject.name} attacks {collider.gameObject.name} for {player.BaseAttackDamage} damage.");
                    }
                }

                // Clear the collider reference to prevent holding onto it
                hitColliders[i] = null;
            }
        }

        private void SpawnSlashVFX()
        {
            // Define the vertical offset
            float verticalOffset = 1.0f; // Adjust this value as needed

            // Calculate the position in front of the player with an upward offset
            Vector3 spawnPosition = player.transform.position
                                    + player.transform.forward * slashVFXOffset
                                    + Vector3.up * verticalOffset;

            // Align the VFX with the player's rotation
            Quaternion spawnRotation = Quaternion.LookRotation(player.transform.forward, Vector3.up);

            // Spawn the VFX from the pool
            GameObject vfxObject = ObjectPooler.Instance.SpawnFromPool(slashVFXTag, spawnPosition, spawnRotation);
            if (vfxObject != null)
            {
                IPoolable poolable = vfxObject.GetComponent<IPoolable>();
                if (poolable != null)
                {
                    poolable.OnObjectSpawn();
                }
            }
            else
            {
                Debug.LogWarning($"Failed to spawn VFX with tag '{slashVFXTag}'.");
            }
        }

        public override void Tick()
        {
            // Any additional logic for continuous checks, if needed
        }

        public override void OnExit()
        {
            // Clean up, if necessary
        }
    }
}

using UnityEngine;

namespace PlayerStates
{
    public class QueenAttackState : PlayerAttackState
    {
        private float attackRange; // Attack range for detecting enemies
        private float attackAngle = 90f;
        private int maxColliders = 20; // Adjust this number based on expected maximum enemies
        private Collider[] hitColliders;

        private LayerMask enemyLayerMask; // Layer mask for filtering enemies

        public QueenAttackState(AncientQueen player, float range) : base(player)
        {
            attackRange = range; // Assign the attack range from the player

            // Initialize the hitColliders array
            hitColliders = new Collider[maxColliders];

            // Get the enemy layer mask from the player
            enemyLayerMask = player.EnemyLayerMask;
        }

        public override void OnEnter()
        {
            Debug.Log("Entering QueenAttackState");
            player.animator.SetTrigger("Attack"); // Trigger the Queen's attack animation

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

                // Check if the collider belongs to an enemy
                if (collider.TryGetComponent<BaseEnemy>(out BaseEnemy enemy))
                {
                    // Calculate the direction to the enemy
                    Vector3 directionToEnemy = (enemy.transform.position - player.transform.position).normalized;

                    // Check if the enemy is within the cone angle in front of the player
                    float angleToEnemy = Vector3.Angle(player.transform.forward, directionToEnemy);
                    if (angleToEnemy <= attackAngle * 0.5f) // Divide by 2 because angle is spread equally on both sides
                    {
                        // Apply damage to the enemy
                        enemy.TakeDamage(player.BaseAttackDamage);
                        Debug.Log($"Enemy {enemy.gameObject.name} hit within cone range. Dealt damage: {player.BaseAttackDamage}");
                    }
                }

                // Clear the collider reference to prevent holding onto it
                hitColliders[i] = null;
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

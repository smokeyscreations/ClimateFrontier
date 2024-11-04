using UnityEngine;

namespace PlayerStates
{
    public class PlayerAttackState : IState
    {
        protected BasePlayer player;
        private float attackCooldown = 1.0f; // Example cooldown duration
        private float lastAttackTime = -1.0f;

        public PlayerAttackState(BasePlayer player)
        {
            this.player = player;
        }

        public virtual void Tick()
        {
            // Cooldown check to allow subsequent attacks
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                AttemptAttack();
            }
        }

        public virtual void OnEnter()
        {
            Debug.Log("Entered Player Attack State");
            AttemptAttack(); // Perform an initial attack on entering the state
        }

        protected virtual internal void AttemptAttack()
        {
            //if (Vector3.Distance(player.transform.position, player.Target.position) <= player.AttackRange)
            //{
            //    player.animator.SetTrigger("Attack");
            //    player.BaseAttack();
            //    lastAttackTime = Time.time;
            //}
        }

        public virtual void OnExit() { }
    }
}

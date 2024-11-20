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
          
        }

        public virtual void OnEnter()
        {

        }

        protected virtual internal void AttemptAttack()
        {
            
        }

        public virtual void OnExit() { }
    }
}

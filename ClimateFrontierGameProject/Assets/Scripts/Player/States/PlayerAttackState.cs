using UnityEngine;

namespace PlayerStates
{
    public class PlayerAttackState : IState
    {
        protected BasePlayer player;

        public PlayerAttackState(BasePlayer player)
        {
            this.player = player;
        }

        public virtual void Tick()
        {
            Debug.Log("Ticking Player Attack State");
        }

        public virtual void OnEnter()
        {
            // Trigger attack animation and logic
            player.animator.SetTrigger("Attack");
            player.BaseAttack();

            Debug.Log("Entered player Attack state");

        }

        public virtual void OnExit()
        {

        }
    }
}
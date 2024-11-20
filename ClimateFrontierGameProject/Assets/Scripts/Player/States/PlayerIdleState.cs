using UnityEngine;

namespace PlayerStates
{
    public class PlayerIdleState : IState
    {
        protected BasePlayer player;

        public PlayerIdleState(BasePlayer player)
        {
            this.player = player;
        }

        public void Tick()
        {
            // Remove Animator updates here
            // Keep any state-specific logic if needed
        }

        public void OnEnter()
        {
            // Debug.Log("Entered Player Idle State");
        }

        public void OnExit()
        {
            // Any cleanup if necessary
        }
    }
}

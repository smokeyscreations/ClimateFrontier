using UnityEngine;

namespace PlayerStates
{
    public class PlayerRunState : IState
    {
        protected BasePlayer player;

        public PlayerRunState(BasePlayer player)
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
            // Debug.Log("Entered Player Run State");
        }

        public void OnExit()
        {
            // Any cleanup if necessary
        }
    }
}

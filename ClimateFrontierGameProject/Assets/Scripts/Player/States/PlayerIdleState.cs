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
            // Set speed to 0 to ensure the Animator stays in Idle state
            player.animator.SetFloat("speed", 0f);
        }

        public void OnEnter()
        {
            Debug.Log("Entered player Idle state");
        }

        public void OnExit()
        {

        }
    }
}
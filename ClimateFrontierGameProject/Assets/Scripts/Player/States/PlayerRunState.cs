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
            // Calculate the movement speed based on input magnitude
            float speedValue = player.MovementInput.magnitude * player.MovementSpeed;
            player.animator.SetFloat("speed", speedValue);

        }

        public void OnEnter()
        {
            Debug.Log("Entered Player Run State");
        }

        public void OnExit()
        {

            // Optionally reset the speed parameter
            player.animator.SetFloat("speed", 0f);
        }
    }
}
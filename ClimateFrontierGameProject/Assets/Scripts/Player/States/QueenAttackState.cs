
using UnityEngine;

namespace PlayerStates
{
    public class QueenAttackState : PlayerAttackState


    {
        public QueenAttackState(AncientQueen player) : base(player) {

            this.player = player;
        }

        public override void Tick()
        {
            Debug.Log("Ticking Queen Attack State");
        }
        public override void OnEnter()
        {
            Debug.Log("Entering from QueenAttackState");
            player.animator.SetTrigger("QueenAttack");
            player.BaseAttack();
        }
    }
}
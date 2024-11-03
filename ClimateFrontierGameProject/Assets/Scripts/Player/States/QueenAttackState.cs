namespace PlayerStates
{
    public class QueenAttackState : PlayerAttackState


    {
        public QueenAttackState(BasePlayer player) : base(player) { }

        public override void OnEnter()
        {
            // Custom attack logic for the AncientQueen
            player.animator.SetTrigger("QueenAttack");
            player.BaseAttack();
        }
    }
}
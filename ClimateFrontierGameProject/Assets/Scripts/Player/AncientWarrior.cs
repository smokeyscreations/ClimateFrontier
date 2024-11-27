using UnityEngine;
using PlayerStates;

public class AncientWarrior : BasePlayer
{

    protected override void InitializeStateMachine()
    {
        base.InitializeStateMachine();

        // Override the attackState with QueenAttackState and set up transitions
        attackState = new WolfAttackState(this, characterData.attackRange);
        Debug.Log("attackState type at init: " + attackState.GetType());

        // Update transitions for QueenAttackState
        stateMachine.AddTransition(idleState, attackState, IsAttacking);
        stateMachine.AddTransition(runState, attackState, IsAttacking);

        // Transition back to idle or run state after attack is completed
        stateMachine.AddTransition(attackState, idleState, () => !IsAttacking() && MovementInput == Vector3.zero);
        stateMachine.AddTransition(attackState, runState, () => !IsAttacking() && MovementInput != Vector3.zero);
    }


}
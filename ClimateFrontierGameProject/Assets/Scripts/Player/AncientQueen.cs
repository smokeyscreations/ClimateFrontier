using UnityEngine;
using PlayerStates;

public class AncientQueen : BasePlayer
{
    [Header("Queen Abilities")]
    [SerializeField] private float ability1Damage = 20f;
    [SerializeField] private float ability2Damage = 25f;
    [SerializeField] private float ability3Damage = 30f;

    protected override void InitializeStateMachine()
    {
        base.InitializeStateMachine();

        // Override the attackState with QueenAttackState and set up transitions
        attackState = new QueenAttackState(this, characterData.attackRange);
        Debug.Log("attackState type at init: " + attackState.GetType());

        // Update transitions for QueenAttackState
        stateMachine.AddTransition(idleState, attackState, IsAttacking);
        stateMachine.AddTransition(runState, attackState, IsAttacking);
        stateMachine.AddTransition(attackState, idleState, () => !IsAttacking());
    }

    public override void UseAbility(int abilityIndex)
    {
        switch (abilityIndex)
        {
            case 1:
                Ability1();
                break;
            case 2:
                Ability2();
                break;
            case 3:
                Ability3();
                break;
            default:
                Debug.LogWarning("Invalid ability index");
                break;
        }
    }

    private void Ability1()
    {
        // Implement the ability logic using characterData
        Debug.Log($"Queen uses Ability 1 with damage {characterData.ability1Damage}!");
        // Add actual ability functionality here
    }

    private void Ability2()
    {
        Debug.Log($"Queen uses Ability 2 with damage {characterData.ability2Damage}!");
        // Add actual ability functionality here
    }

    private void Ability3()
    {
        Debug.Log($"Queen uses Ability 3 with damage {characterData.ability3Damage}!");
        // Add actual ability functionality here
    }
}

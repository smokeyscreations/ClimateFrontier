using UnityEngine;

public class AncientQueen : BasePlayer
{
    [Header("Queen Abilities")]
    [SerializeField] private float ability1Damage = 20f;
    [SerializeField] private float ability2Damage = 25f;
    [SerializeField] private float ability3Damage = 30f;

    protected override void InitializeStateMachine()
    {
        // Call base method to set up default states and transitions
        base.InitializeStateMachine();

        // Replace the attackState with QueenAttackState
        attackState = new QueenAttackState(this);

        // Update the transitions that involve attackState
        stateMachine.AddTransition(idleState, attackState, IsAttacking);
        stateMachine.AddTransition(runState, attackState, IsAttacking);
        stateMachine.AddTransition(attackState, idleState, () => !IsAttacking());
    }

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        healthSystem = GetComponent<PlayerHealth>();
        healthSystem.Initialize(maxHealth);

        InitializeStateMachine();
    }
    public override void BaseAttack()
    {
        Debug.Log("Queen performs a melee attack!");
        // Implement melee attack logic, such as detecting enemies in range and applying damage
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
        Debug.Log("Queen uses Ability 1!");
        // Implement ability 1 logic
    }

    private void Ability2()
    {
        Debug.Log("Queen uses Ability 2!");
        // Implement ability 2 logic
    }

    private void Ability3()
    {
        Debug.Log("Queen uses Ability 3!");
        // Implement ability 3 logic
    }
}

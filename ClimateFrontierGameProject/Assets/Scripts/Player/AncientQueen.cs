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
        InitializeAttributes();
        // Override the attackState with QueenAttackState and set up transitions
        attackState = new QueenAttackState(this, AttackRange);
        Debug.Log("attackState type at init: " + attackState.GetType());

        // Update transitions for QueenAttackState
        stateMachine.AddTransition(idleState, attackState, IsAttacking);
        stateMachine.AddTransition(runState, attackState, IsAttacking);
        stateMachine.AddTransition(attackState, idleState, () => !IsAttacking());
    }

    //public override void BaseAttack()
    //{
    //    if (Time.time - LastAttackTime < AttackCooldown)
    //        return; // Skip if cooldown hasn't elapsed

    //    LastAttackTime = Time.time;
    //    int numHit = Physics.OverlapSphereNonAlloc(transform.position, AttackRange, HitEnemies, EnemyLayerMask);
    //    for (int i = 0; i < numHit; i++)
    //    {
    //        if (HitEnemies[i].TryGetComponent<BaseEnemy>(out BaseEnemy enemyComponent))
    //        {
    //            enemyComponent.TakeDamage(baseAttackDamage);
    //        }
    //    }
    //    Debug.Log("Queen performs a melee attack!");
    //}

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
    public void InitializeAttributes()
    {
        AttackRange = 3f;
    }
    private void Ability1() => Debug.Log("Queen uses Ability 1!");
    private void Ability2() => Debug.Log("Queen uses Ability 2!");
    private void Ability3() => Debug.Log("Queen uses Ability 3!");
}

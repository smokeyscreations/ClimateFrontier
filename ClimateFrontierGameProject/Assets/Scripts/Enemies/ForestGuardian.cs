using System;
using UnityEngine;

public class ForestGuardian : BaseEnemy
{
    [SerializeField] private float detectionRadius = 10f;
    private StateMachine _stateMachine;

    protected override void Awake()
    {
        base.Awake();
        InitializeAttributes();
    }

    protected override void Start()
    {
        base.Start();
        InitializeStateMachine();
    }

    public override void PerformAttack()
    {
        base.PerformAttack(); // Call base to apply default attack damage

        // Additional behavior, e.g., particle effects or sound effects
        Debug.Log("ForestGuardian performs a special attack!");
    }

    private void InitializeAttributes(float difficultyFactor = 1.0f)
    {
        // Dynamically adjust attributes based on a given difficulty factor (default is 1)
        AttackRange = Mathf.Lerp(1.5f, 3f, difficultyFactor);
        MaxHealth = Mathf.Lerp(100f, 200f, difficultyFactor);
        BaseAttackDamage = Mathf.Lerp(10f, 20f, difficultyFactor);
        RotationSpeed = Mathf.Lerp(5f, 10f, difficultyFactor);

        // Update NavMeshAgent speed to a dynamic value if needed
        UpdateMovementSpeed(Mathf.Lerp(3f, 6f, difficultyFactor));
    }

    private void InitializeStateMachine()
    {
        _stateMachine = new StateMachine();

        // Define states
        var idleState = new IdleState(this, animator);
        var chaseState = new ChaseState(this, target, animator);
        var attackState = new AttackState(this, target, animator);

        // Set the initial state to idle
        _stateMachine.SetState(idleState);

        // Define transitions
        At(idleState, chaseState, PlayerInDetectionRadius());    // Transition from Idle to Chase when player is detected
        At(chaseState, idleState, PlayerOutOfDetectionRadius()); // Transition from Chase to Idle when player moves away
        At(chaseState, attackState, PlayerInAttackRange());      // Transition from Chase to Attack when player is in attack range
        At(attackState, chaseState, PlayerOutOfAttackRange());   // Transition back to Chase if player moves out of attack range
    }

    protected override void Update()
    {
        base.Update();
        _stateMachine.Tick();
    }

    // Helper method to define transitions
    private void At(IState from, IState to, Func<bool> condition)
    {
        _stateMachine.AddTransition(from, to, condition);
    }

    // Condition for entering ChaseState when the player is within the detection radius
    private Func<bool> PlayerInDetectionRadius()
    {
        return () => Vector3.Distance(transform.position, target.position) <= detectionRadius;
    }

    // Condition for returning to IdleState when the player exits the detection radius
    private Func<bool> PlayerOutOfDetectionRadius()
    {
        return () => Vector3.Distance(transform.position, target.position) > detectionRadius;
    }

    // Condition for entering AttackState when the player is within attack range
    private Func<bool> PlayerInAttackRange()
    {
        return () => Vector3.Distance(transform.position, target.position) <= AttackRange;
    }

    // Condition for returning to ChaseState when the player exits attack range
    private Func<bool> PlayerOutOfAttackRange()
    {
        return () => Vector3.Distance(transform.position, target.position) > AttackRange;
    }
}

using UnityEngine;

internal class IdleState : IState
{
    private readonly BaseEnemy _enemy;
    private readonly Animator _animator;
    private static readonly int IdleHash = Animator.StringToHash("Idle");

    public IdleState(BaseEnemy enemy, Animator animator)
    {
        _enemy = enemy;
        _animator = animator;
    }

    public void OnEnter()
    {
        _enemy.StopMovement();          // Stop the enemy's movement
        _animator.SetTrigger(IdleHash); // Set the idle animation
    }

    public void Tick()
    {
        // No behavior in Tick() for IdleState, as transitions are handled in the main class
    }

    public void OnExit()
    {
        // No specific cleanup needed for exiting idle state
    }
}

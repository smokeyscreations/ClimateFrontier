using UnityEngine;

internal class AttackState : IState
{
    private readonly BaseEnemy _enemy;
    private readonly Transform _target;
    private readonly Animator _animator;
    private static readonly int AttackHash = Animator.StringToHash("Attack");

    public AttackState(BaseEnemy enemy, Transform target, Animator animator)
    {
        _enemy = enemy;
        _target = target;
        _animator = animator;
    }

    public void OnEnter()
    {
        // Trigger the attack animation
        _animator.SetTrigger(AttackHash);

        // Stop movement to focus on attacking
        _enemy.navMeshAgent.isStopped = true;
    }

    public void Tick()
    {
        if (_target == null) return;

        // Keep the enemy facing the target
        _enemy.LookAtTarget();
    }

    public void OnExit()
    {
        // Reset the Attack trigger to allow future attacks
        _animator.ResetTrigger(AttackHash);
    }

    // Method to be called by the Animation Event at the contact point of the attack
    public void PerformAttack()
    {
        // Call the enemy's PerformAttack method to apply damage
        _enemy.PerformAttack();
    }
}

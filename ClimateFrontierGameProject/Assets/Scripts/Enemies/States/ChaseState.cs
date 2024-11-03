using UnityEngine;

internal class ChaseState : IState
{
    private readonly BaseEnemy _enemy;
    private readonly Transform _target;
    private readonly Animator _animator;
    private static readonly int ChaseHash = Animator.StringToHash("Run");

    public ChaseState(BaseEnemy enemy, Transform target, Animator animator)
    {
        _enemy = enemy;
        _target = target;
        _animator = animator;
    }

    public void OnEnter()
    {
        // Set the chase animation trigger
        _animator.SetTrigger(ChaseHash);
        _enemy.navMeshAgent.isStopped = false;
    }

    public void Tick()
    {
        // Chase logic, such as updating the NavMeshAgent destination
        _enemy.navMeshAgent.SetDestination(_target.position);
    }

    public void OnExit()
    {
        // Reset the chase trigger when exiting the state
        _animator.ResetTrigger(ChaseHash);
        _enemy.navMeshAgent.isStopped = true;
    }
}

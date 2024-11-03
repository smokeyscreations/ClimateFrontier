using UnityEngine;

namespace EnemyStates
{
    public class EnemyAttackState : IState
    {
        private readonly BaseEnemy _enemy;
        private readonly Animator _animator;
        private static readonly int AttackHash = Animator.StringToHash("Attack");

        public EnemyAttackState(BaseEnemy enemy, Animator animator)
        {
            _enemy = enemy;
            _animator = animator;
        }

        public void OnEnter()
        {
            _enemy.navMeshAgent.isStopped = true;
            _animator.SetTrigger(AttackHash);
        }

        public void Tick()
        {
            if (_enemy.Target == null) return;

            _enemy.LookAtTarget();
            _enemy.PerformAttack();
        }

        public void OnExit()
        {
            _animator.ResetTrigger(AttackHash);
        }
    }
}

using UnityEngine;
using UnityEngine.AI;

namespace EnemyStates
{
    public class EnemyChaseState : IState
    {
        private readonly BaseEnemy _enemy;
        private readonly NavMeshAgent _navMeshAgent;
        private readonly Animator _animator;

        public EnemyChaseState(BaseEnemy enemy, NavMeshAgent navMeshAgent, Animator animator)
        {
            _enemy = enemy;
            _navMeshAgent = navMeshAgent;
            _animator = animator;
        }

        public void OnEnter()
        {
            _navMeshAgent.isStopped = false;
            //Debug.Log($"{_enemy.gameObject.name}: Entered ChaseState");
        }

        public void Tick()
        {
            if (_enemy.Target == null) return;

            _enemy.MoveToTarget();
            float speed = _navMeshAgent.velocity.magnitude;
            _animator.SetFloat("speed", speed);
            _enemy.LookAtTarget();
        }

        public void OnExit()
        {
            _navMeshAgent.isStopped = true;
            _animator.SetFloat("speed", 0f);
            //Debug.Log($"{_enemy.gameObject.name}: Exited ChaseState");
        }
    }
}

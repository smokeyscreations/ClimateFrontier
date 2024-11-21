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
            if (_navMeshAgent.isActiveAndEnabled)
            {
                if (_navMeshAgent.isOnNavMesh)
                {
                    _navMeshAgent.isStopped = false;
                }
                else
                {
                    // Attempt to place the agent on the NavMesh
                    if (!_navMeshAgent.Warp(_enemy.transform.position))
                    {
                        Debug.LogError($"{_enemy.gameObject.name}: Failed to place NavMeshAgent on NavMesh at position {_enemy.transform.position}");
                    }
                    else
                    {
                        _navMeshAgent.isStopped = false;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"{_enemy.gameObject.name}: NavMeshAgent is not active and enabled.");
            }
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

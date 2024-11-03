using UnityEngine;

namespace EnemyStates
{
    public class EnemyIdleState : IState
    {
        private readonly BaseEnemy _enemy;
        private readonly Animator _animator;
        private static readonly int SpeedHash = Animator.StringToHash("speed");

        public EnemyIdleState(BaseEnemy enemy, Animator animator)
        {
            _enemy = enemy;
            _animator = animator;
        }

        public void OnEnter()
        {
            Debug.Log($"{_enemy.gameObject.name}: Entered EnemyIdleState");
        }

        public void Tick()
        {
            Debug.Log($"{_enemy.gameObject.name}: Ticking EnemyIdleState");
        }

        public void OnExit()
        {
            Debug.Log($"{_enemy.gameObject.name}: Exited EnemyIdleState");
        }
    }
}

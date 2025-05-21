using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;
using Utils;

namespace BehaviourTree
{
    [System.Serializable]
    public class CarbonAttackConfig
    {
        public float attackSpeed;
    }
    public class CarbonAttackStrategy : IStrategy
    {
        private static readonly int Attack = Animator.StringToHash("Attack");
        private Rigidbody _rb;
        private Transform _enemy;
        private Transform _player;
        private Rigidbody _playerRb;
        private Animator _animator;
        private CarbonAttackConfig _config;
        
        private Vector3 _target;

        public CarbonAttackStrategy(Enemy.Enemy enemy, Animator animator, CarbonAttackConfig config)
        {
            _config = config;
            _enemy = enemy.transform;
            _animator = animator;
            _rb = enemy.Rigidbody;
            _player = GameManager.Instance.Player.transform;
            _playerRb = GameManager.Instance.Player.Config.rigidbody;
        }

        public void Start()
        {
            _animator.SetTrigger(Attack);
            _rb.linearVelocity = Vector3.zero;
        }

        public Node.Status FixedProcess()
        {
            if (!_animator.GetCurrentAnimatorStateInfo(0).IsName("A_Attack"))
            {
                Vector3 playerVelocity = _playerRb.linearVelocity;
                const float predictionTime = 0.2f; 
                Vector3 predictedPosition = _player.position + playerVelocity * predictionTime;

                _target = new Vector3(predictedPosition.x, _enemy.position.y, predictedPosition.z);

                return Node.Status.Running;
            }

            // Move towards predicted target
            Vector3 newPosition = Vector3.MoveTowards(_enemy.position, _target, _config.attackSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(newPosition);

            return Utilities.RadialCheck(_enemy.position, _target, 0.05f) ? Node.Status.Success : Node.Status.Running;
        }

    }
}
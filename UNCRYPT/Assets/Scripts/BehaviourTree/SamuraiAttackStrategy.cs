using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;
using UnityEngine.Serialization;

namespace BehaviourTree
{
    [System.Serializable]
    public class SamuraiAttackConfig{
        [Header("Attack Settings")] 
        public float attackImpulseDuration;
        public float cooldown;
        public float dashVelocity;
    }
    public class SamuraiAttackStrategy : IStrategy
    {
        private Rigidbody _rb;
        private Transform _enemy;
        private Transform _player;
        private SamuraiAttackConfig _config;

        private Vector3 _direction;
        private float _elapsedTime;

        public SamuraiAttackStrategy(Enemy.Enemy enemy, SamuraiAttackConfig config)
        {
            _config = config;
            _enemy = enemy.transform;
            _rb = enemy.Rigidbody;
            _player = GameManager.Instance.Player.transform;
        }

        public void Start()
        {
            _direction = _player.position - _enemy.position;
            _direction.Normalize();
            _elapsedTime = 0;
        }

        public Node.Status FixedProcess()
        {
            if (_elapsedTime > _config.attackImpulseDuration)
            {
                _rb.linearVelocity = Vector3.zero;

                return Node.Status.Success;
            }
            
            _elapsedTime += Time.fixedDeltaTime;
            _rb.linearVelocity = _config.dashVelocity * _direction;
            return Node.Status.Running;
        }
    }
}
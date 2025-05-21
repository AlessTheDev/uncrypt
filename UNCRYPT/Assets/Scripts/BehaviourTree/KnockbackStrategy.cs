using System;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;

namespace BehaviourTree
{
    [Serializable]
    public class KnockBackConfig{
        [Header("KnockBack Settings")] 
        public float initialKnockbackForce;
        public  float knockbackDecayRate; // Rate at which knockback slows (higher = faster decay)
        public  float knockbackMinForce; // Minimum force before stopping knockback
    }
    
    public class KnockbackStrategy : IStrategy
    {
        private Vector3 _knockbackVelocity;
        private Rigidbody _rb;
        private Transform _enemy;
        private Transform _player;
        private KnockBackConfig _config;

        public KnockbackStrategy(Enemy.Enemy enemy, KnockBackConfig knockBackConfig)
        {
            _enemy = enemy.transform;
            _rb = enemy.Rigidbody;
            _config = knockBackConfig;
            _player = GameManager.Instance.Player.transform;
        }
        public Node.Status FixedProcess()
        {
            // Apply knockback and gradually reduce it
            _rb.linearVelocity = _knockbackVelocity;

            // Gradual slowdown using decay rate
            _knockbackVelocity = Vector3.Lerp(_knockbackVelocity, Vector3.zero, _config.knockbackDecayRate * Time.fixedDeltaTime);

            // Stop knockback when it's below the minimum force
            if (_knockbackVelocity.magnitude <= _config.knockbackMinForce)
            {
                _rb.linearVelocity = Vector3.zero; // Stop movement
                return Node.Status.Success;
            }
            
            return Node.Status.Running;
        }

        public void Start()
        {
            Vector3 direction = (_enemy.position - _player.position).normalized; // Away from the player
            _knockbackVelocity = direction * _config.initialKnockbackForce; // Set initial knockback velocity
        }
    }
}
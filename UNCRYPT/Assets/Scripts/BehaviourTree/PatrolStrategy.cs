using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;

namespace BehaviourTree
{
    public class PatrolStrategy : IStrategy
    {
        private readonly Transform _entity;
        private readonly Rigidbody _rigidBody;
        private readonly Transform _player;
        private readonly Enemy.Enemy _enemy;
    
        public PatrolStrategy(Enemy.Enemy enemy)
        {
            _enemy = enemy;
            _player = GameManager.Instance.PlayerTransform;
            _entity = enemy.transform;
            _rigidBody = _enemy.Rigidbody;
        }

        public Node.Status FixedProcess()
        {
            Vector3 moveDirection = (_player.position - _entity.position).normalized;
            _rigidBody.linearVelocity = new Vector3(moveDirection.x, 0, moveDirection.z) * _enemy.Speed;

            if (_enemy.IsNearPlayer)
            {
                _rigidBody.linearVelocity = Vector3.zero;
                return Node.Status.Success;
            }
            if (!_enemy.CanViewPlayer)
            {
                return Node.Status.Failure; // Or Failure
            }
        
            return Node.Status.Running; // Still patrolling
        }
    }
}
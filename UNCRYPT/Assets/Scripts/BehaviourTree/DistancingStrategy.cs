using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;

namespace BehaviourTree
{
    [System.Serializable]
    public class DistancingConfig{
        [Header("Distancing Settings")] 
        public float distanceToKeepFromPlayer;
    }
    public class DistancingStrategy : IStrategy
    {
        private readonly Transform _entity;
        private readonly Rigidbody _rigidBody;
        private readonly Transform _player;
        private readonly Enemy.Enemy _enemy;
        private DistancingConfig _config;

        public DistancingStrategy(Enemy.Enemy enemy, DistancingConfig config)
        {
            _enemy = enemy;
            _player = GameManager.Instance.PlayerTransform;
            _entity = enemy.transform;
            _rigidBody = _enemy.Rigidbody;
            _config = config;
        }

        public Node.Status FixedProcess()
        {
            Vector3 playerDir = (_player.position - _entity.position).normalized;
            Vector3 distancingDir = -playerDir;
            _rigidBody.linearVelocity = new Vector3(distancingDir.x, 0, distancingDir.z) * _enemy.Speed;

            if (_enemy.DistanceFromPlayer >= _config.distanceToKeepFromPlayer)
            {
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
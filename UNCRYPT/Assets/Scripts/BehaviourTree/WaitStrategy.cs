using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;

namespace BehaviourTree
{
    public class WaitStrategy : IStrategy
    {
        private float _elapsedTime;
        private readonly float _timeToWait;

        public WaitStrategy(float timeToWait)
        {
            _timeToWait = timeToWait;
        }
        
        public Node.Status Process()
        {
            _elapsedTime += Time.deltaTime;
            return _elapsedTime >= _timeToWait ? Node.Status.Success : Node.Status.Running;
        }

        public void Start()
        {
            _elapsedTime = 0;
        }

        public void Reset()
        {
            _elapsedTime = 0;
        }
    }
}
using System;
using UnityEngine;

namespace BehaviourTree.Core.Strategies
{
    /// <summary>
    /// Strategy for checking conditions. Returns Success if condition is true,
    /// Failure if false.
    /// </summary>
    public class Condition : IStrategy
    {
        private readonly Func<bool> _condition;

        public Condition(Func<bool> condition)
        {
            _condition = condition;
        }

        public Node.Status Process()
        {
            bool result = _condition();
            return result ? Node.Status.Success : Node.Status.Failure;
        }

        public void Reset() { }
    }
}
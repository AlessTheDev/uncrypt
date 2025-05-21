using System;

namespace BehaviourTree.Core.Strategies
{
    /// <summary>
    /// Strategy for executing simple actions that always succeed.
    /// </summary>
    public class ActionStrategy : IStrategy
    {
        private readonly Action _action;

        public ActionStrategy(Action action)
        {
            _action = action;
        }

        public Node.Status Process()
        {
            _action();
            return Node.Status.Success;
        }
    }
}
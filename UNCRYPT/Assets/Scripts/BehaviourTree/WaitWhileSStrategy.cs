using System;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;

namespace BehaviourTree
{
    /// <summary>
    /// WARNING: NOT WORKING PROPERLY DUE TO BEHAVIOUR TREE IMPLEMENTATION ISSUE
    /// </summary>
    [Obsolete("NOT WORKING PROPERLY DUE TO BEHAVIOUR TREE IMPLEMENTATION ISSUE")]
    public class WaitWhileStrategy : IStrategy
    {
        private Func<bool> _condition;
        public WaitWhileStrategy(Func<bool> condition)
        {
            _condition = condition;
        }
        
        public Node.Status Process()
        {
            return _condition() ? Node.Status.Success : Node.Status.Running;
        }
        
        public Node.Status FixedProcess()
        {
            return Process();
        }
    }
}
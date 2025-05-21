using BehaviourTree.Core.Strategies;

namespace BehaviourTree.Core
{
    /// <summary>
    /// Represents a leaf (end) node that performs actual behaviors using a strategy pattern
    /// </summary>
    public class Leaf : Node
    {
        private readonly IStrategy _strategy;
        private bool _hasStarted;

        public Leaf(string name, IStrategy strategy) : base(name)
        {
            _strategy = strategy;
            _hasStarted = false;
        }
        
        public override Status Process()
        {
            StartIfNeeded();
            return _strategy.Process();
        }
        
        public override Status FixedProcess()
        {
            StartIfNeeded();
            return _strategy.FixedProcess();
        }

        private void StartIfNeeded()
        {
            if (!_hasStarted)
            {
                _strategy.Start();
                _hasStarted = true;
            }
        }

        protected override void Reset()
        {
            _hasStarted = false;
            _strategy.Reset();
        }
    }
}
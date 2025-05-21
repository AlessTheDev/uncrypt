namespace BehaviourTree.Core
{
    /// <summary>
    /// A control node that succeeds only if ALL children succeed. Executes children
    /// in order until one fails or all succeed.
    /// </summary>
    public class Sequence : Node
    {
        public Sequence(string name, Node[] children = null) : base(name, children)
        {
        }

        public override Status Process() => ProcessLogic(false);
        public override Status FixedProcess() => ProcessLogic(true);

        private Status ProcessLogic(bool fixedProcess)
        {
            if (CurrentChild < Children.Count)
            {
                Status status = fixedProcess ? 
                    Children[CurrentChild].FixedProcess() : 
                    Children[CurrentChild].Process();
            
                switch (status)
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        Reset();
                        return Status.Failure;
                    default:
                        CurrentChild++;
                        return CurrentChild == Children.Count ? Status.Success : Status.Running;
                }
            }
            Reset();
            return Status.Success;
        }

    }
}
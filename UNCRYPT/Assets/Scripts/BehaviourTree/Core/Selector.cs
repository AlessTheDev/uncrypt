namespace BehaviourTree.Core
{
    /// <summary>
    /// A control node that succeeds if ANY child succeeds. Tries each child in order
    /// until one succeeds or all fail.
    /// </summary>
    public class Selector : Node
    {
        public Selector(string name, Node[] children = null) : base(name, children)
        {
        }

        public override Status Process() => ProcessLogic(false);
        public override Status FixedProcess() => ProcessLogic(true);

        private Status ProcessLogic(bool fixedProcess)
        {
            while (CurrentChild < Children.Count)
            {
                Status status = fixedProcess ? 
                    Children[CurrentChild].FixedProcess() : 
                    Children[CurrentChild].Process();
    
                switch (status)
                {
                    case Status.Running:
                        return Status.Running;
                    case Status.Success:
                        Reset();
                        return Status.Success;
                    case Status.Failure:
                        CurrentChild++;
                        if (CurrentChild >= Children.Count)
                        {
                            Reset();
                            return Status.Failure;
                        }
                        continue;
                }
            }

            Reset();
            return Status.Failure;
        }
    }
}
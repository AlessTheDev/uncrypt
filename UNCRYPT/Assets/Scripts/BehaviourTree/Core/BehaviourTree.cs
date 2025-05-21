using System;

namespace BehaviourTree.Core
{
    // Credits to https://www.youtube.com/watch?v=lusROFJ3_t8 for the tutorial and clear explanation.
    public class BehaviourTree : Node
    {
        public BehaviourTree(string name, Node[] children) : base(name, children)
        {
        }

        public override Status Process() => ProcessLogic(false);
        public override Status FixedProcess() => ProcessLogic(true);
        private Status ProcessLogic(bool fixedProcess)
        {
            while (CurrentChild < Children.Count) // Use a loop to ensure all children are evaluated
            {
                Status status = fixedProcess ? Children[CurrentChild].FixedProcess() : Children[CurrentChild].Process();
                switch (status)
                {
                    case Status.Failure:
                    {
                        CurrentChild++;
                        continue; // Move to the next child in the Selector
                    }
                    case Status.Running:
                        return Status.Running; // Keep the current child running
                    case Status.Success:
                        Reset(); // Reset the Selector when one child succeeds
                        return Status.Success;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            Reset(); // If no child succeeds, reset and return failure
            return Status.Failure;
        }

        public void Interrupt()
        {
            Reset();
        }

    }
}
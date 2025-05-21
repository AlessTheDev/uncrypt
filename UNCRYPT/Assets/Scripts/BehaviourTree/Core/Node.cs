using System.Collections.Generic;

namespace BehaviourTree.Core
{
    /// <summary>
    /// Base class for all behavior tree nodes.
    /// </summary>
    public class Node
    {
        public enum Status
        {
            Success, // Node completed successfully
            Failure, // Node failed to complete
            Running // Node is still executing
        }

        public string Name;
        protected readonly List<Node> Children = new List<Node>();
        protected int CurrentChild;

        public Node(string name, Node[] children = null)
        {
            Name = name;

            if (children == null) return;

            foreach (Node child in children)
            {
                Children.Add(child);
            }
        }

        public void AddChild(Node child)
        {
            Children.Add(child);
        }

        /// <summary>
        /// Processes the current node by executing its children
        /// </summary>
        public virtual Status Process() => Children[CurrentChild].Process();
        
        /// <summary>
        /// Same as Process, but it should be called in FixedUpdate
        /// </summary>
        public virtual Status FixedProcess() => Children[CurrentChild].FixedProcess();


        /// <summary>
        /// Resets the node and all its children to their initial state
        /// </summary>
        protected virtual void Reset()
        {
            CurrentChild = 0;
            foreach (var child in Children)
            {
                child.Reset();
            }
        }

        public void Debug()
        {
            UnityEngine.Debug.Log(DebugString(0));
        }

        private string DebugString(int indent)
        {
            string output = "";
            for (int i = 0; i < indent; i++)
            {
                output += "  ";
            }

            output += Name;
            foreach (var child in Children)
            {
                output += "\n" + child.DebugString(indent + 1);
            }

            return output;
        }
    }
}
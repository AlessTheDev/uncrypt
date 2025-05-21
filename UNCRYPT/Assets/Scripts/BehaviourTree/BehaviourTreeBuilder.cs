using System;
using System.Collections.Generic;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;

namespace BehaviourTree
{
    public class BehaviourTreeBuilder
    {
        private string _name;
        private Node _root;
        private readonly Stack<Node> _nodeStack = new();

        public BehaviourTreeBuilder(string name)
        {
            _name = name;
        }

        public BehaviourTreeBuilder StartSelector(string name)
        {
            var selector = new Selector(name);
            AddOrSetAsRoot(selector);
            _nodeStack.Push(selector);
            return this;
        }

        public BehaviourTreeBuilder StartSequence(string name)
        {
            var sequence = new Sequence(name);
            AddOrSetAsRoot(sequence);
            _nodeStack.Push(sequence);
            return this;
        }

        public BehaviourTreeBuilder AddCondition(string name, Func<bool> condition)
        {
            var leaf = new Leaf(name, new Condition(condition));
            _nodeStack.Peek().AddChild(leaf);
            return this;
        }

        public BehaviourTreeBuilder AddAction(string name, Action action)
        {
            var leaf = new Leaf(name, new ActionStrategy(action));
            _nodeStack.Peek().AddChild(leaf);
            return this;
        }

        public BehaviourTreeBuilder End()
        {
            _nodeStack.Pop();
            return this;
        }

        #region Conditional Logic Helper

        public BehaviourTreeBuilder If(string name, Func<bool> condition)
        {
            // Create a selector that will try the condition path first, then the else path
            var selector = new Selector($"If ({name}) | Selector");
        
            // Create a sequence for the "Then" branch - it needs both condition AND action to succeed
            var thenSequence = new Sequence("Then Branch");
        
            // Create the condition leaf
            var conditionLeaf = new Leaf($"Condition", new Condition(condition));
            thenSequence.AddChild(conditionLeaf);
        
            selector.AddChild(thenSequence);
            AddOrSetAsRoot(selector);
        
            // Push both the selector and sequence to maintain proper nesting
            _nodeStack.Push(selector);     // Push selector first
            _nodeStack.Push(thenSequence); // Push sequence on top

            return this;
        }
        public BehaviourTreeBuilder Then(string name, Action action)
        {
            Then(new Leaf($"Then: {name}", new ActionStrategy(action)));

            return this;
        }
        
        public BehaviourTreeBuilder Then(Node thenLeaf)
        {
            _nodeStack.Peek().AddChild(thenLeaf);

            return this;
        }

        public BehaviourTreeBuilder Else(Node elseNode)
        {
            // Pop the Then sequence
            _nodeStack.Pop();

            elseNode.Name = $"Else: {elseNode.Name}";
            _nodeStack.Peek().AddChild(elseNode);
            
            // Pop the selector as we're done with this if-then-else block
            _nodeStack.Pop();

            return this;
        }

        public BehaviourTreeBuilder Else(string name, Action action)
        {
            var elseLeaf = new Leaf($"Else: {name}", new ActionStrategy(action));
            Else(elseLeaf);

            return this;
        }


        #endregion

        public Core.BehaviourTree Build()
        {
            // Wrap the root node in a BehaviourTree
            return _root != null
                ? new Core.BehaviourTree(_name, new[] { _root })
                : throw new InvalidOperationException("Tree must have a root node.");
        }

        private void AddOrSetAsRoot(Node node)
        {
            if (_nodeStack.Count > 0)
            {
                _nodeStack.Peek().AddChild(node);
            }
            else
            {
                _root = node;
            }
        }
    }
}

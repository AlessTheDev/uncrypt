using UnityEngine;

namespace Companion.States
{
    public class CompanionState : State
    {
        protected readonly CompanionController Controller;
        protected Transform transform => Controller.transform;

        public CompanionState(CompanionController controller)
        {
            Controller = controller;
        }
        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update()
        {
        }

        public override void FixedUpdate()
        {
        }
        
        public override void LateUpdate()
        {
        }
        
        // Helper methods for state transitions
        protected void TransitionTo<T>() where T : CompanionState
        {
            Controller.StateMachine.TransitionToState<T>();
        }
    }
}
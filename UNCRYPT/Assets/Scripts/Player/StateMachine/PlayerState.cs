namespace Player.StateMachine
{
    public abstract class PlayerState : State
    {
        protected readonly PlayerController Controller;

        protected PlayerState(PlayerController controller)
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

        public virtual void OnAttackInput(){}
        public virtual void OnDashInput(){}

        // Helper methods for state transitions
        protected void TransitionTo<T>() where T : PlayerState
        {
            Controller.StateMachine.TransitionToState<T>();
        }
    }
}
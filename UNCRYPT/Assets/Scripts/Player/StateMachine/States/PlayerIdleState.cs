using UnityEngine;

namespace Player.StateMachine.States
{
    public class PlayerIdleState : PlayerState
    {
        public PlayerIdleState(PlayerController controller) : base(controller)
        {
        }

        public override void Enter()
        {
            Controller.Config.rigidbody.linearVelocity = Vector3.zero;
            Controller.Config.animator.SetRunning(false);
        }

        public override void Update()
        {
            if (Controller.MoveInput)
            {
                TransitionTo<PlayerRunningState>();
            }
        }

        public override void OnAttackInput()
        {
            Controller.Attack();
        }
    }
}
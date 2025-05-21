using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.StateMachine.States
{
    public class PlayerNoMovementState : PlayerState
    {
        public PlayerNoMovementState(PlayerController controller) : base(controller)
        {
        }

        public override void Enter()
        {
            Controller.Config.rigidbody.linearVelocity = Vector3.zero;
            Controller.Config.animator.SetRunning(false);
        }

    }
}
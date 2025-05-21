using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.StateMachine.States
{
    public class DashState : PlayerState
    {
        private readonly Rigidbody _rb;
        private readonly PlayerConfig _config;
        private Vector3 _direction;
        
        private InputAction _dashInput;
        
        private float _elaspedTime;
        
        public DashState(PlayerController controller) : base(controller)
        {
            _config = controller.Config;
            _rb = _config.rigidbody;
            _dashInput = InputManager.Instance.InputActions.Player.Dash;
        }

        public override void Enter()
        {
            _direction = Controller.MovementDirection;
            _config.animator.StartDash();
            _config.dashAudioSource.pitch = Random.Range(0.9f, 1.1f);
            _config.dashAudioSource.Play();
            _elaspedTime = 0;
        }

        public override void FixedUpdate()
        {
            if (_elaspedTime <= _config.dashDuration && _dashInput.inProgress)
            {
                _rb.linearVelocity = _config.dashVelocity * Time.fixedDeltaTime * _direction;
                _elaspedTime += Time.fixedDeltaTime;
            }
            else
            {
                TransitionTo<PlayerRunningState>();
            }
            
        }

        public override void Exit()
        {
            _config.animator.EndDash();
        }
    }
}
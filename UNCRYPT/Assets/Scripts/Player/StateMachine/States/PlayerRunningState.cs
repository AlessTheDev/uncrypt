using UnityEngine;

namespace Player.StateMachine.States
{
    public class PlayerRunningState : PlayerState
    {
        private readonly Rigidbody _rigidbody;
        private readonly PlayerAnimator _animator;
        private readonly FaceCamera _faceCamera;
        private Vector3 _movement;
        private float _lastDashTime;

        private int _horizontalDir;

        #region Controller Properties

        private float MoveSpeed => Controller.Config.moveSpeed;
        private float HorizontalInput => Controller.HorizontalInput;

        #endregion

        public PlayerRunningState(PlayerController controller) : base(controller)
        {
            _rigidbody = controller.Config.rigidbody;
            _faceCamera = controller.Config.faceCamera;
            _animator = controller.Config.animator;
        }

        public override void Enter()
        {
            Controller.Config.animator.SetRunning(true);
        }

        public override void Update()
        {
            _movement = Controller.MovementDirection;

            // Handle sprite flipping
            if (HorizontalInput != 0)
            {
                int dir = (int)Mathf.Sign(HorizontalInput);

                if (dir != _horizontalDir)
                {
                    _faceCamera.flip = dir == -1;
                    _animator.TriggerHorizontalDirectionTransition();
                    _horizontalDir = dir;
                }
            }

            if (!Controller.MoveInput)
            {
                TransitionTo<PlayerIdleState>();
            }
        }

        public override void FixedUpdate()
        {
            PlayerConfig c = Controller.Config;

            Vector3 velocity = new Vector3(
                _movement.x * MoveSpeed,
                _rigidbody.linearVelocity.y,
                _movement.z * MoveSpeed
            );

            if (!IsGrounded(Controller.transform.position + _movement * c.groundCheckerDistanceMultiplier, 15))
            {
                velocity = Vector3.zero;
            }

            _rigidbody.linearVelocity = velocity;
        }

        private bool IsGrounded(Vector3 position, float distance)
        {
            return Physics.Raycast(position, Vector3.down, out RaycastHit hit, distance,
                Controller.Config.terrainLayer);
        }

        public override void OnAttackInput()
        {
            Controller.Attack();
        }

        public override void OnDashInput()
        {
            if (Time.time - _lastDashTime >= Controller.Config.dashCooldown)
            {
                TransitionTo<DashState>();
                _lastDashTime = Time.time;
            }
        }
    }
}
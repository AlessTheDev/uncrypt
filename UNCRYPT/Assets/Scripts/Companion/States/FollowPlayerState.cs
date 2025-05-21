using System.Collections;
using UnityEngine;

namespace Companion.States
{
    public class FollowPlayerState : CompanionState
    {
        private Vector3 _targetPosition;
        private Vector3 _velocity;

        private Transform _player;

        private readonly FollowPlayerConfig _config;

        private bool _slowDown;

        public FollowPlayerState(CompanionController controller) : base(controller)
        {
            _config = controller.FollowPlayerConfig;
        }

        public override void Enter()
        {
            _player = GameManager.Instance.Player.transform;
            Controller.StartCoroutine(SlowDownForSomeSeconds(3));
        }

        private IEnumerator SlowDownForSomeSeconds(float seconds)
        {
            _slowDown = true;
            yield return new WaitForSeconds(seconds);
            _slowDown = false;
        }

        public override void Update()
        {
            Controller.FaceCamera.flip = _player.position.x < transform.position.x;
        }

        public override void FixedUpdate()
        {
            transform.position =
                Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, _slowDown ? 0.9f : (1 / _config.speed));

            Vector3 diff = transform.position - _player.position;
            diff = diff.normalized;
            _targetPosition = _player.position +
                              new Vector3(diff.x, 0, diff.z) * _config.maxDistanceFromPlayer / 2;
            #if UNITY_EDITOR
            Debug.DrawLine(_player.position, _targetPosition);
            #endif
        }
    }
}
using UI;
using UnityEngine;
using UnityEngine.AI;
using Utils;

namespace Companion.States
{
    public class LeadPlayerState : CompanionState
    {
        private readonly LeadPlayerConfig _leadConf;
        private readonly Transform _playerTransform;
        private bool _hasStartedMoving = false;
        private float _timeSinceLastMovement;

        private float _defaultSpeed;

        public LeadPlayerState(CompanionController controller) : base(controller)
        {
            _leadConf = controller.LeadPlayerConfig;
            _playerTransform = GameManager.Instance.PlayerTransform;
            _defaultSpeed = _leadConf.agent.speed;
        }

        public override void Enter()
        {
            _leadConf.agent.enabled = true;
            _hasStartedMoving = false;
        }

        public override void Update()
        {
            var agent = _leadConf.agent;

            bool isPlayerNear = Vector3.Distance(transform.position, _playerTransform.position) <
                                _leadConf.maxDistanceFromPlayer;
            
            Transform playerTransform = GameManager.Instance.PlayerTransform;
            bool isPlayerOnPath = IsPosOnPath(agent.path, playerTransform.position);
            
            if (isPlayerNear || isPlayerOnPath)
            {
                agent.isStopped = false;

                if (isPlayerOnPath)
                {
                    agent.speed = _defaultSpeed + 5;
                }
                
                Vector3 target = _leadConf.targetPosition;
                agent.SetDestination(new Vector3(target.x, transform.position.y, target.z));
                _timeSinceLastMovement = 0;

                // Check if we've started moving
                if (!_hasStartedMoving && agent.velocity.sqrMagnitude > 0.1f)
                {
                    _hasStartedMoving = true;
                }
                
                _leadConf.companionNavigator.HideIndicator();
            }
            else
            {
                agent.isStopped = true;
                _timeSinceLastMovement += Time.deltaTime;
                
                agent.speed = _defaultSpeed;

                if (_timeSinceLastMovement > 7f && !TopUIDialogue.Instance.IsDialogueActive)
                {
                    TopUIDialogue.Instance.ShowDialogue(
                        new TopUIDialogue.DialogueSentence("You're too far away, stay close to me."),
                        new WaitForSeconds(2));
                    _timeSinceLastMovement = 0;
                }
                
                _leadConf.companionNavigator.ShowIndicator();
            }
        }

        public override void LateUpdate()
        {
            var agent = _leadConf.agent;

            Controller.FaceCamera.flip = _leadConf.targetPosition.x < transform.position.x;

            // If the player is near the destination
            bool hasPlayerReachedDest = Utilities.RadialCheck(GameManager.Instance.PlayerTransform.position, _leadConf.targetPosition, 5f);
            
            // Only check for destination if we've actually started moving
            if ((_hasStartedMoving && agent.remainingDistance <= agent.stoppingDistance) || hasPlayerReachedDest) 
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    _leadConf.onTargetReached?.Invoke();
                    TransitionTo<FollowPlayerState>();
                }
            }
        }

        public bool IsPosOnPath(NavMeshPath path, Vector3 pos)
        {
            Vector3[] waypoints = path.corners;

            if (waypoints.Length < 2)
            {
                return false;
            }
            
            for(int i = 1; i < waypoints.Length; i++)
            {
                Vector3 a = waypoints[i - 1];
                Vector3 b = waypoints[i];

                if (DistanceFromSegment(new Vector2(a.x, a.z), new Vector2(b.x, b.z), new Vector2(pos.x, pos.z)) < 20f)
                {
                    return true;
                }
            }

            return false;
        }

        private float DistanceFromSegment(Vector2 segmentStart, Vector2 segmentEnd, Vector2 position)
        {
            Vector2 segment = segmentEnd - segmentStart;
            Vector2 startToTarget = position - segmentStart;
            float projection = Vector2.Dot(startToTarget, segment);
            
            float squaredLength = segment.SqrMagnitude();
            float normalizedProjection = projection / squaredLength;

            Vector2 closestPoint = normalizedProjection switch
            {
                <= 0 => segmentStart,
                >= 1 => segmentEnd,
                _ => segmentStart + segment * normalizedProjection
            };

            return Vector2.Distance(position, closestPoint);
        }

        public override void Exit()
        {
            _leadConf.agent.enabled = false;
            _leadConf.targetPosition = Vector3.positiveInfinity;
            _leadConf.companionNavigator.HideIndicator();
        }
    }
}
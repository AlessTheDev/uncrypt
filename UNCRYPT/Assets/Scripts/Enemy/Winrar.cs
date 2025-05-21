using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using Bullets;
using UnityEngine;

namespace Enemy
{
    public class WinrarEnemy : Enemy
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int Attack = Animator.StringToHash("Attack");
        [SerializeField] private Transform bombSpawn;
        [SerializeField] private Animator animator;
        [SerializeField] private DistancingConfig distancingConfig;
        [SerializeField] private float attackCooldown;
        [SerializeField] private AudioSource throwAudioSource;

        [Header("Bomb Launch Settings")]
        [SerializeField] private float flightTime;
        [SerializeField] private float launchAngle;

        private float _lastLaunchTime;

        protected override void OnStart()
        {
            Sequence patrolSequence = new Sequence("Patrol Sequence", new Node[]
            {
                new Leaf("Patrol Animation", new ActionStrategy(() => animator.SetBool(IsWalking, true))),
                new Leaf("Patrol", new PatrolStrategy(this))
            });
            
            Sequence attackSequence = new Sequence("Attack Sequence", new Node[]
            {
                new Leaf("Stop Patrolling", new ActionStrategy(() => animator.SetBool(IsWalking, false))),
                new Leaf("Trigger Attack Animation", new ActionStrategy(() => animator.SetTrigger(Attack))),
                new Leaf("Load", new WaitStrategy(attackCooldown)),
            });
            
            Selector attackAndDistance = new Selector("Attack While Distancing", new Node[]
            {
                new Sequence("Distance If Needed", new Node[]
                {
                    new Leaf("Should Distance Itself from player?", new Condition(() => DistanceFromPlayer <= distancingConfig.distanceToKeepFromPlayer)),
                    new Leaf("Start Walk Animation", new ActionStrategy(() => animator.SetBool(IsWalking, true))),
                    new Leaf("Keep Distance", new DistancingStrategy(this, distancingConfig))
                }),
                attackSequence
            });
            
            BehaviourTree = new BehaviourTreeBuilder("Sniper Enemy")
                .StartSequence("Check Player Status")
                .If("Can View Player", () => CanViewPlayer)
                .If("Can Attack Player", () => IsNearPlayer)
                .Then(attackAndDistance)
                .Else(patrolSequence)
                .Else("Idle", () => animator.SetBool(IsWalking, false))
                .Build();
        }

        public void ThrowBomb()
        {
            Vector3 target = GameManager.Instance.PlayerTransform.position;
            Vector3 direction = target - bombSpawn.position;
            
            float horizontalDistance = new Vector2(direction.x, direction.z).magnitude;
            float horizontalSpeed = horizontalDistance / flightTime;

            Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z).normalized;

            // (targetY - originY + 0.5 * g * t^2) / t
            float gravity = Mathf.Abs(Physics.gravity.y);
            float verticalSpeed = (direction.y + 0.5f * gravity * flightTime * flightTime) / flightTime;

            Vector3 velocity = horizontalDirection * horizontalSpeed;
            velocity.y = verticalSpeed;

            Rigidbody spawnedBomb = WinrarBombsPool.Instance.GetBomb();
            spawnedBomb.position = bombSpawn.position;
            spawnedBomb.linearVelocity = velocity;
            
            throwAudioSource.Play();
        }

    }
}
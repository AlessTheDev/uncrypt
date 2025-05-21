using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using Bullets;
using UnityEngine;

namespace Enemy
{
    public class Sniper : Enemy
    {
        [SerializeField] private Transform bulletSpawn;
        [SerializeField] private Animator animator;
        [SerializeField] private DistancingConfig distancingConfig;
        [SerializeField] private float attackCooldown;
        
        private static readonly int IsWalking = Animator.StringToHash("IsPatrolling");
        private static readonly int Attack = Animator.StringToHash("Attack");
        private static readonly int Alert = Animator.StringToHash("Alert");

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
                new Leaf("Load", new WaitStrategy(attackCooldown / 2)),
                new Leaf("Alert", new ActionStrategy(() => animator.SetTrigger(Alert))),
                new Leaf("Load", new WaitStrategy(attackCooldown / 2))
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

        public void SpawnBullet()
        {
            Bullet bullet = BigBulletsPool.Instance.GetBullet();
            Transform bulletTransform = bullet.transform;
            bulletTransform.position = bulletSpawn.position;
            bulletTransform.rotation = bulletSpawn.rotation;
            bullet.Launch();
        }
    }
}
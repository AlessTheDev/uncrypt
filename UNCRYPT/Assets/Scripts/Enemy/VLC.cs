using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using Bullets;
using UnityEngine;

namespace Enemy
{
    public class VLC : Enemy
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int Attack = Animator.StringToHash("Attack");
        [SerializeField] private Transform spawner;
        [SerializeField] private Animator animator;
        [SerializeField] private float attackCooldown;
        [SerializeField] private DistancingConfig distancingConfig;
        
        private Transform[] _subSpawners;

        protected override void OnStart()
        {
            _subSpawners = new Transform[spawner.childCount];
            for (int i = 0; i < _subSpawners.Length; i++)
            {
                _subSpawners[i] = spawner.GetChild(i);
            }
            
            Sequence patrolSequence = new Sequence("Patrol Sequence", new Node[]
            {
                new Leaf("Patrol Animation", new ActionStrategy(() => animator.SetBool(IsWalking, true))),
                new Leaf("Patrol", new PatrolStrategy(this))
            });
            
            Sequence attackSequence = new Sequence("Attack Sequence", new Node[]
            {
                new Leaf("Stop Patrolling", new ActionStrategy(() => animator.SetBool(IsWalking, false))),
                new Leaf("Trigger Attack", new ActionStrategy(() =>
                {
                    animator.SetTrigger(Attack);
                })),
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

        public void SpawnBullets()
        {
            VlcBulletsPooler pooler = VlcBulletsPooler.Instance;

            Vector3 directionToPlayer = GameManager.Instance.PlayerTransform.position - spawner.position;
            directionToPlayer.Normalize();
            
            float angleToPlayer = Vector3.SignedAngle(Vector3.forward, directionToPlayer, Vector3.up);
            
            spawner.rotation = Quaternion.Euler(0, angleToPlayer, 0);
            
            foreach (var s in _subSpawners)
            {
                Bullet b = pooler.GetBullet();
                
                b.transform.position = s.position;
                b.transform.rotation = s.rotation;
                b.transform.GetChild(0).transform.rotation = Quaternion.identity;
                b.Launch();
            }
        }
    }
}
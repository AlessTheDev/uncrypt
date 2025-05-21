using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using Bullets;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    public class Wizard : Enemy
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        [SerializeField] private Animator animator;
        [SerializeField] private float attackCooldown;
        [SerializeField] private float lightningsToSpawn;
        [SerializeField] private float attackSmallRadius;
        [SerializeField] private float attackBigRadius;
        
        private Transform player;
        protected override void OnStart()
        {
            player = GameManager.Instance.PlayerTransform;
            
            Sequence patrolSequence = new Sequence("Patrol Sequence", new Node[]
            {
                new Leaf("Patrol Animation", new ActionStrategy(() =>
                {
                    animator.SetBool(IsAttacking, false);
                    animator.SetBool(IsWalking, true);
                })),
                new Leaf("Patrol", new PatrolStrategy(this))
            });
            
            BehaviourTree = new BehaviourTreeBuilder("Sniper Enemy")
                .StartSequence("Check Player Status")
                .If("Can View Player", () => CanViewPlayer)
                .If("Can Attack Player", () => IsNearPlayer)
                .Then(new Sequence("Attack Sequence", new Node[]
                {
                    new Leaf("Attack Animation", new ActionStrategy(() => animator.SetBool(IsAttacking, true))),
                    new Leaf("Spawn Lightnings", new ActionStrategy(SpawnLightnings)),
                    new Leaf("Wait", new WaitStrategy(attackCooldown))
                }))
                .Else(patrolSequence)
                .Else("Idle", () => animator.SetBool(IsWalking, false))
                .Build();
        }

        private void SpawnLightnings()
        {
            Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
            
            // Spawn at center
            LightningPool.Instance.GetLightning().transform.position = playerPos;
            
            // Spawn around
            float step = Mathf.PI * 2 / (lightningsToSpawn - 1);

            for (int i = 0; i < lightningsToSpawn - 1; i++)
            {
                float radius = i % 2 == 0 ? attackSmallRadius : attackBigRadius;
                Vector3 offset = new Vector3(Mathf.Sin(i * step), 0, Mathf.Cos(i * step)) * radius;

                Vector3 pos = playerPos + offset;
                if (!NavMesh.SamplePosition(pos, out NavMeshHit hit, .5f, NavMesh.AllAreas))
                {
                    continue;
                }
                LightningPool.Instance.GetLightning().transform.position = pos;
            }
        }
    }
}
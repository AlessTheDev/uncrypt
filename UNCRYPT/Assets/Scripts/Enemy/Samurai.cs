using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;

namespace Enemy
{
    public class Samurai: Enemy
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int Load = Animator.StringToHash("Load");
        private static readonly int Attack = Animator.StringToHash("Attack");
        
        [SerializeField] private Animator animator;
        [SerializeField] private KnockBackConfig knockbackConfig;
        [SerializeField] private SamuraiAttackConfig attackConfig;
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
                new Leaf("Start Loading", new ActionStrategy(() => animator.SetTrigger(Load))),
                new Leaf("Wait for Load", new WaitStrategy(1)),
                new Sequence("Attack!", new Node[]
                {
                    new Leaf("Start Attack Animation", new ActionStrategy(() => animator.SetTrigger(Attack))),
                    new Leaf("Attack", new SamuraiAttackStrategy(this, attackConfig)),
                    new Leaf("Wait for Cooldown", new WaitStrategy(attackConfig.cooldown))
                })
            });
            
            BehaviourTree.Core.BehaviourTree checkPlayerStatus = new BehaviourTreeBuilder("Check Player Status")
                .StartSequence("Check Player Status")
                    .If("Can View Player", () => CanViewPlayer)
                        .If("Can Attack Player", () => IsNearPlayer)
                            .Then(attackSequence)
                        .Else(patrolSequence)
                .Else("Idle", () => animator.SetBool(IsWalking, false))
                .Build();

            BehaviourTree = new BehaviourTreeBuilder("Sniper Enemy")
                .StartSequence("Check Player Status")
                    .If("Knockback", () => HasBeenAttacked)
                        .Then(new Leaf("Knockback", new KnockbackStrategy(this, knockbackConfig)))
                    .Else(checkPlayerStatus)
                .Build();
        }
        
        protected override void OnHit(Collider col)
        {
            BehaviourTree.Interrupt();
        }
    }
}
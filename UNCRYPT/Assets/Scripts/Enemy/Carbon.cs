using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;

namespace Enemy
{
    public class Carbon : Enemy
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        [SerializeField] private Animator animator;
        [SerializeField] private CarbonAttackConfig attackConfig;
        protected override void OnStart()
        {
            Sequence patrolSequence = new Sequence("Patrol Sequence", new Node[]
            {
                new Leaf("Patrol Animation", new ActionStrategy(() =>
                {
                    animator.SetBool(IsWalking, true);
                })),
                new Leaf("Patrol", new PatrolStrategy(this))
            });

            Sequence attack = new Sequence("Attack", new []
            {
                new Leaf("Attack", new CarbonAttackStrategy(this, animator, attackConfig)),
                new Leaf("Wait", new WaitStrategy(2)),
                new Leaf("Destroy", new ActionStrategy(() => Destroy(gameObject)))
            });
            BehaviourTree = new BehaviourTreeBuilder("Sniper Enemy")
                .StartSequence("Check Player Status")
                    .If("Can View Player", () => CanViewPlayer)
                        .If("Can Attack Player", () => IsNearPlayer)
                            .Then(attack)
                        .Else(patrolSequence)
                    .Else("Idle", () => animator.SetBool(IsWalking, false))
                .Build();
        }
    }
}
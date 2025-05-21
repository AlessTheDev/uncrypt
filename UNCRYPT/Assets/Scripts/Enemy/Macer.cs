using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using UnityEngine;

namespace Enemy
{
    public class Macer : Enemy
    {
        private static readonly int IsWalking = Animator.StringToHash("IsWalking");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        [SerializeField] private Animator animator;
        protected override void OnStart()
        {
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
                            .Then(new Leaf("Attack Animation", new ActionStrategy(() => animator.SetBool(IsAttacking, true))))
                        .Else(patrolSequence)
                    .Else("Idle", () => animator.SetBool(IsWalking, false))
                    .Build();
        }
    }
}
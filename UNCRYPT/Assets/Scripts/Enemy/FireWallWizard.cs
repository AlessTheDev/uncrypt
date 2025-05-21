using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using Bullets;
using UnityEngine;

namespace Enemy
{
    public class FireWallWizard : Enemy
    {
        private static readonly int IsPatrolling = Animator.StringToHash("IsPatrolling");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        [SerializeField] private Animator animator;
        [SerializeField] private WizardBulletsController bulletsController;
        [SerializeField] private DistancingConfig distancingConfig;

        protected override void OnStart()
        {
            Sequence patrolSequence = new Sequence("Patrol Sequence", new Node[]
            {
                new Leaf("Patrol Animation", new ActionStrategy(() => animator.SetBool(IsPatrolling, true))),
                new Leaf("Stop Attacking", new ActionStrategy(() =>
                {
                    animator.SetBool(IsAttacking, false);
                    bulletsController.Disable();
                })),
                new Leaf("Patrol", new PatrolStrategy(this))
            });
            
            Selector attackAndDistance = new Selector("Attack While Distancing", new Node[]
            {
                new Sequence("Distance If Needed", new Node[]
                {
                    new Leaf("Should Distance Itself from player?", new Condition(() => DistanceFromPlayer <= distancingConfig.distanceToKeepFromPlayer)),
                    new Leaf("Keep Distance", new DistancingStrategy(this, distancingConfig))
                }),
                new Leaf("Start Attacking", new ActionStrategy(() =>
                {
                    animator.SetBool(IsAttacking, true);
                    bulletsController.Enable();
                }))
            });


            
            BehaviourTree = new BehaviourTreeBuilder("Check Player Status")
                .StartSequence("Check Player Status")
                .If("Can View Player", () => CanViewPlayer)
                    .If("Can Attack Player", () => IsNearPlayer)
                        .Then(attackAndDistance)
                    .Else(patrolSequence)
                .Else("Idle", () => animator.SetBool(IsPatrolling, false))
                .Build();
        }
        
        protected new void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, distancingConfig.distanceToKeepFromPlayer);
        }
    }
}
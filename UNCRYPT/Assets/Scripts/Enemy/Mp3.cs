using System;
using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using Bullets;
using UnityEngine;
using UnityEngine.UIElements;

namespace Enemy
{
    public class Mp3 : Enemy
    {
        private static readonly int IsPatrolling = Animator.StringToHash("IsPatrolling");
        private static readonly int Attack = Animator.StringToHash("Attack");
        [SerializeField] private Animator animator;
        [SerializeField] private NotesController notesController;
        [SerializeField] private float attackCooldown = 2;

        private bool _isAttacking;

        protected override void OnStart()
        {
            Sequence patrolSequence = new Sequence("Patrol Sequence", new Node[]
            {
                new Leaf("Patrol Animation", new ActionStrategy(() => animator.SetBool(IsPatrolling, true))),
                new Leaf("Patrol", new PatrolStrategy(this))
            });

            Sequence attack = new Sequence("Attack", new Node[]
            {
                new Leaf("Disable patrolling", new ActionStrategy(() => animator.SetBool(IsPatrolling, false))),
                new Leaf("Start Attack", new ActionStrategy(() =>
                {
                    if (_isAttacking) return;
                    
                    animator.SetTrigger(Attack);
                    _isAttacking = true;
                })),
                new Leaf("Wait for cooldown", new WaitStrategy(attackCooldown))
            });


            BehaviourTree = new BehaviourTreeBuilder("Check Player Status")
                .StartSequence("Check Player Status")
                .If("Can View Player", () => CanViewPlayer)
                .If("Can Attack Player", () => IsNearPlayer)
                .Then(attack)
                .Else(patrolSequence)
                .Else("Idle", () => animator.SetBool(IsPatrolling, false))
                .Build();
        }

        public void ResetIsAttacking()
        {
            _isAttacking = false;
        }

        public void SpawnNotes(int n)
        {
            notesController.SpawnNotes(n);
        }
    }
}
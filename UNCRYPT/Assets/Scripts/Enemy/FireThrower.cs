using System;
using System.Collections;
using BehaviourTree;
using BehaviourTree.Core;
using BehaviourTree.Core.Strategies;
using Bullets;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

namespace Enemy
{
    public class FireThrower : Enemy
    {
        private static readonly int IsPatrolling = Animator.StringToHash("IsPatrolling");
        private static readonly int IsAttacking = Animator.StringToHash("IsAttacking");
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationClip alertClip;

        [Header("Attack Settings")] 
        [SerializeField] private Bullet bulletPrefab;

        [SerializeField] private float timeBetweenBullets;
        [SerializeField] private int bulletsCount;
        [SerializeField] private float bulletSpawnerRadius;
        [SerializeField] private float bulletSpawnHeight;
        [SerializeField] private float attackCooldown;

        private ObjectPool<Bullet> _bulletPool;

        private void Awake()
        {
            _bulletPool = new ObjectPool<Bullet>(
                () =>
                {
                    Bullet b = Instantiate(bulletPrefab, transform);
                    b.SetPool(_bulletPool);
                    return b;
                },
                bullet =>
                {
                    bullet.gameObject.SetActive(true);
                    bullet.transform.SetParent(transform);
                },
                bullet => bullet.gameObject.SetActive(false),
                shape => Destroy(shape.gameObject),
                false,
                bulletsCount * 2
            );
        }

        protected override void OnStart()
        {
            float alertDuration = alertClip.length;
            
            Sequence patrolSequence = new Sequence("Patrol Sequence", new Node[]
            {
                new Leaf("Patrol Animation", new ActionStrategy(() => animator.SetBool(IsPatrolling, true))),
                new Leaf("Patrol", new PatrolStrategy(this))
            });
            
            Sequence attack = new Sequence("Attack", new Node[]
            {
                new Leaf("Disable patrolling", new ActionStrategy(() => animator.SetBool(IsPatrolling, false))),
                new Leaf("Is Attacking Already?", new Condition(() => !animator.GetBool(IsAttacking))),
                new Leaf("Start Alert Animation", new ActionStrategy(() => animator.SetBool(IsAttacking, true))),
                new Leaf("Wait for alert animation", new WaitStrategy(alertDuration)),
                new Leaf("Spawn bullets", new ActionStrategy(() => StartCoroutine(AttackCoroutine()))),
                new Leaf("Wait", new WaitStrategy(attackCooldown))
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

        private IEnumerator AttackCoroutine()
        {
            float step = Mathf.PI * 2 / bulletsCount;
            for (float angle = step; angle < Mathf.PI * 2; angle += step)
            {
                yield return new WaitForSeconds(timeBetweenBullets);
                Bullet b = _bulletPool.Get();
                Vector3 offset = bulletSpawnerRadius * new Vector3(Mathf.Sin(angle), bulletSpawnHeight, Mathf.Cos(angle));
                b.transform.position = transform.position + offset;
                b.transform.rotation = Quaternion.Euler(0, angle * Mathf.Rad2Deg, 0);
                b.Launch();
            }

            animator.SetBool(IsAttacking, false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position + new Vector3(0, bulletSpawnHeight, 0), bulletSpawnerRadius);
        }
    }
}
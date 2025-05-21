using System;
using System.Collections;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace Bullets
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private LayerMask playerMask;
        [SerializeField] private float maxDistance = 20;
        [SerializeField] private float directionModifierRange = 8;
        [SerializeField] private float lifeTimeAfterLaunch = 8;
        [SerializeField] private float spawnAnimationDuration = 1;
        [SerializeField] private bool predictPlayerPos = true;
        [SerializeField] private float damage = 5;

        private ObjectPool<Bullet> _parentPool;

        private bool _launched;
        
        private AudioSource _audioSource;

        public bool RayIntersectsPlayer => Physics.Raycast(transform.position,
            transform.TransformDirection(Vector3.forward), out _, maxDistance, playerMask);

        private void OnEnable()
        {
            _launched = false;

            Vector3 initialScale = transform.localScale;
            transform.localScale = Vector3.zero;
            transform.DOScale(initialScale, spawnAnimationDuration).SetEase(Ease.OutBounce);

            _audioSource = GetComponentInChildren<AudioSource>();
        }

        private void FixedUpdate()
        {
            if (_launched)
            {
                transform.position += transform.forward * (speed * Time.fixedDeltaTime);
            }

#if UNITY_EDITOR
            Debug.DrawLine(transform.position, transform.position + transform.forward * maxDistance, Color.green);
#endif
        }

        public void Launch()
        {
            transform.parent = null;
            _launched = true;
            
            _audioSource?.Play();

            if (predictPlayerPos)
            {
                // Get player transform and velocity
                Transform player = GameManager.Instance.PlayerTransform;
                Rigidbody playerRb = GameManager.Instance.PlayerRb;

                Vector3 playerFuturePosition = PredictFuturePosition(player.position, playerRb.linearVelocity);

                Quaternion initialRotation = transform.rotation;
                transform.LookAt(playerFuturePosition);

                transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, transform.rotation.eulerAngles.y,
                    transform.rotation.eulerAngles.z);

                transform.Rotate(new Vector3(0, Random.Range(-directionModifierRange, directionModifierRange), 0));

            } 
            
            StartCoroutine(DestroyAfterTime(lifeTimeAfterLaunch));
        }

        private Vector3 PredictFuturePosition(Vector3 playerPosition, Vector3 playerVelocity)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
            float timeToReachPlayer = distanceToPlayer / speed;
            return playerPosition + playerVelocity * timeToReachPlayer;
        }

        private IEnumerator DestroyAfterTime(float time)
        {
            yield return new WaitForSeconds(time);
            DestroyOrRelease();
        }

        public void SetPool(ObjectPool<Bullet> pool)
        {
            _parentPool = pool;
        }

        private void DestroyOrRelease()
        {
            if (_parentPool == null)
            {
                Destroy(gameObject);
            }
            else
            {
                _parentPool.Release(this);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == PlayerController.Layer)
            {
                GameManager.Instance.Player.Damage(damage);
                if (_launched)
                {
                    DestroyOrRelease();
                }
            }
        }
    }
}
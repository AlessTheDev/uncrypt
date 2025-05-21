using System.Collections;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy
{
    public abstract class Enemy : MonoBehaviour
    {
        [Header("Main Settings")] 
        [SerializeField] private Indicator indicator;

        [SerializeField] private Rigidbody rb;
        [SerializeField] private Transform visual;
        [SerializeField] private SpriteRenderer whiteOverlay;
        [SerializeField] private float speed;
        [SerializeField] private float damageEffectDuration = 0.2f;

        [SerializeField] private float viewRange;
        [SerializeField] private float attackRange;

        [SerializeField] private float hp;
        [SerializeField] private float damage;
        [SerializeField] private float dissolveDuration = 1.2f;
        [SerializeField] private AudioSource hitSfx;
        [SerializeField] private GameObject heart;
        
        
        
        public UnityEvent<Enemy> OnDeath;

        #region Settings Getters

        public Indicator Indicator => indicator;
        public Rigidbody Rigidbody => rb;
        public float Speed => speed;
        public float ViewRange => viewRange;
        public float AttackRange => attackRange;
        public float HP => hp;
        public float Damage => damage;
        public Transform Visual => visual;

        #endregion

        private bool _isPlayerInAttackRange;

        public bool CanViewPlayer { get; private set; }
        public bool IsNearPlayer { get; private set; }

        public bool HasBeenAttacked { get; private set; }

        private float _lastAttackReceivedTimestamp;

        private readonly int _defaultDirection;

        private readonly int DissolveAmount = Shader.PropertyToID("_DissolveAmount");
        
        private bool _processBehaviourTree;
        protected BehaviourTree.Core.BehaviourTree BehaviourTree;

        private bool _isPlayerInSafeZone;

        protected virtual void OnHit(Collider col)
        {
        }


        private void Start()
        {
            Player = GameManager.Instance.PlayerTransform;
            _processBehaviourTree = true;

            rb.mass = 10;

            GameManager.Instance.OnPlayerEntersSafeZone.AddListener(OnPlayerEntersSafeZone);
            GameManager.Instance.OnPlayerExitsSafeZone.AddListener(OnPlayerExitsSafeZone);
            OnStart();
        }

        private void OnPlayerEntersSafeZone()
        {
            _isPlayerInSafeZone = true;
            BehaviourTree.Interrupt();
        }

        private void OnPlayerExitsSafeZone()
        {
            _isPlayerInSafeZone = false;
        }

        private void Update()
        {
            float distanceFromPlayer = DistanceFromPlayer;

            bool couldViewPlayer = CanViewPlayer;
            CanViewPlayer = distanceFromPlayer <= viewRange && !_isPlayerInSafeZone;

            if (CanViewPlayer && !couldViewPlayer)
            {
                OnAlert();
                Indicator.ShowForSeconds(2);
            }

            IsNearPlayer = distanceFromPlayer <= attackRange;

            visual.eulerAngles = new Vector3(0, transform.position.x > Player.transform.position.x ? 180 : 0, 0);

            if (_processBehaviourTree)
            {
                BehaviourTree.Process();
            }

            OnUpdate();

            if (HasBeenAttacked && Time.time - _lastAttackReceivedTimestamp >= 0.1f)
            {
                HasBeenAttacked = false;
            }
        }

        protected virtual void OnAlert()
        {
            
        }

        private void FixedUpdate()
        {
            if (_processBehaviourTree)
            {
                BehaviourTree.FixedProcess();
            }
            OnFixedUpdate();
        }

        protected virtual void OnStart()
        {
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnFixedUpdate()
        {
        }

        protected void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, viewRange);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        public Transform Player { get; private set; }

        public float DistanceFromPlayer => Vector3.Distance(Player.position, transform.position);

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player Attack"))
            {
                _lastAttackReceivedTimestamp = Time.time;
                HasBeenAttacked = true;
                
                // Hit Effect
                whiteOverlay.DOFade(1, damageEffectDuration / 2)
                    .OnComplete(() => whiteOverlay.DOFade(0, damageEffectDuration / 2));
                GameManager.Instance.SpawnAttackParticles(other.ClosestPoint(transform.position));
                
                hitSfx.Play();
                // Update HP
                hp -= PlayerStats.Damage;
                if (hp <= 0)
                {
                    OnDeath?.Invoke(this);
                    Rigidbody.isKinematic = true;
                    Collider c = GetComponent<Collider>();
                    if (c)
                    {
                        c.enabled = false;
                    }
                    foreach (var col in GetComponentsInChildren<Collider>())
                    {
                        col.enabled = false;
                    }

                    if (heart != null && Random.Range(0, 100) < 30)
                    {
                        Transform h = Instantiate(heart, transform.position, Quaternion.identity).transform;
                        
                        Collider col = h.GetComponentInChildren<Collider>();
                        col.enabled = false;
                        
                        h.transform.localScale = Vector3.zero;

                        float y = transform.position.y;
                        Sequence sequence = DOTween.Sequence();
                        h.transform.DOScale(Vector3.one, .6f);
                        sequence
                            .Append(h.DOMoveY(y + 2.5f, .5f).OnComplete(() => col.enabled = true))
                            .Append(h.DOMoveY(y + .3f, .6f).SetEase(Ease.OutBounce));

                        sequence.Play();
                    }
                    StartCoroutine(Dissolve());
                }
                
                // Call Event on children
                OnHit(other);
            }

            if (other.gameObject.layer == PlayerController.Layer)
            {
                GameManager.Instance.Player.Damage(damage);
            }
        }

        private IEnumerator Dissolve()
        {
            _processBehaviourTree = false;

            SpriteRenderer sr = visual.GetComponent<SpriteRenderer>();
            if (!sr)
            {
                sr = visual.GetComponentInChildren<SpriteRenderer>();
            }

            Material material = sr.material;
            material.SetFloat(DissolveAmount, 0);

            DOTween.To(() => material.GetFloat(DissolveAmount),
                x => material.SetFloat(DissolveAmount, x),
                1f, dissolveDuration);

            yield return new WaitForSeconds(dissolveDuration);

            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (GameManager.Instance == null) return;
            GameManager.Instance.OnPlayerEntersSafeZone.RemoveListener(OnPlayerEntersSafeZone);
            GameManager.Instance.OnPlayerExitsSafeZone.RemoveListener(OnPlayerExitsSafeZone);
        }
    }
}
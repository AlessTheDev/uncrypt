using System;
using System.Collections;
using DG.Tweening;
using Player.StateMachine;
using Player.StateMachine.States;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    [Serializable]
    public class PlayerConfig
    {
        [Header("References")] public Rigidbody rigidbody;
        public FaceCamera faceCamera;
        public Collider collider;
        public LayerMask terrainLayer;
        public PlayerAnimator animator;

        [Header("Movement Settings")] public float moveSpeed;
        public bool snapToGround;
        public float snapSpeed;
        public float groundCheckerDistanceMultiplier;

        [Header("Dash Settings")] public float dashVelocity;
        public float dashDuration;
        public float dashCooldown;
        public AudioSource dashAudioSource;
    }

    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private PlayerConfig config;
        [SerializeField] private AudioSource damageAudioSource;
        [SerializeField] private AudioSource healAudioSource;
        [SerializeField] private AudioSource attackAudioSource;
        [SerializeField] private AudioSource deathAudioSource;
        [SerializeField] private AudioSource footstepAudioSource;

        [SerializeField] private bool ignoreDashInput;

        [FormerlySerializedAs("playerDeathExpression")] [SerializeField]
        private Sprite antiVirusSprite;

        [Header("Attack Settings")] [SerializeField]
        private float attackCooldown;

        public global::StateMachine StateMachine { get; private set; }

        private Vector2 _input;
        private Vector3 _lastNotNullMovementDir;

        private float _lastAttackTimestamp;

        private Transform _cameraTransform;
        private bool _isDying;

        private SaveSystem.AimType _aimType;

        public float Hp { get; private set; }


        private void Awake()
        {
            StateMachine = new global::StateMachine(
                new PlayerIdleState(this),
                new PlayerRunningState(this),
                new DashState(this),
                new PlayerNoMovementState(this)
            );

            _cameraTransform = Camera.main.transform;

            Hp = PlayerStats.BaseHp;
            _isDying = false;
        }

        private void Start()
        {
            InputManager.Instance.InputActions.Player.Attack.performed += AttackOnPerformed;
            InputManager.Instance.InputActions.Player.Dash.performed += DashOnPerformed;

            UpdateAimType();

            // Set initial state
            StateMachine.TransitionToState<PlayerIdleState>();
        }

        private void DashOnPerformed(InputAction.CallbackContext obj)
        {
            if (ignoreDashInput) return;
            ((PlayerState)StateMachine.ActiveState).OnDashInput();
        }

        private void AttackOnPerformed(InputAction.CallbackContext obj)
        {
            ((PlayerState)StateMachine.ActiveState).OnAttackInput();
        }

        private void Update()
        {
            _input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            if (_input.y != 0)
            {
                config.animator.SetVerticalDirection(_input.y);
            }

            if (MovementDirection != Vector3.zero)
            {
                _lastNotNullMovementDir = MovementDirection;
            }

            StateMachine.Update();
        }

        private void FixedUpdate()
        {
            if (config.snapToGround)
            {
                HandleGroundAlignment();
            }

            StateMachine.FixedUpdate();
        }

        public void DisableMovement()
        {
            StateMachine.TransitionToState<PlayerNoMovementState>();
        }

        public void EnableMovement()
        {
            StateMachine.TransitionToState<PlayerIdleState>();
        }

        /// <summary>
        /// Snaps the player to the floor
        /// </summary>
        private void HandleGroundAlignment()
        {
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity,
                    config.terrainLayer))
            {
                float targetY = hit.point.y + config.collider.bounds.extents.y;
                float newY = Mathf.Lerp(
                    config.rigidbody.position.y,
                    targetY,
                    Time.fixedDeltaTime * config.snapSpeed
                );

                config.rigidbody.MovePosition(new Vector3(
                    config.rigidbody.position.x,
                    newY,
                    config.rigidbody.position.z
                ));
            }
        }

        public void Attack()
        {
            if (Time.time - _lastAttackTimestamp <= attackCooldown) return;

            var attack = GameManager.Instance.SlashAttack;
            attack.transform.position = transform.position;
            attack.transform.SetParent(transform);

            config.animator.TriggerAttack();


            if (_aimType == SaveSystem.AimType.Keyboard)
            {
                // Point attack towards movement direction
                float xDir = HorizontalInput != 0 ? HorizontalInput : _lastNotNullMovementDir.x;
                float yDir = VerticalInput != 0 ? -VerticalInput : -_lastNotNullMovementDir.z;

                Vector2 attackDir = new Vector2(xDir, yDir).normalized;
                attack.transform.rotation = Quaternion.identity;
                attack.transform.Rotate(Vector3.up * (Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg));
            }
            else
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                

                if (Physics.Raycast(ray, out RaycastHit hit, 300, config.terrainLayer))
                {
                    Vector3 dir = hit.point - transform.position;
                    dir.y = 0;
                    attack.transform.rotation = Quaternion.LookRotation(dir.normalized) * Quaternion.Euler(0, -90f, 0);
                    Debug.DrawRay(transform.position, dir.normalized * 2f, Color.red, 1f);
                }
                else
                {
                    Debug.Log("No hit raycast hit for mouse aim type");
                    Vector3 mousePos = Input.mousePosition;
                    Vector2 centeredMousePos = new Vector2(
                        mousePos.x - Screen.width / 2f,
                        mousePos.y - Screen.height / 2f
                    );
                    Vector3 dir = new Vector3(
                        Mathf.Sign(centeredMousePos.x),
                        0,
                        Mathf.Sign(centeredMousePos.y)
                    );

                    attack.transform.rotation = Quaternion.LookRotation(dir.normalized) * Quaternion.Euler(0, -90f, 0);
                }
            }


            attackAudioSource.pitch = UnityEngine.Random.Range(0.4f, 0.8f);
            attackAudioSource.Play();

            _lastAttackTimestamp = Time.time;
        }

        private void OnDestroy()
        {
            InputManager.Instance.InputActions.Player.Disable();
            InputManager.Instance.InputActions.Player.Attack.performed -= AttackOnPerformed;
            InputManager.Instance.InputActions.Player.Dash.performed -= DashOnPerformed;
        }

        public void Damage(float damage)
        {
            if (_isDying) return;

            Hp -= damage;

            damageAudioSource.Play();
            GameManager.Instance.HitStop(0.3f, 0.1f, 0.02f);
            CameraManager.Instance.BigShake();

            if (Hp <= 0)
            {
                StartCoroutine(Die());
            }
        }

        private IEnumerator Die()
        {
            _isDying = true;

            DisableMovement();
            GameManager.Instance.HitStop(0.5f, 0.05f, 0.03f);
            yield return new WaitForSeconds(0.7f);
            CameraManager.Instance.ZoomToPlayer();
            deathAudioSource.Play();

            config.animator.TriggerDeath();

            yield return new WaitForSeconds(2f);

            if (!TopUIDialogue.Instance.IsBusy)
            {
                TopUIDialogue.Instance.ShowDialogue(
                    new TopUIDialogue.DialogueSentence("Virus Detected... Cleaning process activated...",
                        antiVirusSprite), new WaitForSeconds(4));
            }

            yield return new WaitForSeconds(3f);

            DeathScreen.Instance.Show();
        }

        public void UpdateAimType()
        {
            _aimType = SaveSystem.GetData().AimType;
        }

        public void PlayFootstepSfx()
        {
            footstepAudioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            footstepAudioSource.Play();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Heart"))
            {
                float tmpHp = Hp;
                tmpHp += 20;
                Hp = Mathf.Clamp(tmpHp, 0, PlayerStats.BaseHp);
                healAudioSource.Play();
                other.enabled = false;
                other.transform.DOScale(Vector3.zero, 1).OnComplete(() =>
                {
                    Destroy(other.transform.parent.gameObject);
                });
            }
        }

        #region Public accessors

        public PlayerConfig Config => config;
        public float HorizontalInput => _input.x;
        public float VerticalInput => _input.y;

        public Vector3 MovementDirection
        {
            get
            {
                float yaw = _cameraTransform.eulerAngles.y;

                // Snap yaw to the nearest 90 degrees
                float roundedYaw = Mathf.Round(yaw / 90f) * 90f;

                // Convert to direction vectors
                Vector3 snappedForward = Quaternion.Euler(0, roundedYaw, 0) * Vector3.forward;
                Vector3 snappedRight = Quaternion.Euler(0, roundedYaw, 0) * Vector3.right;

                // Compute movement based on input
                Vector3 movement = snappedRight * HorizontalInput + snappedForward * VerticalInput;

                return movement.normalized; // Keep movement normalized
            }
        }

        public bool MoveInput => HorizontalInput != 0 || VerticalInput != 0;
        public static LayerMask Layer => LayerMask.NameToLayer("Player");

        #endregion
    }
}
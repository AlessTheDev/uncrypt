using System;
using System.Collections;
using Companion.States;
using Terminal;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Companion
{
    [System.Serializable]
    public class FollowPlayerConfig
    {
        public float speed;
        public float maxDistanceFromPlayer;
    }

    [System.Serializable]
    public class LeadPlayerConfig
    {
        public float maxDistanceFromPlayer;
        public NavMeshAgent agent;
        public Vector3 targetPosition;
        public Action onTargetReached;
        public CompanionNavigator companionNavigator;
    }

    public class CompanionController : MonoBehaviour
    {
        [SerializeField] private FollowPlayerConfig followPlayerConfig;
        [SerializeField] private LeadPlayerConfig leadPlayerConfig;
        [SerializeField] private FaceCamera _faceCamera;

        private bool _teachControls;
        private bool _isTeachingControls;

        public StateMachine StateMachine { get; private set; }

        private void Start()
        {
            StateMachine = new StateMachine(
                new FollowPlayerState(this),
                new LeadPlayerState(this),
                new NoMovementState(this)
            );

            if (SaveSystem.GetData().StoryCheckpoint == SaveSystem.StoryCheckpoint.None)
            {
                _teachControls = true;
            }
            
            leadPlayerConfig.agent.enabled = false;
            
            GameManager.Instance.OnPlayerEntersSafeZone.AddListener(() =>
            {
                StopAllCoroutines();
                TopUIDialogue.Instance.Reset();
                _isTeachingControls = false;
            });
        }

        private void Update()
        {
            StateMachine.Update();
        }

        private void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }

        private void LateUpdate()
        {
            StateMachine.LateUpdate();
        }

        public void LeadPlayerTo(Vector3 targetPos, Action onTargetReached = null)
        {
            leadPlayerConfig.targetPosition = targetPos;
            leadPlayerConfig.onTargetReached = onTargetReached;
            StateMachine.TransitionToState<LeadPlayerState>();
        }

        public void DisableMovement()
        {
            StateMachine.TransitionToState<NoMovementState>();
        }

        public FollowPlayerConfig FollowPlayerConfig => followPlayerConfig;
        public LeadPlayerConfig LeadPlayerConfig => leadPlayerConfig;
        public FaceCamera FaceCamera => _faceCamera;

        private void OnTriggerEnter(Collider other)
        {
            if (_teachControls && !_isTeachingControls && !TerminalManager.Instance.Writer.isActiveAndEnabled)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    StartCoroutine(ControlsTutorial());
                    _isTeachingControls = true;
                }
            }
        }

        private IEnumerator ControlsTutorial()
        {
            yield return
                TopUIDialogue.Instance.ShowDialogueCoroutine(
                    new TopUIDialogue.DialogueSentence("Oh no! They noticed us!"),
                    new WaitForSeconds(0.5f)
                );

            yield return
                TopUIDialogue.Instance.ShowDialogueCoroutine(
                    new TopUIDialogue.DialogueSentence(
                        "Quickly! Use the left mouse button to attack them, or Z if you're playing with the arrows"),
                    new WaitUntil(InputManager.Instance.InputActions.Player.Attack.IsPressed)
                );

            yield return
                TopUIDialogue.Instance.ShowDialogueCoroutine(
                    new TopUIDialogue.DialogueSentence(
                        "Well done, now try to perform a dash with SHIFT or R2"),
                    new WaitUntil(InputManager.Instance.InputActions.Player.Dash.IsPressed)
                );

            _teachControls = false;
            _isTeachingControls = false;
            yield return TopUIDialogue.Instance.ShowDialogueCoroutine(
                new TopUIDialogue.DialogueSentence(
                    "Very good, fight because enemies can give you health supplies, but don't get too distracted!"), new WaitForSeconds(2)
            );
        }
    }
}
using System;
using System.Collections;
using DialogueSystem;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NPCs
{
    // ReSharper disable once InconsistentNaming
    public class BaseNPC : MonoBehaviour, IDialogueExecutor
    {
        public enum NpcDialogueMode
        {
            Normal,
            Random,
            Fallback,
        }

        [SerializeField] private Interactable interactable;
        [SerializeField] private Animator animator;

        [Header("Dialogue Settings")] [SerializeField]
        private NpcDialogueMode dialogueMode = NpcDialogueMode.Normal;

        [Tooltip("Unique ID identifying this NPC in the dialogue system.")] [SerializeField]
        private string characterId;

        [Tooltip("Display name of the character shown during dialogue.")] [SerializeField]
        private string characterDisplayName;

        [Tooltip("ID of the initial dialogue entry for this NPC.")] [SerializeField, HideInInspector]
        private string dialogueId; // Valid for normal and fallback dialogue mode

        [Tooltip("Set of dialogue ids that will be randomly selected on interaction")] [SerializeField, HideInInspector]
        private string[] randomDialogueIds; // Valid for Random only

        [Tooltip("Set of dialogue ids that will be randomly selected on the second interaction")] [SerializeField, HideInInspector]
        private string[] randomFallbackDialogueIds; // Valid for Fallback only

        private bool _acceptInteractions;
        private bool _hasPlayedDialogueBefore;

        private void Start()
        {
            _acceptInteractions = true;
            _hasPlayedDialogueBefore = false;
            DialogueManager.Instance.RegisterDialogueExecutor(characterId, this);
            interactable.InteractUI.Hide();
            OnStart();
        }

        protected virtual void OnStart(){}

        public void Talk(string sentence)
        {
            DialogueUIManager.Instance.Display(characterDisplayName, sentence);
        }

        public virtual void ExecuteAction(string action)
        {
            if (string.IsNullOrEmpty(action)) return;
            String[] actionParameters = action.Split('|');

            switch (actionParameters[0])
            {
                case "PlayAnimation":
                    animator.Play(actionParameters[1]);
                    break;
                case "JumpTo":
                    DialogueManager.Instance.OverrideDialogue(actionParameters[1]);
                    break;
                case "ResetMemory":
                    _hasPlayedDialogueBefore = false;
                    break;
            }
        }

        public void SetOptions(DialogueOption[] options)
        {
            DialogueUIManager.Instance.LoadOptions(options, this);
        }

        public void OnInteract()
        {
            if (!_acceptInteractions) return;

            _acceptInteractions = false;
            interactable.InteractUI.Hide();

            switch (dialogueMode)
            {
                case NpcDialogueMode.Normal:
                    StartCoroutine(DialogueManager.Instance.ParseDialogue(dialogueId));
                    break;
                case NpcDialogueMode.Random:
                    StartCoroutine(
                        DialogueManager.Instance.ParseDialogue(
                            randomDialogueIds[Random.Range(0, randomDialogueIds.Length)]));
                    break;
                case NpcDialogueMode.Fallback:
                    StartCoroutine(DialogueManager.Instance.ParseDialogue(
                        _hasPlayedDialogueBefore
                            ? randomFallbackDialogueIds[Random.Range(0, randomFallbackDialogueIds.Length)]
                            : dialogueId)
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            _hasPlayedDialogueBefore = true;
        }

        public virtual void OnDialogueEnd(string dialogueId1)
        {
            interactable.InteractUI.Show();
            _acceptInteractions = true;
        }

        public bool CanDialogueProceed() => DialogueUIManager.Instance.CanDialogueProceed();
        public Transform GetTransform() => transform;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(BaseNPC), true)]
    public class BaseNpcEditor : Editor
    {
        private SerializedProperty _dialogueMode;
        private SerializedProperty _dialogueId;
        private SerializedProperty _randomDialogueIds;
        private SerializedProperty _randomFallbackDialogueIds;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Cache props
            _dialogueMode = serializedObject.FindProperty("dialogueMode");
            _dialogueId = serializedObject.FindProperty("dialogueId");
            _randomDialogueIds = serializedObject.FindProperty("randomDialogueIds");
            _randomFallbackDialogueIds = serializedObject.FindProperty("randomFallbackDialogueIds");

            var property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;

                // Skip the ones we’ll handle manually below
                if (property.name == "_dialogueId" || property.name == "_randomDialogueIds" ||
                    property.name == "_randomFallbackDialogueIds")
                    continue;

                EditorGUILayout.PropertyField(property, true);
            }

            // Now conditionally draw the relevant fields based on mode
            EditorGUILayout.Space();
            var mode = (BaseNPC.NpcDialogueMode)_dialogueMode.enumValueIndex;

            switch (mode)
            {
                case BaseNPC.NpcDialogueMode.Normal:
                    EditorGUILayout.PropertyField(_dialogueId);
                    break;
                case BaseNPC.NpcDialogueMode.Random:
                    EditorGUILayout.PropertyField(_randomDialogueIds);
                    break;
                case BaseNPC.NpcDialogueMode.Fallback:
                    EditorGUILayout.PropertyField(_dialogueId);
                    EditorGUILayout.PropertyField(_randomFallbackDialogueIds);
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
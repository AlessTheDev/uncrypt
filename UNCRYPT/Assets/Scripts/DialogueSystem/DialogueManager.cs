using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace DialogueSystem
{
    public class DialogueManager : SceneSingleton<DialogueManager>
    {
        [SerializeField] private TextAsset dialoguesAsset;

        private readonly Dictionary<string, Dialogue> _dialogues = new();
        private readonly Dictionary<string, IDialogueExecutor> _executors = new();

        private string _currentDialogueId;

        protected override void OnAwake()
        {
            Dialogue[] dialoguesFromFile = Utilities.JsonHelper.FromJson<Dialogue>(dialoguesAsset.text);

            foreach (Dialogue dialogue in dialoguesFromFile)
            {
                if (_dialogues.ContainsKey(dialogue.dialogueId))
                {
                    Debug.LogError($"Duplicate Dialogue: {dialogue.dialogueId}");
                }

                _dialogues.Add(dialogue.dialogueId, dialogue);
            }
        }

        /// <summary>
        /// Registers a dialogue executor for a specific character ID.
        /// </summary>
        /// <param name="id">The character ID.</param>
        /// <param name="executor">The executor to associate with the character ID.</param>
        public void RegisterDialogueExecutor(string id, IDialogueExecutor executor)
        {
            if (_executors.ContainsKey(id))
            {
                Debug.LogError($"Multiple Dialogues Executors have been registred under the same id: {id}");
            }

            _executors[id] = executor;
        }

        /// <summary>
        /// Retrieves a dialogue by its ID.
        /// </summary>
        /// <param name="dialogueId">The ID of the dialogue to retrieve.</param>
        /// <returns>The dialogue associated with the given ID, or null if not found.</returns>
        public Dialogue GetDialogue(string dialogueId)
        {
            if (!_dialogues.TryGetValue(dialogueId, out var dialogue))
            {
                Debug.LogError($"Dialogue with ID {dialogueId} not found.");
                return null;
            }

            return dialogue;
        }

        // Allows to "inject" a dialogue 
        public void OverrideDialogue(string dialogueId)
        {
            _currentDialogueId = dialogueId;
        }

        public IEnumerator ParseDialogue(string dialogueId, bool enableMovementBack = true,
            bool enableInputsBack = true)
        {
            InputManager.Instance.InputActions.Disable();

            DialogueUIManager dialogueUIManager = DialogueUIManager.Instance;
            dialogueUIManager.ShowContainer();

            GameManager.Instance.Player.DisableMovement();
            GameManager.Instance.OnPlayerEntersSafeZone.Invoke();

            bool waitForDialogueContainerFadeIn = true;

            _currentDialogueId = dialogueId;
            while (!string.IsNullOrEmpty(_currentDialogueId) &&
                   !_currentDialogueId.Equals("NONE", StringComparison.OrdinalIgnoreCase))
            {
                Dialogue d = GetDialogue(_currentDialogueId);
                IDialogueExecutor ex = _executors[d.characterId];

                CameraManager.Instance.DialogueView(ex.GetTransform()); // Focus on the speaking character

                if (waitForDialogueContainerFadeIn)
                {
                    yield return new WaitForSeconds(dialogueUIManager.ContainerFadeInDuration);
                    waitForDialogueContainerFadeIn = false;
                }

                ex.Talk(d.sentence); // Trigger dialogue

                // Execute actions if any
                if (d.actions != null)
                {
                    foreach (var action in d.actions)
                    {
                        ex.ExecuteAction(action);
                    }
                }

                // Set dialogue options
                if (d.options != null)
                {
                    ex.SetOptions(d.options);
                }

                // Move to the next dialogue in the sequence
                _currentDialogueId = d.nextDialogue;

                // Wait until the executor allows us to proceed
                yield return new WaitUntil(ex.CanDialogueProceed);
            }

            // End of dialogue sequence
            foreach (var executor in _executors.Values)
            {
                executor.OnDialogueEnd(dialogueId);
            }

            if (enableInputsBack)
            {
                InputManager.Instance.InputActions.Enable();
            }

            dialogueUIManager.HideContainer();

            if (enableMovementBack)
            {
                GameManager.Instance.Player.EnableMovement();
            }

            GameManager.Instance.OnPlayerExitsSafeZone.Invoke();

            CameraManager.Instance.FollowPlayer(); // Restore camera to follow player
        }
    }
}
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

namespace DialogueSystem
{
    public class DialogueUIManager : PersistentSingleton<DialogueUIManager>
    {
        private static readonly int Show = Animator.StringToHash("Show");
        [SerializeField] private OptionUI[] optionButtons;

        [SerializeField] private GameObject dialogueContainer;
        [SerializeField] private GameObject optionsContainer;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private TextMeshProUGUI characterName;
        [SerializeField] private Animator continueDialogue;

        [SerializeField] private float timeBetweenCharacters = 0.01f;
        [SerializeField] private float pauseTime = 0.5f;

        [Header("UI Animation Settings")] [SerializeField]
        private float verticalZoomInDuration;

        [SerializeField] private float horizontalZoomInDuration;

        [SerializeField] private float delayBetweenOptionAnimations;

        private bool _isWriting;
        private bool _isWaitingForInput;
        private bool _waitForOptionClick;

        private bool _inputKeyIsDown;

        protected override void OnAwake()
        {
            foreach (var optionButton in optionButtons)
            {
                optionButton.gameObject.SetActive(false);
            }

            dialogueContainer.SetActive(false);
            optionsContainer.SetActive(false);

            characterName.text = string.Empty;
            dialogueText.text = string.Empty;

            _isWriting = false;
            _isWaitingForInput = false;
            _waitForOptionClick = false;
            _inputKeyIsDown = false;
        }

        private void Start()
        {
            continueDialogue.gameObject.SetActive(false);

            InputManager.Instance.InputActions.Dialogue.Proceed.performed += ProceedOnPerformed;
        }

        private void OnDestroy()
        {
            InputManager.Instance.InputActions.Dialogue.Proceed.performed -= ProceedOnPerformed;
        }

        private void ProceedOnPerformed(InputAction.CallbackContext obj)
        {
            _inputKeyIsDown = true;
        }

        public void Display(string characterName, string sentence)
        {
            this.characterName.text = characterName;
            StartCoroutine(WriteToDialogueBox(sentence.Replace("{playerName}", SaveSystem.PlayerName)));
        }

        private IEnumerator WriteToDialogueBox(string text)
        {
            const char pauseCharacter = '&';

            _isWriting = true;
            dialogueText.text = "";
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (_inputKeyIsDown && i > 40)
                {
                    if (c != pauseCharacter)
                    {
                        dialogueText.text += c;
                    }

                    continue;
                }

                if (c == pauseCharacter)
                {
                    yield return new WaitForSeconds(pauseTime);
                }
                else
                {
                    dialogueText.text += c;
                    yield return new WaitForSeconds(timeBetweenCharacters);
                }
            }

            _isWaitingForInput = true;
            _isWriting = false;

            if (_waitForOptionClick)
            {
                yield return new WaitWhile(() => _waitForOptionClick);
            }
            else
            {
                _inputKeyIsDown = false;
                ShowContinueLabel();
                yield return new WaitUntil(() => _inputKeyIsDown);
                _inputKeyIsDown = false;
                HideContinueLabel();
            }

            _isWaitingForInput = false;
        }

        public void ShowContainer()
        {
            dialogueText.text = "";

            InputManager.Instance.InputActions.Dialogue.Enable();

            Vector3 initialContainerScale = dialogueContainer.transform.localScale;
            optionsContainer.SetActive(true);
            dialogueContainer.SetActive(true);

            dialogueContainer.transform.localScale = Vector3.zero;
            dialogueContainer.transform
                .DOScale(new Vector3(initialContainerScale.x * .01f, initialContainerScale.y, initialContainerScale.z),
                    verticalZoomInDuration) // Scale vertically
                .OnComplete(() =>
                {
                    dialogueContainer.transform.DOScale(initialContainerScale,
                        horizontalZoomInDuration); // And then horizontally
                });
        }

        public void HideContainer()
        {
            InputManager.Instance.InputActions.Dialogue.Disable();
            dialogueContainer.SetActive(false);
            optionsContainer.SetActive(false);
        }

        public void LoadOptions(DialogueOption[] options, IDialogueExecutor executor)
        {
            if (options == null || options.Length == 0) return;

            _waitForOptionClick = true;

            StartCoroutine(ShowOptionsAfterTheDialogueIsWrittenDown(options, executor));
        }

        private IEnumerator ShowOptionsAfterTheDialogueIsWrittenDown(DialogueOption[] options,
            IDialogueExecutor executor)
        {
            yield return new WaitWhile(() => _isWriting);
            for (int i = 0; i < options.Length; i++)
            {
                OptionUI optionButton = optionButtons[i];
                DialogueOption option = options[i];

                float animDelay = delayBetweenOptionAnimations * (options.Length - i);
                optionButton.Enable(option.optionName, () =>
                {
                    if (_isWriting) return;

                    foreach (var action in option.optionActions)
                    {
                        executor.ExecuteAction(action);
                    }

                    CameraManager.Instance.EventSystem.SetSelectedGameObject(null);

                    _waitForOptionClick = false;
                    ClearOptions();
                }, animDelay);
            }
        }

        public void ClearOptions()
        {
            foreach (var optionButton in optionButtons)
            {
                optionButton.Disable();
            }
        }

        private void ShowContinueLabel()
        {
            continueDialogue.gameObject.SetActive(true);
            continueDialogue.SetBool(Show, true);
        }

        private void HideContinueLabel()
        {
            continueDialogue.SetBool(Show, false);
        }

        public bool CanDialogueProceed() => !_isWriting && !_isWaitingForInput;

        public float ContainerFadeInDuration => verticalZoomInDuration + horizontalZoomInDuration;
    }
}
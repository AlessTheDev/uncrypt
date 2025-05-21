using System;
using System.Collections;
using DialogueSystem;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace UI
{
    public class TopUIDialogue : PersistentSingleton<TopUIDialogue>
    {
        public class DialogueSentence
        {
            public readonly string Sentence;
            public readonly Sprite Expression;

            public DialogueSentence(string sentence, Sprite expression = null)
            {
                Sentence = sentence;
                Expression = expression ?? DefaultSprite;
            }
        }
        
        [SerializeField] private Sprite defaultSprite;

        [SerializeField] private CanvasGroup dialogueContainer;
        [SerializeField] private TextMeshProUGUI dialogueText;
        [SerializeField] private Image expressionImage;

        [SerializeField] private float entranceDuration = 1;
        [SerializeField] private float pauseTime;
        [SerializeField] private float timeBetweenCharacters;

        private float _showHeightPos;
        private Canvas _canvas;

        private RectTransform _rectTransform;

        private bool _isBusy;

        protected override void OnAwake()
        {
            DefaultSprite = defaultSprite;
            
            _canvas = gameObject.GetComponent<Canvas>();
            _canvas.enabled = false;
            _rectTransform = dialogueContainer.GetComponent<RectTransform>();
            
            _showHeightPos = _rectTransform.anchoredPosition.y;

            dialogueContainer.gameObject.SetActive(false);
            Reset();
        }

        public void Reset()
        {
            // Move the entire dialogue container down
            _rectTransform.anchoredPosition = new Vector2(
                _rectTransform.anchoredPosition.x,
                -Mathf.Abs(_showHeightPos)
            );

            _rectTransform.localScale = Vector3.one * 0.5f;
            dialogueContainer.alpha = 0;
            
            _isBusy = false;
        }

        public void ShowDialogue(DialogueSentence[] dialogues, YieldInstruction condition)
        {
            StartCoroutine(ShowDialogueCoroutine(dialogues, condition));
        }

        public void ShowDialogue(DialogueSentence sentence, YieldInstruction condition = null)
        {
            StartCoroutine(ShowDialogueCoroutine(new[] { sentence }, condition));
        }

        public IEnumerator ShowDialogueCoroutine(DialogueSentence sentence, YieldInstruction condition = null)
        {
            return ShowDialogueCoroutine(new[] { sentence }, condition);
        }

        public IEnumerator ShowDialogueCoroutine(DialogueSentence[] sentences, YieldInstruction condition)
        {
            return ShowDialogueCoroutine(sentences, condition as object);
        }

        public IEnumerator ShowDialogueCoroutine(DialogueSentence sentence, WaitUntil condition = null)
        {
            return ShowDialogueCoroutine(new[] { sentence }, condition);
        }

        public void ShowDialogue(DialogueSentence[] dialogues, WaitForSeconds condition)
        {
            StartCoroutine(ShowDialogueCoroutine(dialogues, condition));
        }

        public void ShowDialogue(DialogueSentence[] dialogues, WaitUntil condition)
        {
            StartCoroutine(ShowDialogueCoroutine(dialogues, condition));
        }

        private IEnumerator ShowDialogueCoroutine(DialogueSentence[] dialogues, object condition)
        {
            _canvas.enabled = true;

            _isBusy = true;
            dialogueContainer.gameObject.SetActive(true);

            _rectTransform.DOAnchorPos(new Vector2(_rectTransform.anchoredPosition.x, _showHeightPos), entranceDuration)
                .SetEase(Ease.InSine);
            _rectTransform.DOScale(Vector3.one, entranceDuration).SetEase(Ease.InSine);
            dialogueContainer.DOFade(1, entranceDuration).SetEase(Ease.InSine);
            yield return new WaitForSeconds(entranceDuration);

            foreach (var dialogueSentence in dialogues)
            {
                dialogueText.text = "";
                if (dialogueSentence.Expression)
                {
                    expressionImage.sprite = dialogueSentence.Expression;
                }

                foreach (char c in dialogueSentence.Sentence)
                {
                    if (c == '&')
                    {
                        yield return new WaitForSeconds(pauseTime);
                    }
                    else
                    {
                        dialogueText.text += c;
                        yield return new WaitForSeconds(timeBetweenCharacters);
                    }
                }

                yield return condition;
            }

            _rectTransform
                .DOAnchorPos(new Vector2(_rectTransform.anchoredPosition.x, -_showHeightPos), entranceDuration)
                .SetEase(Ease.OutSine);
            _rectTransform.DOScale(Vector3.one * 0.5f, entranceDuration).SetEase(Ease.OutSine);
            dialogueContainer.DOFade(0, entranceDuration).SetEase(Ease.OutSine);

            yield return new WaitForSeconds(entranceDuration);

            dialogueContainer.gameObject.SetActive(false);
            _canvas.enabled = false;
            _isBusy = false;
        }

        public bool IsDialogueActive => dialogueContainer.gameObject.activeSelf;
        
        public bool IsBusy => _isBusy;

        private static Sprite DefaultSprite
        {
            get;
            set;
        }
    }
}
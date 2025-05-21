using System.Collections;
using DefaultNamespace;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UI.FileSystemPuzzle
{
    public class FileSystemPuzzleUI : MonoBehaviour
    {
        [SerializeField] private string puzzleString = "OmqemdQzodkbfuaz";
        [SerializeField] private string solvedString = "CaesarEncryption";
        
        private const string Alphabet = "abcdefghijklmnopqrstuvwxyz";

        [Header("Decipher Tool")]
        [SerializeField] private Transform puzzleStringContainer;
        [SerializeField] private Transform inputContainer;
        [SerializeField] private Transform puzzleStringBox;
        [SerializeField] private TMP_InputField puzzleInputBox;
        [SerializeField] private Color defaultTextColor;
        [SerializeField] private Color solvedTextColor;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private RectTransform helpPanel;
        [SerializeField] private float helpPanelFadeDuration;
        [SerializeField] private Ease helpPanelFadeEase;
        [SerializeField] private EventSystem eventSystem;

        private string _currentUserInput;
        
        [Header("Alphabet Settings")]
        [SerializeField] private PuzzleTextBox alphabetBox;
        [SerializeField] private Transform alphabetPivotNormal;
        [SerializeField] private Transform alphabetPivotOffset;
        [SerializeField] private TextMeshProUGUI offsetText;
        
        private int _alphabetOffset;
        private PuzzleTextBox[] _translatedAlphabet;
        private float _alphabetBoxSize;
        private float _alphabetContainerSize;

        private float _targetHelpPanelX;

        private bool _isCompleted;

        private void Start()
        {
            _isCompleted = false;
            
            InitializeAlphabet();
            InitializeCypheredText();
            InitializeInputBoxes();

            _targetHelpPanelX = helpPanel.anchoredPosition.x * -1;
            helpPanel.anchoredPosition = new Vector2(_targetHelpPanelX, helpPanel.anchoredPosition.y);
        }

        private void InitializeInputBoxes()
        {
            // Set default user input 
            for (int i = 0; i < solvedString.Length; i++)
            {
                _currentUserInput += " ";
            }

            for (int i = 0; i < solvedString.Length; i++)
            {
                TMP_InputField box = Instantiate(puzzleInputBox, inputContainer.transform);
                int index = i;
                box.onValueChanged.AddListener((value) =>
                {
                    if (value == "" || _isCompleted) return;

                    _currentUserInput = _currentUserInput[..index] + value + _currentUserInput[(index+1)..];

                    if (solvedString[index].ToString().Equals(value))
                    {
                        box.textComponent.DOColor(solvedTextColor, animationDuration).SetEase(Ease.Linear);

                        if (index + 1 < inputContainer.childCount)
                        {
                            eventSystem.SetSelectedGameObject(inputContainer.GetChild(index + 1).gameObject);
                        }
                    }
                    else
                    {
                        box.textComponent.DOColor(defaultTextColor, animationDuration).SetEase(Ease.Linear);
                    }

                    if (solvedString.Equals(_currentUserInput))
                    {
                        StartCoroutine(SwitchToFinalScene());
                    }
                });
            }
        }

        private IEnumerator SwitchToFinalScene()
        {
            yield return TopUIDialogue.Instance.ShowDialogueCoroutine(new []
            {
                new TopUIDialogue.DialogueSentence("Very good, the password has been decrypted!"),
                new TopUIDialogue.DialogueSentence("You're really a hacker now"),
                new TopUIDialogue.DialogueSentence("Now we should be able to return to the hub, we shouldn't have any problems..."),
                new TopUIDialogue.DialogueSentence("But something doesn't seem right..."),
            }, new WaitForSeconds(.5f));

            yield return new WaitForSeconds(.5f);
            _ = SceneFader.Instance.SwitchScene(Scenes.Final);
        }
        
        private void InitializeCypheredText()
        {
            for (int i = 0; i < puzzleString.Length; i++)
            {
                TextMeshProUGUI box = Instantiate(puzzleStringBox, puzzleStringContainer.transform).GetComponentInChildren<TextMeshProUGUI>();
                box.text = puzzleString[i].ToString();
            }
        }
        private void InitializeAlphabet()
        {
            PuzzleTextBox[] alphabet = new PuzzleTextBox[Alphabet.Length];
            _translatedAlphabet = new PuzzleTextBox[Alphabet.Length];
            for (int i = 0; i < alphabet.Length; i++)
            {
                alphabet[i] = Instantiate(alphabetBox);
                alphabet[i].text.text = Alphabet[i].ToString();
                alphabet[i].transform.SetParent(alphabetPivotNormal);
                
                _translatedAlphabet[i] = Instantiate(alphabetBox);
                _translatedAlphabet[i].text.text = Alphabet[i].ToString();
                _translatedAlphabet[i].transform.SetParent(alphabetPivotOffset);
            }
            _alphabetBoxSize = alphabet[0].GetComponent<RectTransform>().rect.width;
            _alphabetContainerSize = _alphabetBoxSize * Alphabet.Length;
            
            AssignLetterPositions(alphabet, alphabetPivotNormal.position, 0);
            AssignLetterPositions(_translatedAlphabet, alphabetPivotOffset.position, _alphabetOffset);
        }

        private void AssignLetterPositions(PuzzleTextBox[] alphabet, Vector2 pivot, int offset)
        {
            Vector2 leftPivot = pivot - Vector2.right * _alphabetContainerSize / 2;
            for (int i = 0; i < alphabet.Length; i++)
            {
                int offsetIndex = (i + offset) % alphabet.Length;
                if (offsetIndex < 0)
                {
                    offsetIndex = alphabet.Length + offsetIndex;
                }
                float xOffset = offsetIndex * _alphabetBoxSize;
                alphabet[i].targetPos = leftPivot + Vector2.right * xOffset;
            }      
        }

        public void IncrementOffset()
        {
            _alphabetOffset++;
            UpdateOffset();
        }
        
        public void DecrementOffset()
        {
            _alphabetOffset--;
            UpdateOffset();
        }

        private void UpdateOffset()
        {
            AssignLetterPositions(_translatedAlphabet, alphabetPivotOffset.position, _alphabetOffset);
            offsetText.text = _alphabetOffset.ToString();
        }

        public void ToggleHelpPanel()
        {
            helpPanel.DOKill();
            _targetHelpPanelX *= -1;
            helpPanel.DOAnchorPosX(_targetHelpPanelX, helpPanelFadeDuration).SetEase(helpPanelFadeEase);
        }
    }
}
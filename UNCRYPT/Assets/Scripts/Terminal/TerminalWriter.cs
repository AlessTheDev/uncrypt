using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Terminal
{
    public class TerminalWriter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textRenderer;
        [SerializeField] private int maxLines;
        [SerializeField] private int maxCharPerLine = 55;
        [SerializeField] private int maxOutputCharPerLine = 90;
        [SerializeField] private RectTransform cursor;

        private readonly List<string> _lines = new();
        private readonly List<string> _commandHistory = new();

        private string _renderedText = "";
        private string _currentCommand = "";

        private int _commandHistoryOffset = 0;
        private int _cursorOffset;

        private int _escPressCount;

        private int CursorOffset
        {
            get => _cursorOffset;
            set =>
                _cursorOffset = Mathf.Clamp(value, -_currentCommand.Length, 0);
        }

        private int CommandHistoryOffset
        {
            get => _commandHistoryOffset;
            set =>
                _commandHistoryOffset = Mathf.Clamp(value, 1, _commandHistory.Count);
        }


        private bool _inputMode;

        public UnityEvent<string> onCommandEntered;

        private void OnEnable()
        {
            InputMode();
        }

        private void Update()
        {
            if (!_inputMode) return;

            ListenToInput();
            UpdateCursor();
        }

        private void ListenToInput()
        {
            if (Input.anyKey)
            {
                foreach (char c in Input.inputString)
                {
                    switch (c)
                    {
                        case '\b':
                        {
                            // Backspace
                            if (_currentCommand.Length > 0 && CursorPos > 0)
                                _currentCommand = _currentCommand[..(CursorPos - 1)] + _currentCommand[CursorPos..];
                            break;
                        }
                        case '\n':
                        case '\r':
                            // Enter
                            Print(Header);
                            _commandHistory.Add(_currentCommand);
                            onCommandEntered?.Invoke(_currentCommand);
                            _currentCommand = string.Empty;
                            _commandHistoryOffset = 0;
                            CursorOffset = 0;
                            break;
                        default:
                            if (_currentCommand.Length < maxCharPerLine)
                            {
                                _currentCommand = _currentCommand[..CursorPos] + c + _currentCommand[CursorPos..];
                            }

                            break;
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                CursorOffset--;
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                CursorOffset++;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                CommandHistoryOffset++;
                TrySetCurrentCommandBasedOnHistory();
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                CommandHistoryOffset--;
                TrySetCurrentCommandBasedOnHistory();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _escPressCount++;
                if (_escPressCount == 2)
                {
                    _currentCommand = string.Empty;
                }
            }
            else if (Input.anyKeyDown)
            {
                _escPressCount = 0;
            }


            textRenderer.text = _renderedText + Header;
        }

        private void TrySetCurrentCommandBasedOnHistory()
        {
            if (_commandHistory.Count == 0) return;
            _currentCommand = _commandHistory[^CommandHistoryOffset];
        }

        private int CursorPos => CursorOffset + _currentCommand.Length;

        private void UpdateCursor()
        {
            cursor.position = GetCursorPosition();

            float lineHeight = GetLineHeight();

            cursor.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lineHeight);
        }

        private float GetLineHeight()
        {
            var textInfo = textRenderer.textInfo;

            return textInfo.lineInfo[0].lineHeight;
        }

        private string Header =>
            $"<color=white>[Virus_35]</color>: <color=#1daecf>{TerminalManager.Instance.FileSystem.CurrentDir.GetPath()}</color>> {_currentCommand}";


        private Vector2 GetCursorPosition()
        {
            var textInfo = textRenderer.textInfo;

            if (textInfo.characterCount == 0)
            {
                // Default position when there's no text
                return textRenderer.transform.position;
            }

            // Iterate over characters to account for spaces properly
            var targetCharacterIndex = textInfo.characterCount - 1 + CursorOffset;
            var targetCharacter = textInfo.characterInfo[targetCharacterIndex];

            // Calculate the vertical center of the character
            float verticalCenter = (targetCharacter.ascender + targetCharacter.descender) / 2;

            float xPos = targetCharacter.topRight.x;

            if (targetCharacter.character == ' ' &&
                (targetCharacterIndex == textInfo.characterCount - 1 || // Order is important!
                 textInfo.characterInfo[targetCharacterIndex + 1].character != ' '))
            {
                xPos += textInfo.characterInfo[1].topRight.x - textInfo.characterInfo[0].topRight.x;
            }

            // Calculate cursor position at the vertical center
            Vector3 localPosition = new Vector3(xPos, verticalCenter, 0);

            // Convert local position to world space
            return textRenderer.transform.TransformPoint(localPosition);
        }

        public void Print(string text)
        {
            List<string> splittedContent = SplitContent(text, maxOutputCharPerLine);

            foreach (string line in splittedContent)
            {
                PrintLine(line);
            }
        }

        #region SplitUtils

        private List<string> SplitContent(string content, int maxCharsPerLine)
        {
            if (GetVisibleLength(content) <= maxCharsPerLine)
            {
                return new List<string>() { content };
            }

            List<string> result = new List<string>();
            int currentPos = 0;

            while (currentPos < content.Length)
            {
                // Start a new line
                int charCount = 0;
                int lineStartPos = currentPos;
                int lastGoodBreakPos = currentPos;

                while (currentPos < content.Length && charCount < maxCharsPerLine)
                {
                    // Check if we're at the start of a tag
                    Match tagMatch = Regex.Match(content.Substring(currentPos), @"<[^>]+>");
                    if (tagMatch.Success && tagMatch.Index == 0)
                    {
                        // Skip the entire tag
                        currentPos += tagMatch.Length;
                        continue;
                    }
                    // Check for space (potential break point)
                    else if (content[currentPos] == ' ')
                    {
                        lastGoodBreakPos = currentPos;
                        charCount++;
                        currentPos++;
                    }
                    // Regular character
                    else
                    {
                        charCount++;
                        currentPos++;
                    }
                }

                // If we couldn't find a good break point, force break at max chars
                if (lastGoodBreakPos == lineStartPos)
                {
                    // Just take what we have so far to avoid infinite loop
                    lastGoodBreakPos = currentPos;
                }

                // Add this line to results
                result.Add(content.Substring(lineStartPos, lastGoodBreakPos - lineStartPos).Trim());

                // Start next line after last good break
                currentPos = lastGoodBreakPos;

                // Skip any spaces at the beginning of next line
                while (currentPos < content.Length && content[currentPos] == ' ')
                {
                    currentPos++;
                }
            }

            return result;
        }

// Helper function to calculate visible text length (excluding tags)
        private int GetVisibleLength(string text)
        {
            // Remove all tags using regex
            string visibleText = Regex.Replace(text, @"<[^>]+>", "");
            return visibleText.Length;
        }

        #endregion

        private void PrintLine(string text)
        {
            foreach (var l in text.Split('\n'))
            {
                _lines.Add(l);
            }

            while (_lines.Count > maxLines)
            {
                _lines.RemoveAt(0);
            }

            Refresh();
        }

        public void Clear(int nLines)
        {
            for (int i = 0; i < nLines; i++)
            {
                _lines.RemoveAt(_lines.Count - 1);
            }

            Refresh();
        }

        public void Clear()
        {
            Clear(_lines.Count);
        }

        private void Refresh()
        {
            _renderedText = "";
            foreach (var line in _lines)
            {
                _renderedText += line + "\n";
            }

            textRenderer.text = _renderedText;
        }

        public void OutputMode()
        {
            _inputMode = false;
            cursor.gameObject.SetActive(false);
        }

        public void InputMode()
        {
            _inputMode = true;
            cursor.gameObject.SetActive(true);
        }

        public string GetLastExecutedCommand() => _commandHistory.Count > 0 ? _commandHistory[^1] : "";
    }
}
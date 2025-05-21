using System;
using System.Collections;
using DefaultNamespace;
using Terminal;
using TMPro;
using UnityEngine;

namespace UI
{
    public class InitialCutsceneController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private int usernameLengthLimit;
        [SerializeField] private float timeBetweenCharacters;
        [SerializeField] private float timeBetweenDots;
        [SerializeField] private PlayerLoading playerLoading;
        [SerializeField] private AudioSource keyAudioSource;

        private bool _collectInput;
        private string _username = "";

        private IEnumerator Start()
        {
            text.text = "";
            
            _collectInput = false;
            playerLoading.UpdatePercentage(0);

            yield return new WaitForSeconds(1f);

            yield return PrintTextAndDots("Analyzing Input");
            yield return PrintTextAndDots("Processing language");
            yield return PrintLine("Language Found!");

            playerLoading.UpdatePercentage(10);
            yield return new WaitForSeconds(1f);
            Clear();

            yield return PrintTextAndDots("Saving to memory");
            yield return PrintTextAndDots("Variable changed successfully");
            yield return PrintTextAndDots("Compiling");
            playerLoading.UpdatePercentage(15);
            yield return PrintLine("Error, missing value on variable \"name\"");

            yield return Print("Please choose a name (type with the keyboard): ", "white");
            _collectInput = true;
            yield return new WaitWhile(() => _collectInput);
            yield return Print("Name Saved Successfully");

            SaveSystem.GameData saveFile = new SaveSystem.GameData(new FileSystem().Root);
            saveFile.PlayerName = _username;
            SaveSystem.Save(saveFile);
            
            playerLoading.UpdatePercentage(30);
            yield return new WaitForSeconds(1f);
            Clear();

            yield return PrintTextAndDots("Preparing malware intelligence");
            yield return PrintLine("Obtaining libraries:\n");
            yield return PrintTextAndDots("Downloading - Strange Behaviour");
            playerLoading.UpdatePercentage(40);
            yield return PrintTextAndDots("Downloading - Silly Robot");
            playerLoading.UpdatePercentage(50);
            yield return PrintTextAndDots("Downloading - Crypto Analyzer");
            playerLoading.UpdatePercentage(60);

            Clear();

            yield return PrintTextAndDots("Done! Training Model");
            playerLoading.UpdatePercentage(80);
            yield return PrintTextAndDots("Creating companion child");
            playerLoading.UpdatePercentage(90);
            yield return PrintTextAndDots("Final Touches");
            playerLoading.UpdatePercentage(100);

            Clear();
            yield return Print("Done!", "#77a8f7");
            NewLine();
            yield return Print("Press any key to continue!", "white");

            yield return new WaitUntil(() => Input.anyKeyDown);
            _ = SceneFader.Instance.SwitchScene(Scenes.Hub);
        }

        private void Update()
        {
            if (_collectInput)
            {
                ListenToInput();
            }
        }

        private void ListenToInput()
        {
            if (!Input.anyKey) return;

            foreach (char c in Input.inputString)
            {
                switch (c)
                {
                    case '\b':
                    {
                        // Backspace
                        if (_username.Length > 0)
                        {
                            text.text = text.text[..^1];
                            _username = _username[..^1];
                        }

                        break;
                    }
                    case '\n':
                    case '\r':
                        // Enter
                        if (_username.Length > 1)
                        {
                            _collectInput = false;
                            NewLine();
                        }

                        break;
                    default:
                        if (!(_username.Length + 1 > usernameLengthLimit))
                        {
                            text.text += c;
                            _username += c;
                        }

                        break;
                }
            }
        }

        private IEnumerator Print(string textToPrint, string color = null)
        {
            foreach (char c in textToPrint)
            {
                if (color != null)
                {
                    text.text += $"<color={color}>";
                    PrintChar(c);
                    text.text += $"</color>";
                }
                else
                {
                    PrintChar(c);
                }

                yield return new WaitForSeconds(timeBetweenCharacters);
            }
        }

        private IEnumerator PrintLine(string textToPrint)
        {
            yield return Print(textToPrint);
            NewLine();
        }

        private void NewLine()
        {
            text.text += '\n';
        }

        private IEnumerator PrintTextAndDots(string textToPrint, int nDots = 3)
        {
            yield return Print(textToPrint);
            yield return PrintDots(nDots);
            NewLine();
        }

        private IEnumerator PrintDots(int n = 3)
        {
            for (int i = 0; i < n; i++)
            {
                PrintChar('.');
                yield return new WaitForSeconds(timeBetweenDots);
            }
        }

        private void PrintChar(char c)
        {
            text.text += c;
            keyAudioSource.Play();
        }

        private void Clear()
        {
            text.text = "";
        }
    }
}
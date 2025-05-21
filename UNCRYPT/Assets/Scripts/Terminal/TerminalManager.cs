using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using DialogueSystem;
using Firewall;
using Player.StateMachine.States;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Terminal
{
    public class TerminalManager : PersistentSingleton<TerminalManager>
    {
        [SerializeField] private TerminalWriter writer;

        private readonly Dictionary<string, TerminalCommand> _registeredCommands = new();

        public FileSystem FileSystem { get; private set; }

        public bool IsReadOnly { get; private set; }
        public bool IsInTutorialMode { get; private set; }

        private bool _needsTutorial;

        private bool _needsNmapTutorial;

        public bool CanClose;

        private bool _hasInitializedSavedData;

        protected override void OnAwake()
        {
            _hasInitializedSavedData = false;
            IsReadOnly = false;
            _needsNmapTutorial = false;
            CanClose = true;

            writer.onCommandEntered.AddListener((command) => StartCoroutine(ExecuteCommand(command)));

            FileSystem = new FileSystem();
            CreateCommands();
        }

        private void Start()
        {
            writer.gameObject.SetActive(false);

            InitializeSavedData();
            SceneManager.activeSceneChanged += (_, _) =>
            {
                InitializeSavedData();
            };

        }

        private void InitializeSavedData()
        {
            if(_hasInitializedSavedData) return;
            
            SaveSystem.GameData data = SaveSystem.GetData();
            
            if(data == null) return;
            
            FileSystem.LoadFromSaveFile();

            _needsTutorial = !data.HasOpenedTerminalBefore;
            _needsNmapTutorial = data.NeedsNMapTutorial;
            _hasInitializedSavedData = true;
        }

        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        private void CreateCommands()
        {
            new HelpCommand();
            new HackCommand();
            new ClearCommand();
            new NMapCommand();
            new SecretCommand();
            new TreeCommand();
            new CurlCommand();
            new SCPCommand();
            new ChangeDirectoryCommand();
            new ListCommand();
            new CatCommand();
            new ExitCommand();
            new RmCommand();
            new SudoRmCommand();
            new EchoCommand();
        }

        public IEnumerator ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) yield break;

            if (_registeredCommands.TryGetValue(command.Split(" ")[0], out TerminalCommand c))
            {
                writer.OutputMode();
                yield return
                    c.Execute(command.Split(" ", StringSplitOptions.RemoveEmptyEntries));
                
                if (IsReadOnly)
                {
                    yield return new WaitUntil(() => Input.anyKeyDown);
                    Close();
                }
                else
                {
                    writer.InputMode();
                }
            }
            else
            {
                writer.Print("Command not found");
            }
        }

        public void RegisterCommand(TerminalCommand command)
        {
            _registeredCommands.Add(command.GetName(), command);
        }

        public void Open(string command, bool readOnly = false)
        {
            GameManager.Instance.OnPlayerEntersSafeZone?.Invoke();
            IsReadOnly = readOnly;
            GameManager.Instance.Player.DisableMovement();

            InputManager.Instance.InputActions.Player.Disable();

            writer.gameObject.SetActive(true);
            StartCoroutine(ExecuteCommand(command));

            if (!readOnly)
            {
                StartCoroutine(CheckTutorial());
            }
        }

        public void Close(bool enableMovement = true)
        {
            if (!CanClose)
            {
                writer.Print(
                    "You can't close the terminal right now, wait for the tutorial to be finished.");
                return;
            }

            writer.gameObject.GetComponent<TerminalUIOpenAnimation>().Close();
            GameManager.Instance.OnPlayerExitsSafeZone?.Invoke();

            if (enableMovement)
            {
                GameManager.Instance.Player.EnableMovement();
                StartCoroutine(ReenableInteractAfterDelay());
            }
            
        }

        private IEnumerator CheckTutorial()
        {
            CanClose = false;

            if (_needsTutorial)
            {
                Debug.Log("Needs Tutorial: " + _needsTutorial);

                SaveSystem.GameData data = SaveSystem.GetData();
                data.HasOpenedTerminalBefore = true;
                SaveSystem.Save(data);

                _needsTutorial = false;

                yield return GeneralTutorial();
            }

            if (_needsNmapTutorial && SceneManager.GetActiveScene().name == Scenes.Firewall)
            {
                SaveSystem.GameData data = SaveSystem.GetData();
                data.NeedsNMapTutorial = false;
                SaveSystem.Save(data);

                _needsNmapTutorial = false;

                yield return NMapTutorial();
            }

            CanClose = true;
        }
        
        private IEnumerator ReenableInteractAfterDelay()
        {
            yield return null; // Wait one frame for the input state to reset.
            InputManager.Instance.InputActions.Player.Enable();
        }


        private IEnumerator GeneralTutorial()
        {
            TopUIDialogue ui = TopUIDialogue.Instance;
            yield return new WaitWhile(() => ui.IsBusy || ui.IsDialogueActive);
            IsInTutorialMode = true;
            Debug.Log("General Tutorial");
            yield return ui.ShowDialogueCoroutine(
                new[]
                {
                    new TopUIDialogue.DialogueSentence("Ok, welcome to the terminal"),
                    new TopUIDialogue.DialogueSentence("The terminal is like a computer"),
                    new TopUIDialogue.DialogueSentence(
                        "But instead of using the graphical interface, this works via CLI"),
                    new TopUIDialogue.DialogueSentence("That is, through commands you type with the keyboard"),
                }, new WaitForSeconds(1));
            yield return ui.ShowDialogueCoroutine(
                new TopUIDialogue.DialogueSentence("Try typing the command 'help'"),
                new WaitUntil(() => NormalizeCommand(writer.GetLastExecutedCommand()) == "help"));

            yield return ui.ShowDialogueCoroutine(
                new TopUIDialogue.DialogueSentence(
                    "If you're unsure how a command works, type the command followed by '--help'"),
                new WaitForSeconds(1));

            yield return ui.ShowDialogueCoroutine(
                new TopUIDialogue.DialogueSentence("For example, try cat --help"),
                new WaitUntil(() => NormalizeCommand(writer.GetLastExecutedCommand()) == "cat --help"));

            ui.ShowDialogue(
                new TopUIDialogue.DialogueSentence("Then when you're done, type exit"),
                new WaitForSeconds(2));
            IsInTutorialMode = false;
        }

        private IEnumerator NMapTutorial()
        {
            TopUIDialogue ui = TopUIDialogue.Instance;

            yield return new WaitWhile(() => ui.IsBusy);
            yield return ui.ShowDialogueCoroutine(
                new[]
                {
                    new TopUIDialogue.DialogueSentence(
                        "Now, thanks to the terminal, we can find vulnerabilities faster"),
                    new TopUIDialogue.DialogueSentence("The command to find the open port is called nmap"),
                }
                , new WaitForSeconds(1));

            yield return ui.ShowDialogueCoroutine(
                new TopUIDialogue.DialogueSentence("Try typing nmap"),
                new WaitUntil(() =>
                    writer.GetLastExecutedCommand().StartsWith("nmap")));
        }

        public TerminalWriter Writer => writer;

        public static string NormalizeCommand(string command)
        {
            return Regex.Replace(command.Trim(), @"\s+", " ");
        }
    }
}
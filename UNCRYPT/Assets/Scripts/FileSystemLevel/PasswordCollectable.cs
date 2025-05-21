using System;
using System.Collections;
using DefaultNamespace;
using DialogueSystem;
using Terminal;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FileSystemLevel
{
    public class PasswordCollectable : Collectable
    {
        private bool _hasInteracted;
        
        private void Awake()
        {
            _hasInteracted = false;
            gameObject.SetActive(SaveSystem.GetData().StoryCheckpoint == SaveSystem.StoryCheckpoint.TutorialCompleted);
        }

        public void Show()
        {
            if(_hasInteracted) return;
            
            _hasInteracted = true;
            StartCoroutine(ShowCoroutine());
        }

        public IEnumerator ShowCoroutine()
        {
            ShowPanel("passwords", "password.txt", "OmqemdQzodkbfuaz",
                "You've found the password, but it's encrypted. Once you figure out the key to decrypt it, you'll be able to reveal its contents.");

            yield return new WaitWhile(() => TerminalManager.Instance.Writer.gameObject.activeSelf);
            InputManager.Instance.InputActions.Disable();

            yield return DialogueManager.Instance.ParseDialogue("found_password_file_1", false, false);
            
            SaveSystem.GameData data = SaveSystem.GetData();
            data.StoryCheckpoint = SaveSystem.StoryCheckpoint.FileTaken;
            SaveSystem.Save(data);
            SceneFader.Instance.SwitchScene(Scenes.Hub);
        }
    }
}
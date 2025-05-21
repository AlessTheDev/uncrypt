using System.Collections;
using DefaultNamespace;
using DialogueSystem;
using Terminal;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FileSystemLevel
{
    public class DecryptionKey : Collectable
    {
        [Header("Key Info")] 
        [SerializeField] private string title;
        [SerializeField] private string content;
        [SerializeField] private bool isFirstKey;

        private bool _hasInteracted;
        
        private void Awake()
        {
            gameObject.SetActive(false);
            _hasInteracted = false;

            switch (SaveSystem.GetData().StoryCheckpoint)
            {
                case SaveSystem.StoryCheckpoint.FileTaken:
                    if (isFirstKey)
                    {
                        gameObject.SetActive(true);
                    }
                    break;
                case SaveSystem.StoryCheckpoint.FirstKeyTaken:
                    if (!isFirstKey)
                    {
                        gameObject.SetActive(true);
                    }
                    break;
            }
            fileNameVisual.text = title;
        }

        public void Show()
        {
            if(_hasInteracted) return;
            
            _hasInteracted = true;
            StartCoroutine(ShowCoroutine());
        }

        private IEnumerator ShowCoroutine()
        {
            ShowPanel("keys", title, content, $"You obtained the key {title}, remember it... You can find it in the 'keys' folder");
            yield return new WaitWhile(() => TerminalManager.Instance.Writer.isActiveAndEnabled);

            yield return DialogueManager.Instance.ParseDialogue((isFirstKey ? "first" : "second") +  "_key_found_1", false, false);

            SaveSystem.GameData data = SaveSystem.GetData();
            data.StoryCheckpoint = isFirstKey
                ? SaveSystem.StoryCheckpoint.FirstKeyTaken
                : SaveSystem.StoryCheckpoint.SecondKeyTaken;
            SaveSystem.Save(data);
            SceneFader.Instance.SwitchScene(Scenes.Hub);
        }
    }
}

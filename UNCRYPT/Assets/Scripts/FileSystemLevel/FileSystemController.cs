using System;
using System.Collections;
using Companion;
using DefaultNamespace;
using DialogueSystem;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FileSystemLevel
{
    public class FileSystemController : MonoBehaviour
    {
        [SerializeField] private CompanionController companion;
        [SerializeField] private Transform arenaEntrance;
        [SerializeField] private Transform arenaMissionSpawnPosition;
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip mainTheme;
        [SerializeField] private AudioClip battleTheme;
        [SerializeField] private AudioClip puzzleTheme;

        private IEnumerator Start()
        {
            SaveSystem.GameData data = SaveSystem.GetData();

            if (data.StoryCheckpoint == SaveSystem.StoryCheckpoint.SecondKeyTaken)
            {
                Transform playerTransform = GameManager.Instance.Player.transform;
                playerTransform.position = new Vector3(arenaMissionSpawnPosition.position.x, playerTransform.position.y,
                    arenaMissionSpawnPosition.position.z);
            }
            
            audioSource.clip = mainTheme;
            StartCoroutine(AudioFadeIn(2));

            yield return new WaitForSeconds(1f);

            switch (data.StoryCheckpoint)
            {
                case SaveSystem.StoryCheckpoint.None:
                    data.StoryCheckpoint = SaveSystem.StoryCheckpoint.TutorialCompleted;
                    SaveSystem.Save(data);

                    yield return DialogueManager.Instance.ParseDialogue("arrived_to_filesystem_1");
                    _ = SceneFader.Instance.SwitchScene(Scenes.Hub);
                    break;
                case SaveSystem.StoryCheckpoint.TutorialCompleted:
                    yield return DialogueManager.Instance.ParseDialogue("need_to_find_password_1");
                    break;
                case SaveSystem.StoryCheckpoint.FileTaken:
                    yield return DialogueManager.Instance.ParseDialogue("need_to_find_first_key_1");
                    break;
                case SaveSystem.StoryCheckpoint.FirstKeyTaken:
                    yield return DialogueManager.Instance.ParseDialogue("need_to_find_second_key_1");
                    break;
                case SaveSystem.StoryCheckpoint.SecondKeyTaken:
                    yield return DialogueManager.Instance.ParseDialogue("need_to_decrypt_1");
                    companion.LeadPlayerTo(arenaEntrance.position);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IEnumerator AudioFadeOut(float duration)
        {
            float startVolume = audioSource.volume;

            while (audioSource.volume > 0) {
                audioSource.volume -= startVolume * Time.deltaTime / duration;

                yield return null;
            }

            audioSource.Stop();
            audioSource.volume = startVolume;
        }
        
        public IEnumerator AudioFadeIn(float duration)
        {
            audioSource.Play();
            
            while (audioSource.volume < 1) {
                audioSource.volume += Time.deltaTime / duration;

                yield return null;
            }
        }

        public IEnumerator SwitchToBattleTheme()
        {
            yield return AudioFadeOut(1f);
            yield return new WaitForSeconds(1);
            audioSource.clip = battleTheme;
            yield return AudioFadeIn(1);
        }
        
        public void StartPuzzleTheme()
        {
            audioSource.clip = puzzleTheme;
            StartCoroutine(AudioFadeIn(1));
        }

    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Terminal;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace HUB.UI
{
    public class HubUI : MonoBehaviour
    {
        [SerializeField] private GameObject mainContainer;
        
        [SerializeField] private HubButton[] hubButtons;

        private bool _isEnabled = false;
        public void Show()
        {
            if (_isEnabled) return;
            _isEnabled = true;
            mainContainer.SetActive(true);
            StartCoroutine(ShowRandomly());
            
            GameManager.Instance.Player.DisableMovement();
        }

        private IEnumerator ShowRandomly()
        {
            foreach (HubButton b in hubButtons)
            {
                yield return new WaitForSeconds(Random.Range(0f, 0.3f));
                b.Show();
            }
        }

        public void Hide()
        {
            if (!_isEnabled) return;

            _isEnabled = false;
            
            foreach (HubButton b in hubButtons)
            {
                b.Hide();
            }
            CameraManager.Instance.FollowPlayer();
            GameManager.Instance.Player.EnableMovement();
        }
        
        public void StartMission()
        {
            _ = SceneFader.Instance.SwitchScene(
                SaveSystem.GetData().StoryCheckpoint switch
                {
                    SaveSystem.StoryCheckpoint.FirstKeyTaken => Scenes.FileSystem,
                    SaveSystem.StoryCheckpoint.SecondKeyTaken => Scenes.FileSystem,
                    _ => Scenes.Firewall
                });
        }

        public void OpenTerminal()
        {
            Hide();
            TerminalManager.Instance.Open("");
        }
    }
}
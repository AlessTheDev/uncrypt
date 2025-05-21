using System;
using TMPro;
using UnityEngine;

namespace HUB
{
    public class HubMissionButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI missionDescription;

        private void Start()
        {
            missionDescription.text = GetMissionDescription();
        }

        private static string GetMissionDescription() =>
            SaveSystem.GetData().StoryCheckpoint switch
            {
                SaveSystem.StoryCheckpoint.None => "M. 00 Tutorial",
                SaveSystem.StoryCheckpoint.TutorialCompleted => "M. 01 Find the password",
                SaveSystem.StoryCheckpoint.FileTaken => "M. 02 Find the key",
                SaveSystem.StoryCheckpoint.FirstKeyTaken => "M. 03 Find the key",
                SaveSystem.StoryCheckpoint.SecondKeyTaken => "M. 04 Decipher the password",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
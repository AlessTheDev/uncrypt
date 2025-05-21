using System;
using System.Collections;
using Companion;
using DialogueSystem;
using Terminal;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HUB
{
    public class HubController : MonoBehaviour
    {
        [SerializeField] private GameObject hubUI;
        [SerializeField] private HubStatue hubStatue;
        [SerializeField] private CompanionController companion;

        private IEnumerator Start()
        {
            InputManager.Instance.InputActions.Player.Enable();
            if (SaveSystem.GetData().StoryCheckpoint == SaveSystem.StoryCheckpoint.None)
            {
                companion.DisableMovement();
                
                yield return new WaitUntil(() => hubStatue.IsExpanded);
                
                InputManager.Instance.InputActions.Disable();
                yield return StartCoroutine(DialogueManager.Instance.ParseDialogue("hub_intro_1"));
                InputManager.Instance.InputActions.Enable();

                yield return new WaitUntil(() => hubUI.activeSelf);
                TopUIDialogue.Instance.ShowDialogue(new TopUIDialogue.DialogueSentence[]
                {
                    new("Very well, I see that you've managed to start the HUB."),
                    new("Now let's go on a mission if you're ready!"),
                }, new WaitForSeconds(2));
            }
        }
    }
}
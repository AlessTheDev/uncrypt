using System.Collections;
using Companion;
using DialogueSystem;
using UI;
using UnityEngine;

namespace Firewall
{
    public class FirewallController : MonoBehaviour
    {
        [SerializeField] private CompanionController companion;
        [SerializeField] private Transform terminalLocation;
        [SerializeField] private bool debugTutorial = false;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(3f);
            
            if (debugTutorial || SaveSystem.GetData().StoryCheckpoint == SaveSystem.StoryCheckpoint.None)
            {
                yield return StartCoroutine(DialogueManager.Instance.ParseDialogue("intro_firewall_1"));
                TopUIDialogue.Instance.ShowDialogue(
                    new TopUIDialogue.DialogueSentence("Follow me, I'll take you to the terminal"), new WaitForSeconds(3));
            
                companion.LeadPlayerTo(
                    terminalLocation.position,
                    () =>
                    {
                        StartCoroutine(OnTerminalArrival());
                    }
                );
            }
            else
            {
                yield return StartCoroutine(DialogueManager.Instance.ParseDialogue("lead_to_terminal_1"));
            }
        }

        private IEnumerator OnTerminalArrival()
        {
            yield return new WaitWhile(() => TopUIDialogue.Instance.IsBusy);
            TopUIDialogue.Instance.ShowDialogue(new[]
            {
                new TopUIDialogue.DialogueSentence("We've arrived!"),
                new TopUIDialogue.DialogueSentence("Now approach it and interact...")
            }, new WaitForSeconds(1));
        }
    }
}
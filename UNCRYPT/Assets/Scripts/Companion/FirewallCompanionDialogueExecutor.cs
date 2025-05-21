using Firewall;
using UI;
using UnityEngine;
using Utils;

namespace Companion
{
    public class FirewallCompanionDialogueExecutor : CompanionDialogueExecutor
    {
        [SerializeField] private Transform terminal;

        public override void ExecuteAction(string action)
        {
            base.ExecuteAction(action);

            switch (action)
            {
                case "LeadToTerminal":
                    TopUIDialogue.Instance.ShowDialogue(
                        new TopUIDialogue.DialogueSentence("Follow me, I'll take you to the terminal"), new WaitForSeconds(3));
                    gameObject.GetComponent<CompanionController>().LeadPlayerTo(
                        terminal.position,
                        () => StartCoroutine(Utilities.WaitAndExecute(ArrivedDialogue, () => !TopUIDialogue.Instance.IsBusy))
                    );
                    break;
                
                case "LeadToPort":
                    TopUIDialogue.Instance.ShowDialogue(
                        new TopUIDialogue.DialogueSentence("Ok, let's go"), new WaitForSeconds(1));
                    gameObject.GetComponent<CompanionController>().LeadPlayerTo(
                        PortsManager.Instance.OpenPort.PinnedPosition,
                        () => StartCoroutine(Utilities.WaitAndExecute(ArrivedDialogue, () => !TopUIDialogue.Instance.IsBusy))
                    );
                    break;
            }
        }

        public void ArrivedDialogue()
        {
            TopUIDialogue.Instance.ShowDialogue(new[]
            {
                new TopUIDialogue.DialogueSentence("We've arrived!"),
            }, new WaitForSeconds(2));
        }
    }
}
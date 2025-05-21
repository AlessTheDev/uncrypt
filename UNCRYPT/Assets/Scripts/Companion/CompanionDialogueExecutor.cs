using System;
using DialogueSystem;
using UnityEngine;

namespace Companion
{
    public class CompanionDialogueExecutor : MonoBehaviour, IDialogueExecutor
    {
        private const string CharacterId = "companion";
        private const string DisplayName = "Assistant";
        
        private void Start()
        {
            DialogueManager.Instance.RegisterDialogueExecutor(CharacterId, this);
        }

        public void Talk(string sentence)
        {
            DialogueUIManager.Instance.Display(DisplayName, sentence);
        }

        public virtual void ExecuteAction(string action)
        {
            if(string.IsNullOrEmpty(action)) return;
            string[] actionParameters = action.Split('|');

            switch (actionParameters[0])
            {
                case "PlayAnimation":
                    Debug.LogError("Animations are not supported for companion yet");
                    break;
                case "JumpTo":
                    DialogueManager.Instance.OverrideDialogue(actionParameters[1]);
                    break;
            }
        }

        public void SetOptions(DialogueOption[] options)
        {
            DialogueUIManager.Instance.LoadOptions(options, this);
        }

        public void OnDialogueEnd(string dialogueId)
        {
            
        }

        public bool CanDialogueProceed() => DialogueUIManager.Instance.CanDialogueProceed();
        public Transform GetTransform() => transform;
    }
}
using System;
using DialogueSystem;
using UnityEngine;

namespace NPCs
{
    public class EmptyDialogueExecutor : MonoBehaviour, IDialogueExecutor
    {
        [SerializeField] private string characterDisplayName;
        [SerializeField] private string characterId;

        private void Start()
        {
            DialogueManager.Instance.RegisterDialogueExecutor(characterId, this);
        }

        public void Talk(string sentence)
        {
            DialogueUIManager.Instance.Display(characterDisplayName, sentence);
        }

        public void ExecuteAction(string action)
        {
            
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
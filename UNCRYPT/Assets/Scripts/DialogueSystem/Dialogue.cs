using System;
using UnityEngine.Serialization;

namespace DialogueSystem
{
    [Serializable]
    public class Dialogue
    {
        public string dialogueId;
        public string characterId;
        public string sentence;
        public string[] actions;
        public string nextDialogue;
        public DialogueOption[] options;
    }

    [Serializable]
    public class DialogueOption
    {
        public string optionName;
        public string[] optionActions;
    }
}
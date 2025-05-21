using UnityEngine;

namespace DialogueSystem
{
    public interface IDialogueExecutor
    {
        /// <summary>
        /// Displays the given sentence
        /// </summary>
        /// <param name="sentence">The sentence to display.</param>
        void Talk(string sentence);

        /// <summary>
        /// Executes a specified action during the dialogue.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        void ExecuteAction(string action);

        /// <summary>
        /// Sets the dialogue options for the current dialogue state.
        /// </summary>
        /// <param name="options">An array of dialogue options.</param>
        void SetOptions(DialogueOption[] options);

        /// <summary>
        /// Called when the dialogue ends, allowing the executor to clean up or reset.
        /// </summary>
        /// <param name="dialogueId"></param>
        void OnDialogueEnd(string dialogueId);

        /// <summary>
        /// Determines if the dialogue can proceed to the next state.
        /// </summary>
        /// <returns>True if the dialogue can proceed, otherwise false.</returns>
        bool CanDialogueProceed();

        /// <summary>
        /// Gets the transform associated with the dialogue executor, typically for camera focusing.
        /// </summary>
        /// <returns>The transform of the executor.</returns>
        Transform GetTransform();
    }
}
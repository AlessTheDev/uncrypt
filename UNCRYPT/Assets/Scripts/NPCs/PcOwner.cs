using System;
using DG.Tweening;
using DialogueSystem;
using UnityEngine;

namespace NPCs
{
    public class PcOwner : BaseNPC
    {
        [SerializeField] private Transform bot;
        [SerializeField] private Transform botTarget;
        [SerializeField] private GameObject explosions;

        protected override void OnStart()
        {
            base.OnStart();
            StartCoroutine(DialogueManager.Instance.ParseDialogue("final_assistant_1"));
        }

        public override void ExecuteAction(string action)
        {
            base.ExecuteAction(action);

            if (string.IsNullOrEmpty(action)) return;

            string[] actionParameters = action.Split('|');

            switch (actionParameters[0])
            {
                case "CallBot":
                    bot.DOMove(botTarget.position, 3f).SetEase(Ease.OutCubic);
                    break;
            }
        }

        public override void OnDialogueEnd(string dialogueId)
        {
            if (dialogueId == "pc_owner_1")
            {
                explosions.SetActive(true);
                GameManager.Instance.Player.DisableMovement();
                InputManager.Instance.InputActions.Disable();
            }
        }
    }
}
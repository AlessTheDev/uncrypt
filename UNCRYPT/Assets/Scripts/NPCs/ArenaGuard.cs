using DG.Tweening;
using DialogueSystem;
using UnityEngine;

namespace NPCs
{
    public class ArenaGuard : BaseNPC
    {
        [SerializeField] private GameObject interactableObj;
        [SerializeField] private GameObject interactCanvas;

        [SerializeField] private Vector3 blockPos;
        [SerializeField] private Vector3 freePos;
        [SerializeField] private BoxCollider blockCollider;

        protected override void OnStart()
        {
            if (SaveSystem.GetData().StoryCheckpoint == SaveSystem.StoryCheckpoint.SecondKeyTaken)
            {
                transform.localPosition = freePos;
                interactableObj.SetActive(false);
                interactCanvas.SetActive(false);
                blockCollider.enabled = false;
            }
            else
            {
                transform.localPosition = blockPos;
            }
        }

        public void BlockPassage()
        {
            transform.DOLocalMove(blockPos, 0.5f);
            blockCollider.enabled = true;
            interactableObj.SetActive(false);
        }
    }
}
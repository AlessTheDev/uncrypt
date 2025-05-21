using HUB.UI;
using Player;
using UnityEngine;

namespace HUB
{
    public class HubStatue : MonoBehaviour
    {
        [SerializeField] private HubSpheres hubSpheres;
        [SerializeField] private Interactable interactable;
        [SerializeField] private HubUI ui;
        public bool IsExpanded { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == PlayerController.Layer)
            {
                hubSpheres.ExpandedState();
                IsExpanded = true;
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == PlayerController.Layer)
            {
                hubSpheres.DefaultState();
                IsExpanded = false;
            }
        }

        public void OnInteract()
        {
            CameraManager.Instance.HubView();
            interactable.InteractUI.Hide();
            ui.Show();
        }
    }
}
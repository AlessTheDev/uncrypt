using System;
using Player;
using UnityEngine;

namespace Firewall
{
    public class PortAreaCollider : MonoBehaviour
    {
        [SerializeField] private GameObject portVirtualCamera;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == PlayerController.Layer)
            {
                portVirtualCamera.SetActive(true);
                GameManager.Instance.OnPlayerEntersSafeZone?.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == PlayerController.Layer)
            {
                portVirtualCamera.SetActive(false);
                GameManager.Instance.OnPlayerExitsSafeZone?.Invoke();
            }
        }
    }
}
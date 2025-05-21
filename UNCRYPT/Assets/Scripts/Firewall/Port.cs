using System;
using System.Collections;
using Player;
using UI;
using Unity.Cinemachine;
using UnityEngine;

namespace Firewall
{
    public class Port : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material openMaterial;
        [SerializeField] private Material closedMaterial;
        [SerializeField] private CinemachineCamera upViewCamera;
        [SerializeField] private int portNumber;
        [SerializeField] private string serviceName;
        [SerializeField] private Transform pinnedTransform;
        
        public bool IsOpen { get; private set; }

        private void Awake()
        {
            meshRenderer.material = closedMaterial;
            IsOpen = false;
        }

        public void Open()
        {
            meshRenderer.material = openMaterial;
            IsOpen = true;
        }
        
        public int PortNumber => portNumber;
        public string PortService => serviceName;

        public IEnumerator Highlight(float seconds)
        {
            upViewCamera.gameObject.SetActive(true);
            yield return new WaitForSeconds(seconds);
            upViewCamera.gameObject.SetActive(false);
        }
        
        public Vector3 PinnedPosition => pinnedTransform.position;
    }
}
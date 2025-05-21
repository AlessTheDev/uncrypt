using System;
using DG.Tweening;
using UnityEngine;

namespace HUB
{
    public class HubSpheres : MonoBehaviour
    {
        [SerializeField] private float height;
        [SerializeField] private float duration;
        [SerializeField] private float scaleMultiplier;
        
        private Vector3 _startPosition;
        private void Start()
        {
            _startPosition = transform.localPosition;
            
            transform.DOLocalMove(_startPosition + Vector3.up * height, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
            DefaultState();
        }

        public void DefaultState()
        {
            transform.DOScale(Vector3.one, 1);
        }
        
        public void ExpandedState()
        {
            transform.DOScale(transform.localScale * scaleMultiplier, 1f).SetEase(Ease.OutSine);
        }

        
    }
}

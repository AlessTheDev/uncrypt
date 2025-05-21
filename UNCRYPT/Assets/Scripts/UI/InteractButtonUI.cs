using System;
using UnityEngine;
using DG.Tweening;

namespace UI
{
    public class InteractButtonUI : MonoBehaviour
    {
        [SerializeField] private float fadeDuration = 2f;
        [SerializeField] private Ease easeType = Ease.InOutQuint;
        [SerializeField] private float yOffset = -2f;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private float _startY = 0;
        private void Start()
        {
            _startY = transform.position.y;
            transform.position = new Vector3(transform.position.x, transform.position.y + yOffset, transform.position.z);
            if (canvasGroup)
            {
                canvasGroup.alpha = 0;
            }
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            transform.DOMoveY(_startY, fadeDuration).SetEase(easeType);

            if (canvasGroup)
            {
                canvasGroup.DOFade(1, fadeDuration).SetEase(easeType);
            }
        }
    
        public void Hide()
        {
            transform.DOMoveY(_startY + yOffset, fadeDuration).SetEase(easeType).OnComplete(() => gameObject.SetActive(false));
            if (canvasGroup)
            {
                canvasGroup.DOFade(0, fadeDuration).SetEase(easeType);
            }
        }
    }
}

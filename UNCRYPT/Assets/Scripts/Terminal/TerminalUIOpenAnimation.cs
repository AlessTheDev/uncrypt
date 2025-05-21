using System;
using DG.Tweening;
using UnityEngine;

namespace Terminal
{
    [RequireComponent(typeof(RectTransform))]
    public class TerminalUIOpenAnimation : MonoBehaviour
    {
        [SerializeField] private float duration = 1f;
        [SerializeField] private Ease ease = Ease.InOutQuad;

        private RectTransform _rect;
        private Tween _activeTween;
        private float _initialScaleX;

        private void Awake()
        {
            _rect = GetComponent<RectTransform>();
            _initialScaleX = _rect.localScale.x;
        }

        private void OnEnable()
        {
            _activeTween?.Kill();

            _rect.localScale = new Vector2(.1f, _initialScaleX);
            _activeTween = _rect.DOScaleX(_initialScaleX, duration).SetEase(ease);
        }

        public void Close()
        {
            _activeTween?.Kill();
            _activeTween = _rect.DOScaleX(0, duration).SetEase(ease).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
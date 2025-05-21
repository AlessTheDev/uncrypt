using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

namespace DialogueSystem
{
    public class OptionUI : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI text;

        [Header("Animation Settings")] [SerializeField]
        private float fadeDuration;

        [SerializeField] private Ease easeIn = Ease.InOutExpo;
        [SerializeField] private Ease easeOut = Ease.OutCubic;

        private UnityAction _callback;
        private RectTransform _rectTransform;
        
        private float _originalWidth;
        
        private Tween _tween;
        
        private void Awake()
        {
            button.onClick.AddListener(() => _callback?.Invoke());
            _rectTransform = GetComponent<RectTransform>();
            _rectTransform.pivot = new Vector2(1f, 0.5f);
            _originalWidth = _rectTransform.sizeDelta.x;
        }

        public void Enable(string optionName, UnityAction onClick, float animationDelay = 0)
        {
            _tween?.Kill();
            
            gameObject.SetActive(true);
            
            text.text = optionName;
            _callback = onClick;

            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);

            DOTween.To(() => _rectTransform.sizeDelta.x,
                    value => _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value),
                    _originalWidth,
                    fadeDuration - animationDelay)
                .SetDelay(animationDelay)
                .SetEase(easeIn);
        }

        public void Disable()
        {
            _tween = DOTween.To(() => _rectTransform.sizeDelta.x,
                    value => _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value),
                    0,
                    fadeDuration)
                .SetEase(easeOut)
                .OnComplete(() => gameObject.SetActive(false));
        }

    }
}
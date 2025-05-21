using System;
using Companion;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class CompanionNavigator : MonoBehaviour
    {
        [SerializeField] private RectTransform companionNavigator;
        [SerializeField] private CompanionController companion;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform stableImage;
        [SerializeField] private float radius;
        [SerializeField] private float scaleAdditionMultiplier = 0.5f;
        [SerializeField] private float maxScale = 400;

        private Canvas _canvas;
        private bool _isEnabled;

        private Transform _playerTransform;
        private float _indicatorBaseScale;
        private float _stableImageBaseScale;
        
        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;
            _isEnabled = false;
            _playerTransform = GameManager.Instance.PlayerTransform;
            _indicatorBaseScale = companionNavigator.sizeDelta.x;
            _stableImageBaseScale = stableImage.sizeDelta.x;
        }

        public void ShowIndicator()
        {
            if(_isEnabled) return;
            
            _isEnabled = true;
            canvasGroup.DOFade(0.7f, 1.5f).SetEase(Ease.InQuad).OnStart(() =>
            {
                _canvas.enabled = true;
            });
        }

        
        public void HideIndicator()
        {
            if(!_isEnabled) return;
            
            _isEnabled = false;
            canvasGroup.DOFade(0, 1.5f).SetEase(Ease.InQuad).OnComplete(() => _canvas.enabled = false);
        }

        private void Update()
        {
            if(!_isEnabled) return;
            
            Vector3 directionToPlayer = companion.transform.position - _playerTransform.position;
            directionToPlayer.Normalize();

            Vector2 uiDirection = new Vector2(directionToPlayer.x, directionToPlayer.z);
            
            companionNavigator.anchoredPosition = uiDirection * radius;
            companionNavigator.eulerAngles = new Vector3(0, 0, Vector3.SignedAngle(Vector3.right, uiDirection, Vector3.forward));

            float scaleAddition = Vector3.Distance(_playerTransform.position, companion.transform.position);
            
            float navResizedScale = _indicatorBaseScale + scaleAdditionMultiplier * scaleAddition;
            companionNavigator.sizeDelta = Vector2.one * Mathf.Min(navResizedScale, maxScale);
            
            float imageResizedScale = _stableImageBaseScale + scaleAdditionMultiplier * scaleAddition;
            stableImage.sizeDelta = Vector2.one * Mathf.Min(imageResizedScale, maxScale);
            
            stableImage.rotation = Quaternion.identity;
        }
    }
}

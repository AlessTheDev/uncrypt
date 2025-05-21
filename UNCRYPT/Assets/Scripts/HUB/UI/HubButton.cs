using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Serialization;

namespace HUB.UI
{
    public class HubButton : MonoBehaviour
    {
        [SerializeField] private RectTransform circle;
        [SerializeField] private RectTransform buttonRect;
        [SerializeField] private Button button;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private Vector2 linePosOffset;

        [Header("Animation Settings")] 
        [SerializeField] private float circleFadeInDuration = 1f;
        [SerializeField] private float buttonFadeInDuration = 2f;
        
        [SerializeField] private float circleFadeOutDuration = .3f;
        [SerializeField] private float buttonFadeOutDuration = .2f;
        
        [SerializeField] private float lineRendererSpeed = 5;

        private bool _showLine;
        private Vector2 _originalCircleScale;
        private float _originalButtonWidth;

        private void Awake()
        {
            lineRenderer.positionCount = 2;

            // Set the pivot based on the button's local position
            buttonRect.pivot = new Vector2(Mathf.Sign(buttonRect.localPosition.x) == 1 ? 0 : 1, 0.5f);

            _originalCircleScale = circle.sizeDelta;
            _originalButtonWidth = buttonRect.sizeDelta.x;
            
            ResetAnimationStates();
        }

        private void Update()
        {
            UpdateLinePositions();
        }

        private void UpdateLinePositions()
        {
            // Get the circle's center in world space
            Vector3 circleCenter = circle.TransformPoint(Vector3.zero);
            lineRenderer.SetPosition(0, circleCenter);

            lineRenderer.SetPosition(1,
                Vector3.Lerp(lineRenderer.GetPosition(1),
                    _showLine
                        ? GetButtonEdgePosition() + new Vector3(linePosOffset.x * Mathf.Sign(buttonRect.position.x),
                            linePosOffset.y, 0)
                        : circleCenter,
                    lineRendererSpeed * Time.deltaTime));
        }

        private Vector3 GetButtonEdgePosition()
        {
            // Get the rect transform's corners
            Vector3[] corners = new Vector3[4];
            buttonRect.GetWorldCorners(corners);

            // If pivot is on the left (0), use the left edge
            // If pivot is on the right (1), use the right edge
            int edgeIndex = buttonRect.pivot.x < 0.5f ? 0 : 2; // 0 for left edge, 2 for right edge

            // Return the middle point of the selected edge
            return Vector3.Lerp(corners[edgeIndex], corners[edgeIndex + 1], 0.5f);
        }
        
        private void ResetAnimationStates()
        {
            circle.localScale = Vector2.zero;
            buttonRect.sizeDelta = new Vector2(0, buttonRect.sizeDelta.y);
        }

        public void Show()
        {
            button.interactable = true;

            lineRenderer.enabled = true;
            // Animate the circle scaling and button width expansion
            circle.transform.DOScale(_originalCircleScale, circleFadeInDuration).SetDelay(1f)
                .OnComplete(() =>
                {
                    _showLine = true;
                    DOTween.To(() => buttonRect.sizeDelta.x,
                        value => buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value),
                        _originalButtonWidth,
                        buttonFadeInDuration).SetEase(Ease.OutCubic).SetDelay(1f);
                });
            
        }

        public void Hide()
        {
            button.interactable = false;
            DOTween.To(() => buttonRect.sizeDelta.x,
                value => buttonRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value),
                0,
                buttonFadeOutDuration).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                _showLine = false;
                circle.transform.DOScale(0, circleFadeOutDuration).SetDelay(0.5f).OnComplete(() => lineRenderer.enabled = false);
            });
        }
    }
}
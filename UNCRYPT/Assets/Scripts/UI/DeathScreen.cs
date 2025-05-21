using DefaultNamespace;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Canvas))]
    public class DeathScreen : PersistentSingleton<DeathScreen>
    {
        [SerializeField] private CanvasGroup container;
        [SerializeField] private Image panel;
        [SerializeField] private float fadeDuration = 1f;
        [SerializeField] private Ease fadeEase = Ease.OutSine;
        
        private Canvas _canvas;

        private float _initialPanelAlpha;
        private Vector3 _initialPanelSize;

        protected override void OnAwake()
        {
            _canvas = GetComponent<Canvas>();
            
            _canvas.enabled = false;
            _initialPanelAlpha = panel.color.a;
            _initialPanelSize = panel.rectTransform.localScale;
        }

        public void Show()
        {
            container.alpha = 0;

            Color pColor = panel.color;
            pColor.a = 0;
            panel.color = pColor;

            container.alpha = 0;
            panel.rectTransform.localScale *= new Vector2(0, 1);
            
            _canvas.enabled = true;
            
            panel.DOFade(_initialPanelAlpha, fadeDuration).SetEase(fadeEase);
            container.DOFade(1, fadeDuration).SetEase(fadeEase);
            
            panel.rectTransform.DOScale(_initialPanelSize, fadeDuration).SetEase(fadeEase);
        }

        public void BackToHub()
        {
            _ = SceneFader.Instance.SwitchScene(Scenes.Hub);
            _canvas.enabled = false;
        }
    }
}

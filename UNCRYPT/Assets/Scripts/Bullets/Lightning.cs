using System;
using System.Collections;
using DG.Tweening;
using Player;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.VFX;
using Utils;

namespace Bullets
{
    public class Lightning : MonoBehaviour
    {
        [SerializeField] private float damage = 40f;
        
        [Header("Collider")]
        [SerializeField] private Collider col;
        [SerializeField] private float colDelay = 0.4f;
        
        [Header("Lightning")]
        [SerializeField] private GameObject lightning;
        [SerializeField] private float lightningDelay;
        [SerializeField] private float lightningDuration;

        [Header("Base")]
        [SerializeField] private float baseSpawnDuration;
        [SerializeField] private float xSpawnDelay;
        [SerializeField] private Ease easeType;
        [SerializeField] private float fadeOutDuration;
        
        [SerializeField] private Transform baseCircleTransform;
        [SerializeField] private SpriteRenderer baseCircleRenderer;
        
        [SerializeField] private Transform baseXTransform;
        [SerializeField] private SpriteRenderer baseXRenderer;

        private Vector3 _initialBaseCircleScale;
        private Vector3 _initialBaseXScale;

        private ObjectPool<Lightning> _pool;

        private void Awake()
        {
            _initialBaseCircleScale = baseCircleTransform.localScale;
            _initialBaseXScale = baseXTransform.localScale;
        }

        private void OnEnable()
        {
            lightning.SetActive(false);
            col.enabled = false;

            // Initialize base 
            Vector3 baseRot = new Vector3(90, 0, 0);
            baseCircleTransform.eulerAngles = baseRot;
            baseXTransform.eulerAngles = baseRot;

            Utilities.SetAlphaToZero(baseCircleRenderer);
            Utilities.SetAlphaToZero(baseXRenderer);

            baseCircleTransform.localScale = Vector3.zero;
            baseXTransform.localScale = Vector3.zero;

            // Scale animations
            baseCircleTransform.DOScale(_initialBaseCircleScale, baseSpawnDuration).SetEase(easeType);
            baseXTransform.DOScale(_initialBaseXScale, baseSpawnDuration - xSpawnDelay).SetDelay(xSpawnDelay).SetEase(easeType);

            Vector3 targetRot = new Vector3(90, 0, 720); // 2 full spins
            baseCircleTransform.DORotate(targetRot, baseSpawnDuration, RotateMode.FastBeyond360).SetEase(easeType);
            baseXTransform.DORotate(targetRot, baseSpawnDuration - xSpawnDelay, RotateMode.FastBeyond360)
                .SetDelay(xSpawnDelay).SetEase(easeType);

            // Fade in
            baseCircleRenderer.DOFade(1, baseSpawnDuration).SetEase(easeType);
            baseXRenderer.DOFade(1, baseSpawnDuration - xSpawnDelay).SetDelay(xSpawnDelay).SetEase(easeType);

            // Start lightning effect sequence
            StartCoroutine(ShowLightning());
        }

        private IEnumerator ShowLightning()
        {
            yield return new WaitForSeconds(lightningDelay);
            lightning.SetActive(true);
            
            yield return new WaitForSeconds(colDelay);
            col.enabled = true;
            CameraManager.Instance.SmallShake(0.5f);
            
            yield return new WaitForSeconds(lightningDuration);
            lightning.SetActive(false);
            
            col.enabled = false;
            
            baseCircleRenderer.DOFade(0, fadeOutDuration).SetEase(easeType);
            baseXRenderer.DOFade(0, fadeOutDuration).SetEase(easeType);
            
            yield return new WaitForSeconds(fadeOutDuration);
            
            _pool.Release(this);
        }

        public void SetPool(ObjectPool<Lightning> pool)
        {
            _pool = pool;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == PlayerController.Layer)
            {
                GameManager.Instance.Player.Damage(damage);
            }
        }
    }
}

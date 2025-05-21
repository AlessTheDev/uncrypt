using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Bullets
{
    public class MaceController : MonoBehaviour
    {
        [SerializeField] private int macesCount;
        [SerializeField] private float minRadius;
        [SerializeField] private float maxRadius;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private float radiusChangeSpeed;
        [SerializeField] private float dissolveDuration = 4f;
        [SerializeField] private Transform macePrefab;

        private Transform[] _maces;

        private float _radius;

        private float _angleT;
        private float _radiusT;

        private void Start()
        {
            _maces = new Transform[macesCount];

            for (int i = 0; i < macesCount; i++)
            {
                _maces[i] = Instantiate(macePrefab, transform);
                _maces[i].DORotate(new Vector3(360 * Utilities.GetRandomDir(), 360 * Utilities.GetRandomDir()), 2f,
                    RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutCubic);
            }

            _angleT = 0;
        }

        private void Update()
        {
            _angleT += Time.deltaTime * rotationSpeed;
            _radiusT += Time.deltaTime * radiusChangeSpeed;

            float angleStep = Mathf.PI * 2 / macesCount;

            for (int i = 0; i < macesCount; i++)
            {
                float phase = i * angleStep + _angleT;
                _maces[i].localPosition = new Vector3(Mathf.Cos(phase), 0, Mathf.Sin(phase)) * _radius;
            }

            _radius = minRadius + (Mathf.Sin(_radiusT) * 0.5f + 0.5f) * (maxRadius - minRadius);
        }

        public void Dissolve()
        {
            transform.SetParent(null);
            foreach (var mace in _maces)
            {
                mace.DOKill();
                MeshRenderer[] meshRenderers = mace.GetComponentsInChildren<MeshRenderer>();
                foreach (var meshRenderer in meshRenderers)
                {
                    Material[] materials = new Material[1];
                    Array.Copy(meshRenderer.materials, materials, 1);
                    meshRenderer.materials = materials;
                }

                mace.DOScale(0, dissolveDuration);
            }

            StartCoroutine(DestroyAfterSeconds(dissolveDuration + 0.1f));
        }

        private IEnumerator DestroyAfterSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            foreach (var mace in _maces)
            {
                mace.DOKill();
            }
            Destroy(gameObject);
        }
    }
}
using System;
using DG.Tweening;
using UnityEngine;

namespace HUB
{
    public class HubSphere : MonoBehaviour
    {
        [SerializeField] private Vector2 rotationDirection;
        [SerializeField] private float rotationDuration;
        [SerializeField] private LoopType loopType = LoopType.Restart;
        [SerializeField] private Ease easeType = Ease.Linear;

        private void Start()
        {
            transform.DORotate(rotationDirection, rotationDuration, RotateMode.FastBeyond360).SetLoops(-1, loopType).SetEase(easeType);
        }
    }
}

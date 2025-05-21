using UnityEngine;
using DG.Tweening;
using Utils;

namespace HUB
{
    public class PedestalCube : MonoBehaviour
    {
        private void Start()
        {
            transform.DORotate(new Vector2(360 * Utilities.GetRandomDir(), 360 * Utilities.GetRandomDir()), Random.Range(0, 15), RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
        }
    }
}

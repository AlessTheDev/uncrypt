using UnityEngine;
using DG.Tweening;

public class SecCamAnimation : MonoBehaviour
{
    [SerializeField] GameObject camBody;
    [SerializeField] float rotation;
    [SerializeField] float rotationDuration;
    


    void Start()
    {
        rotationDuration = Random.Range(2f,5f);
        camBody.transform.DORotate(new Vector3(0,0,rotation), rotationDuration, RotateMode.LocalAxisAdd).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }   
}

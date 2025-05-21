using System;
using UnityEngine;

public class LocalSinMovement : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private float speed;
    
    private Vector3 _startPosition;
    private float _t;

    private void Start()
    {
        _startPosition = transform.localPosition;
    }

    private void Update()
    {
        transform.localPosition = _startPosition + new Vector3(0, Mathf.Sin(_t) * range, 0);
        _t += Time.deltaTime * speed;
    }
}
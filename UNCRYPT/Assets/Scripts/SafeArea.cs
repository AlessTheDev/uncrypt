using System;
using System.Collections;
using Player;
using UnityEngine;
using Utils;

public class SafeArea : MonoBehaviour
{
    private bool _isInside;

    private void Start()
    {
        _isInside = false;
        
        // Avoid to accidentally go out of safe zone if the player exits terminal, it should be handled better with layers in the future
        GameManager.Instance.OnPlayerExitsSafeZone.AddListener(() =>
        {
            if (_isInside)
            {
                StartCoroutine(Utilities.InvokeLater(() =>
                {
                    GameManager.Instance.OnPlayerEntersSafeZone?.Invoke();
                }));
            }
        });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == PlayerController.Layer)
        {
            _isInside = true;
            GameManager.Instance.OnPlayerEntersSafeZone?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == PlayerController.Layer)
        {
            _isInside = false;
            GameManager.Instance.OnPlayerExitsSafeZone?.Invoke();
        }
    }
}
using System;
using Player;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    [SerializeField] private InteractButtonUI interactUI;
    public UnityEvent onInteract;
    private bool _isInteractable;

    private void Awake()
    {
        _isInteractable = false;
    }

    private void Start()
    {
        InputManager.Instance.InputActions.Player.Interact.performed += OnInteractionPerformed;
    }

    private void OnDestroy()
    {
        InputManager.Instance.InputActions.Player.Interact.performed -= OnInteractionPerformed;
    }

    private void OnInteractionPerformed(InputAction.CallbackContext obj)
    {
        if (_isInteractable)
        {
            onInteract?.Invoke();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == PlayerController.Layer)
        {
            interactUI.Show();
            _isInteractable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == PlayerController.Layer)
        {
            interactUI.Hide();
            _isInteractable = false;
        }
    }

    public InteractButtonUI InteractUI => interactUI;
}
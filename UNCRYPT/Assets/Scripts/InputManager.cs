using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : PersistentSingleton<InputManager>
{
    public GeneralInputActions InputActions { get; private set; }

    protected override void OnAwake()
    {
        InputActions = new GeneralInputActions();
        InputActions.Player.Enable();
    }
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;

[CreateAssetMenu(menuName = "SO/InputReader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    public event Action OnMouseUp;

    private Controls _controls;
    public Controls GetControl => _controls;

    private void OnEnable() {
        if(_controls == null) {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }

        _controls.Enable();
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Canceled)
            OnMouseUp?.Invoke();
    }
}

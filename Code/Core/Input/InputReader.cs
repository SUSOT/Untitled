using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayerInput", menuName = "SO/PlayerInput", order = 0)]
public class InputReader : ScriptableObject, PlayerInput.IPlayerActions
{
    public Vector2 InputDirection { get; private set; }
    public event Action OnJumpKeyPressed;
    public event Action OnDashKeyPressed;
    public event Action OnErrorSkillKeyPressed;
    public event Action OnAttackKeyPressed;
    public event Action OnCounterKeyPressed;
    public event Action OnEnterWindowKeyPressed;
    public event Action<bool> OnSkillKeyPressed;

    private PlayerInput _playerInput;

    private void OnEnable()
    {
        if (_playerInput == null)
        {
            _playerInput = new PlayerInput();
            _playerInput.Player.SetCallbacks(this);
        }
        _playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Disable();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnJumpKeyPressed?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        InputDirection = context.ReadValue<Vector2>();
    }

    public void ClearSubscription()
    {
        OnJumpKeyPressed = null;
        OnAttackKeyPressed = null;
        OnDashKeyPressed = null;
        OnCounterKeyPressed = null;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnAttackKeyPressed?.Invoke();
    }

    public void OnCounter(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnCounterKeyPressed?.Invoke();
    }

    public void OnErrorWall(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnErrorSkillKeyPressed?.Invoke();
    }

    public void OnWindow(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnEnterWindowKeyPressed?.Invoke();
    }
}

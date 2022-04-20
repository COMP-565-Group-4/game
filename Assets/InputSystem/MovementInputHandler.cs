using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystem {
public class MovementInputHandler : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    public void MoveEventHandler(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void LookEventHandler(InputAction.CallbackContext context)
    {
        if (cursorInputForLook)
        {
            look = context.ReadValue<Vector2>();
        }
    }

    public void JumpEventHandler(InputAction.CallbackContext context)
    {
        jump = context.ReadValueAsButton();
    }

    public void SprintEventHandler(InputAction.CallbackContext context)
    {
        sprint = context.ReadValueAsButton();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
}

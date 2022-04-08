using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets {
public class StarterAssetsInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;

    // custom input values
    public bool interact;
    public bool grab;
    public bool isPaused;

    [Header("Movement Settings")]
    public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook) {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    // custom inputs below

    public void OnInteract(InputValue value)
    {
        InteractInput(value.isPressed);
    }

    public void OnGrab(InputValue value)
    {
        GrabInput(value.isPressed);
    }

    public void OnPause(InputValue value)
    {
        PauseInput(value.isPressed);
    }
#else
    // old input sys if we do decide to have it (most likely wont)...
#endif

    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    // custom inputs below

    public void InteractInput(bool newInteractState)
    {
        interact = newInteractState;
    }

    public void GrabInput(bool newGrabState)
    {
        grab = newGrabState;
    }

    public void PauseInput(bool newPauseState)
    {
        // Toggle the pause state if the input was activated.
        if (newPauseState)
            isPaused = !isPaused;
    }

#if !UNITY_IOS || !UNITY_ANDROID

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }

#endif
}

}

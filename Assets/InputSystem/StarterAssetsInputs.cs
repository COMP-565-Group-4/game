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

    [Header("Movement Settings")]
    public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
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

#else
    // old input sys if we do decide to have it (most likely wont)...
#endif

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

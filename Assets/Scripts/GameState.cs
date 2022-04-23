using UnityEngine;
using UI;

using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameState : MonoBehaviour
{
    public static bool GamePaused = false;

    private static HUDManager _hudManager;

    public UnityEvent pauseEvent;
    public UnityEvent resumeEvent;

    public void PauseEventHandler(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        if (!GamePaused) {
            Pause();
        } else {
            Resume();
        }
    }

    private void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0f;
        pauseEvent.Invoke();
    }

    private void Resume()
    {
        GamePaused = false;
        Time.timeScale = 1.0f;
        resumeEvent.Invoke();
    }
}

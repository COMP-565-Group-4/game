using UnityEngine;

using UnityEngine.Events;
using UnityEngine.InputSystem;

using Utils;

public class GameState : Singleton<GameState>
{
    public bool GamePaused = false;

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

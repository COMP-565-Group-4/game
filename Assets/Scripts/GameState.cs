using UnityEngine;

using UnityEngine.Events;
using UnityEngine.InputSystem;

using Utils;

public class GameState : Singleton<GameState>
{
    public bool GamePaused = false;

    public UnityEvent PauseEvent;
    public UnityEvent ResumeEvent;

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

    public void QuitEventHandler()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0f;
        PauseEvent.Invoke();
    }

    private void Resume()
    {
        GamePaused = false;
        Time.timeScale = 1.0f;
        ResumeEvent.Invoke();
    }
}

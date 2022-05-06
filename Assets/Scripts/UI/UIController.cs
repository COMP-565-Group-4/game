using System;

using ScriptableObjects;

using UnityEngine;

namespace UI {
public class UIController : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField]
    private GameObject hud;

    [SerializeField]
    private GameObject pauseMenu;

    [SerializeField]
    private GameObject startMenu;

    [SerializeField]
    private GameObject roundEndMenu;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void PauseEventHandler()
    {
        if (!RoundManager.Instance.Started)
            return; // Ignore if the round hasn't started.

        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeEventHandler()
    {
        if (!RoundManager.Instance.Started)
            return; // Ignore if the round hasn't started.

        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RoundEndEventHandler(Round endedRound, uint roundNumber, RoundEndReason reason)
    {
        if (reason is RoundEndReason.Restarted)
            return;

        // TODO: set round's info in the UI.
        // TODO: hide continue button if round wasn't won.

        pauseMenu.SetActive(false);
        roundEndMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void RoundStartEventHandler(Round round, uint number, uint total)
    {
        pauseMenu.SetActive(false);
        startMenu.SetActive(false);
        roundEndMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartRoundEventHandler()
    {
        RoundManager.Instance.EndRound(RoundEndReason.Restarted);
        RoundManager.Instance.StartRound();
    }

    public void QuitRoundEventHandler()
    {
        if (RoundManager.Instance.Started) {
            RoundManager.Instance.EndRound(RoundEndReason.Quit);
        } else {
            // If the round already ended, then go to the start menu.
            pauseMenu.SetActive(false);
            roundEndMenu.SetActive(false);
            startMenu.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    public void PauseMenuCloseEventHandler()
    {
        GameState.Instance.Resume();
    }
}
}

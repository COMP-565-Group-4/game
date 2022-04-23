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

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void PauseEventHandler()
    {
        if (startMenu.activeInHierarchy)
            return; // Ignore if start menu is being shown.

        pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeEventHandler()
    {
        if (startMenu.activeInHierarchy)
            return; // Ignore if start menu is being shown.

        pauseMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RoundStartEventHandler(Round round, uint number, uint total)
    {
        startMenu.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartRoundEventHandler()
    {
        RoundManager.Instance.EndRound(true);
        RoundManager.Instance.StartRound();
        GameState.Instance.Resume();
    }

    public void QuitRoundEventHandler()
    {
        RoundManager.Instance.EndRound(true);
        GameState.Instance.Resume();

        startMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
    }
}
}

using System;

using ScriptableObjects;

using TMPro;

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

    [Header("Round End Menu Components")]
    [SerializeField]
    [Tooltip("TMP component for the title of the round end menu")]
    private TextMeshProUGUI roundEndTitle;

    [SerializeField]
    [Tooltip("TMP component for the round info shown by the round end menu")]
    private TextMeshProUGUI roundEndInfo;

    [SerializeField]
    [Tooltip("TMP component for the continue button in the round end menu")]
    private GameObject roundEndContinueButton;

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
        switch (reason) {
            case RoundEndReason.RoundWon:
                roundEndTitle.text =
                    $"Round {roundNumber}/{RoundManager.Instance.Rounds.Length} Complete!";
                roundEndContinueButton.SetActive(true);
                break;
            case RoundEndReason.GameWon:
                roundEndTitle.text = "All Rounds Complete!";
                roundEndContinueButton.SetActive(false);
                break;
            case RoundEndReason.TimedOut:
                roundEndTitle.text = $"Round {roundNumber} Timed Out";
                roundEndContinueButton.SetActive(false);
                // TODO: Set restart button to purple when continue button is inactive.
                break;
            case RoundEndReason.Restarted:
                return;
            case RoundEndReason.Quit:
                roundEndTitle.text = $"Round {roundNumber} Quit";
                roundEndContinueButton.SetActive(false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(reason), reason, null);
        }

        var minutes = Math.DivRem((long) RoundManager.Instance.Time, 60, out long seconds);
        roundEndInfo.text = $"{minutes:00}:{seconds:00}\n"
            + $"{RoundManager.Instance.OrdersCompleted}/{endedRound.OrderCount}\n"
            + $"{RoundManager.Instance.Money:F0}";

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

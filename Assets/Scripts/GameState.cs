using UnityEngine;
using UI;

using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameState : MonoBehaviour
{
    public int CurrentRound = 1;
    public static uint TotalRounds = 7;
    public static int CurrentMoney = 0;
    public static int LastRoundMoney = 0;
    public static bool GamePaused = false;

    private static HUDManager _hudManager;

    public UnityEvent pauseEvent;
    public UnityEvent resumeEvent;

    void Start()
    {
        _hudManager = GameObject.Find("HUD").GetComponent<HUDManager>();
        StartRound();

        _hudManager.Round = (uint) CurrentRound;
        _hudManager.TotalRounds = TotalRounds;
    }

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

    public static void AddMoney(int value)
    {
        // todo: "TotalMoney" variable, add value to it if value is positive?
        CurrentMoney = CurrentMoney + value;
        _hudManager.Money = (uint) CurrentMoney;
    }

    void StartRound()
    {
        print("Round " + CurrentRound + " started!");
        // initialize some variables or whatever
        // start round
        transform.SendMessage("TimerStart");
    }

    void EndRound()
    {
        print("Round " + CurrentRound + " finished!");
        // tally results
        LastRoundMoney = CurrentMoney;
        if (CurrentRound >= 7) {
            // idk, go to a win screen or something
        }
    }
}

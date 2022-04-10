using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static int CurrentRound = 1;
    public static int CurrentMoney = 0;
    public static int LastRoundMoney = 0;
    public static bool GamePaused = false;

    private static GameObject _hud;
    private static GameObject _pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        _hud = GameObject.Find("HUD");
        _pauseMenu = GameObject.Find("PauseMenu");

        _pauseMenu.SetActive(false);
        StartRound();
    }

    // Update is called once per frame
    void Update() { }

    public static void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0f;
        // print("Time set to " + Time.timeScale);
        // hide HUD
        _hud.SetActive(false);
        // activate pause UI
        _pauseMenu.SetActive(true);
    }

    public static void Resume()
    {
        GamePaused = false;
        Time.timeScale = 1.0f;
        // print("Time set to " + Time.timeScale);
        // deactivate pause UI
        _pauseMenu.SetActive(false);
        // show HUD
        _hud.SetActive(true);
    }

    public static void AddMoney(int value)
    {
        CurrentMoney = CurrentMoney + value;
    }

    void StartRound()
    {
        print("Round " + CurrentRound + " started!");
        // initialize some variables
        RoundTimer.SetRound(CurrentRound);
        // start round
        RoundTimer.TimerStart();
    }

    void EndRound()
    {
        print("Round " + CurrentRound + " finished!");
        // tally results
        LastRoundMoney = CurrentMoney;
        // increment round number
        if (CurrentRound < 7) {
            CurrentRound++;
        } else {
            // idk, go to a win screen or something
        }
    }
}

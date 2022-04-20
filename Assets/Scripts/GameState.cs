using UnityEngine;
using UI;

public class GameState : MonoBehaviour
{
    public int CurrentRound = 1;
    public static uint TotalRounds = 7;
    public static int CurrentMoney = 0;
    public static int LastRoundMoney = 0;
    public static bool GamePaused = false;

    private static GameObject _hud;
    private static GameObject _pauseMenu;
    private static HUDManager _hudManager;

    // Start is called before the first frame update
    void Start()
    {
        _hud = GameObject.Find("HUD");
        _pauseMenu = GameObject.Find("PauseMenu");

        _hudManager = _hud.GetComponent<HUDManager>();
        _pauseMenu.SetActive(false);
        StartRound();

        _hudManager.Round = (uint) CurrentRound;
        _hudManager.TotalRounds = TotalRounds;
    }

    // Update is called once per frame
    void Update() { }

    public static void Pause()
    {
        GamePaused = true;
        Time.timeScale = 0f;
        // hide HUD
        _hud.SetActive(false);
        // activate pause UI
        _pauseMenu.SetActive(true);
    }

    public static void Resume()
    {
        GamePaused = false;
        Time.timeScale = 1.0f;
        // deactivate pause UI
        _pauseMenu.SetActive(false);
        // show HUD
        _hud.SetActive(true);
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class RoundTimer : MonoBehaviour
{
    [Tooltip("How many seconds this round will last")]
    public float RoundLength;

    private float _time;
    public bool _paused;

    private HUDManager _hudManager;

    // Start is called before the first frame update
    void Start()
    {
        _time = RoundLength;
        _paused = true;
        _hudManager = GameObject.Find("HUD").GetComponent<HUDManager>();
        TimerStart();
    }

    // Update is called once per frame
    void Update()
    {
        Tick();
    }

    public void TimerStart()
    {
        // start the timer
        _paused = false;
    }

    void Tick()
    {
        if (!_paused) {
            if (_time > 0) {
                _time = _time - Time.deltaTime;

                // update hud
                _hudManager.Minutes = (uint) Math.Floor(_time) / 60;
                _hudManager.Seconds = (uint) Math.Floor(_time) % 60;
            } else {
                TimerStop();
            }
        }
    }

    void TimerStop()
    {
        // stop the timer
        _paused = true;
        // update the hud clock
        _hudManager.Minutes = 0;
        _hudManager.Seconds = 0;
        // end the round
        SendMessage("EndRound");
    }
}

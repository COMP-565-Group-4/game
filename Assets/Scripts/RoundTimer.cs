using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundTimer : MonoBehaviour
{
    public static float[] RoundLength = new float[] { 0, 120, 180, 180, 180, 180, 180, 180 };
    private static float _time;
    private static bool _paused;

    // Start is called before the first frame update
    void Start()
    {
        _paused = true;
    }

    // Update is called once per frame
    void Update()
    {
        Tick();
    }

    public static void SetRound(int value)
    {
        _time = RoundLength[value];
    }

    public static void TimerStart()
    {
        // start the timer
        _paused = false;
    }

    void Tick()
    {
        if (!_paused) {
            if (_time > 0) {
                _time = _time - Time.deltaTime;
            } else {
                TimerStop();
            }
        }
    }

    void TimerStop()
    {
        // stop the timer
        _paused = true;
        // end the round
        SendMessage("EndRound");
    }
}

using UnityEngine;

public class Countdown : MonoBehaviour
{
    [Tooltip("The amount of time before the countdown ends.")]
    public float Length = 0;

    public bool SelfDestruct = false;

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        Tick();
    }

    void Tick()
    {
        if (Length > 0) {
            Length = Length - Time.deltaTime;
        } else {
            TimeUp();
        }
    }

    void TimeUp()
    {
        if (SelfDestruct) {
            Destroy(gameObject);
        } else {
            SendMessage("Timeout");
        }
    }
}

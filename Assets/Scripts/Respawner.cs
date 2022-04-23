using UnityEngine;

public class Respawner : MonoBehaviour
{
    [Tooltip("Add the prefabs of items you want to respawn here.")]
    public GameObject[] Things;
    [Tooltip("How many seconds should elapse between attempts to respawn the objects.")]
    public float Timer;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = Timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0) {
            timer = timer - Time.deltaTime;
        } else {
            Respawn();
            timer = Timer;
        }
    }

    void Respawn()
    {
        foreach (GameObject thing in Things) {
            if (GameObject.Find(thing.name) == null) {
                GameObject newThing = Instantiate(thing);
                newThing.name = thing.name;
            }
        }
    }
}

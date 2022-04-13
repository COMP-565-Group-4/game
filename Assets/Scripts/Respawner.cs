using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawner : MonoBehaviour
{
    [Tooltip("Add the prefabs of items you want to respawn here.")]
    public GameObject[] Things;
    [Tooltip("How many seconds should elapse between attempts to respawn the objects.")]
    public float Timer;

    private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        _timer = Timer;
    }

    // Update is called once per frame
    void Update()
    {
        if (_timer > 0) {
            _timer = _timer - Time.deltaTime;
        } else {
            Respawn();
            _timer = Timer;
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

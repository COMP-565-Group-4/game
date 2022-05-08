using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeRespawner : MonoBehaviour
{
    [Tooltip("Add the items you want to respawn here.")]
    public GameObject[] Things;
    private GameObject[] newThings;

    // Start is called before the first frame update
    void Start() { }

    void Prepare()
    {
        foreach (GameObject thing in Things) {
            GameObject newThing = Instantiate(thing);
            newThing.name = thing.name;
        }
    }

    void Respawn()
    {
        // foreach (GameObject thing in Things) {
        //     if (transform.Find(thing.name) == null) {
        //         GameObject newThing = Instantiate(thing);
        //         newThing.name = thing.name;
        //     }
        // }
        print("This feature is a work in progress!");
    }
}

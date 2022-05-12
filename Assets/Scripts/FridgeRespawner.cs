using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeRespawner : MonoBehaviour
{
    [Tooltip("Add the items you want to respawn here. Make sure they're inactive.")]
    public GameObject[] Things;

    // Start is called before the first frame update
    void Start()
    {
        Respawn();
    }

    void Respawn()
    {
        foreach (GameObject thing in Things) {
            if (GameObject.Find(thing.name) == null) {
                GameObject newThing = Instantiate(thing);
                newThing.name = thing.name;
                newThing.SetActive(true);
            }
        }
    }
}

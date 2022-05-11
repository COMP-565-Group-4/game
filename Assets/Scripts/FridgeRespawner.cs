using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeRespawner : MonoBehaviour
{
    [Tooltip("Add the items you want to respawn here.")]
    public GameObject[] Things;
    private GameObject[] newThings;

    // Start is called before the first frame update
    void Start()
    {
        Prepare();
    }

    void Prepare()
    {
        newThings = new GameObject[Things.Length];
        for (int i = 0; i < Things.Length; i++) {
            newThings[i] = Instantiate(Things[i]);
            newThings[i].name = Things[i].name;
            newThings[i].SetActive(false);
        }
    }

    void Respawn()
    {
        // print("This feature is a work in progress!");
        for (int i = 0; i < Things.Length; i++) {
            if (Things[i] == null || !Things[i].activeSelf) {
                Things[i] = Instantiate(newThings[i]);
                Things[i].name = newThings[i].name;
                Things[i].SetActive(true);
            }
        }
        Prepare();
    }
}

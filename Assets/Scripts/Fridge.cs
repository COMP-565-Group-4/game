using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction {
public class Fridge : Interactable
{
    public GameObject upperDoor;
    public GameObject lowerDoor;
    private bool doorClosed = true;

    private enum FridgeType
    {
        Single,
        Double
    }
    ;
    [SerializeField]
    private FridgeType fridgeType;

    // // Start is called before the first frame update
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }

    protected override void Interact()
    {
        if (!doorClosed) {
            // close door
            if (fridgeType == FridgeType.Single) {
                upperDoor.transform.localPosition =
                    new Vector3(-0.296257734f, -0.0573703051f, 0.168954581f);
                lowerDoor.transform.localPosition =
                    new Vector3(-0.296123713f, -0.727697492f, 0.213802174f);
                upperDoor.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                lowerDoor.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            } else if (fridgeType == FridgeType.Double) {
                upperDoor.transform.localPosition =
                    new Vector3(-0.510672569f, -0.781334043f, 0.149223432f);
                lowerDoor.transform.localPosition =
                    new Vector3(-0.510672569f, -0.781334043f, 0.149223432f);
                upperDoor.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                lowerDoor.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            }
            // call item respawner script
            transform.SendMessage("Respawn");
        } else {
            // open door
            if (fridgeType == FridgeType.Single) {
                upperDoor.transform.localPosition =
                    new Vector3(-0.30925557f, -0.0573703051f, 0.166886821f);
                lowerDoor.transform.localPosition =
                    new Vector3(-0.352999985f, -0.727697492f, 0.157000005f);
                upperDoor.transform.localRotation = Quaternion.Euler(0f, -90.0f, 0f);
                lowerDoor.transform.localRotation = Quaternion.Euler(0f, -90.0f, 0f);
            } else if (fridgeType == FridgeType.Double) {
                upperDoor.transform.localPosition = new Vector3(-0.5f, -0.781334043f, 0.149223432f);
                lowerDoor.transform.localPosition = new Vector3(-0.5f, -0.781334043f, 0.149223432f);
                upperDoor.transform.localRotation = Quaternion.Euler(0f, -90.0f, 0f);
                lowerDoor.transform.localRotation = Quaternion.Euler(0f, -90.0f, 0f);
            }
        }

        doorClosed = !doorClosed;
    }

    protected override void Hold() { }
}
}

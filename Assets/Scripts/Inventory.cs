using UnityEngine;
using UI;

public class Inventory : MonoBehaviour
{
    public static GameObject HeldItem;
    private static HUDManager _hudManager;

    void Start()
    {
        _hudManager = GameObject.Find("HUD").GetComponent<HUDManager>();
    }

    // method for picking up items (note: make sure to clean up the object reference once you call
    // this!)
    public static void AddItem(GameObject item)
    {
        // allocate the item slot
        HeldItem = item;
        // hide the item we just grabbed
        HeldItem.SetActive(false);
        // update hud
        _hudManager.HeldItem = item.name;
    }

    public static GameObject RemoveItem()
    {
        GameObject item = HeldItem;
        // clear the item slot
        HeldItem = null;
        // update hud
        _hudManager.HeldItem = "nothing";
        return item;
    }

    // TODO: "drop item" method, probably using a position parameter?
}

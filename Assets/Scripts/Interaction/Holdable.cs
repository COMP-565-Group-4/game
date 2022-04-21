using ScriptableObjects;

using UnityEngine;

namespace Interaction {
public class Holdable : Interactable
{
    public PlayerData playerData;

    protected override void Hold()
    {
        if (Inventory.HeldItem == null) // we aren't holding anything
        {
            // pick up object
            Inventory.AddItem(transform.gameObject);
        } else {
            if (playerData.SwapItems == true) {
                GameObject oldItem = Inventory.RemoveItem();
                oldItem.transform.position = transform.gameObject.transform.position;
                oldItem.SetActive(true);
                Inventory.AddItem(transform.gameObject);
            } else {
                print(
                    "Cannot pick up " + transform.name + ", currently holding a "
                    + Inventory.HeldItem.name
                );
            }
        }
    }

    protected override void Interact() { }
}
}

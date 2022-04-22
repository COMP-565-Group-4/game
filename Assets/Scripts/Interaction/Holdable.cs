using ScriptableObjects;

using UnityEngine;

namespace Interaction {
public class Holdable : Interactable
{
    public PlayerData playerData;

    protected override void Hold()
    {
        if (inventory.HeldItem == null) // we aren't holding anything
        {
            // pick up object
            inventory.AddItem(transform.gameObject);
        } else {
            if (playerData.SwapItems == true) {
                GameObject oldItem = inventory.RemoveItem();
                oldItem.transform.position = transform.gameObject.transform.position;
                oldItem.SetActive(true);
                inventory.AddItem(transform.gameObject);
            } else {
                print(
                    "Cannot pick up " + transform.name + ", currently holding a "
                    + inventory.HeldItem.name
                );
            }
        }
    }

    protected override void Interact() { }
}
}

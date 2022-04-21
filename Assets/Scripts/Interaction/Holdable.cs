using UnityEngine;

namespace Interaction {
public class Holdable : Interactable
{
    protected override void Hold()
    {
        if (Inventory.HeldItem == null) // we aren't holding anything
        {
            // pick up object
            Inventory.AddItem(transform.gameObject);
        } else {
            // TODO: support SwapItems flag
            // if (SwapItems == true) {
            GameObject oldItem = Inventory.RemoveItem();
            oldItem.transform.position = transform.gameObject.transform.position;
            oldItem.SetActive(true);
            Inventory.AddItem(transform.gameObject);
            /*} else {
                print(
                    "Cannot pick up " + transform.name + ", currently holding a "
                    + Inventory.HeldItem.name
                );
            }*/
        }
    }

    protected override void Interact() { }
}
}

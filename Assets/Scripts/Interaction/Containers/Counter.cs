using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.DualShock;

namespace Interaction.Containers {
public sealed class Counter : Container
{
    public UnityEvent<Ingredient> OrderSubmitEvent;

    protected override void Extract()
    {
        print("There's nothing on the counter.");
    }

    protected override void Insert()
    {
        // TODO: don't allow item to be placed if it's not for any active order.
        try {
            var ingredient = inventory.HeldItem.GetComponentInChildren<Ingredient>();

            // Done last to ensure it has the component before removing it.
            var item = inventory.RemoveItem();

            // Move dish to counter and display it
            item.transform.position = transform.position;
            item.SetActive(true);

            Destroy(item, 3);
            OrderSubmitEvent.Invoke(ingredient);

            Debug.Log($"Placed {item.name} on the counter.");
        } catch (MissingComponentException) {
            print($"Can't place {inventory.HeldItem.name} on the counter: not an ingredient.");
        }
    }

    protected override void Interact() { }
}
}

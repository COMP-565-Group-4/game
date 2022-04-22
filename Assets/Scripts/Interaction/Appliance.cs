using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using ScriptableObjects;

using UnityEngine;

namespace Interaction {
public class Appliance : Container
{
    [Tooltip("Items which this appliance can produce")]
    public Ingredient[] Products = Array.Empty<Ingredient>();

    protected readonly Stack<Ingredient> Inputs = new Stack<Ingredient>();
    protected Ingredient Output = null;
    protected bool Busy = false;

    protected override void Extract()
    {
        if (Output is null) {
            if (Inputs.Count > 0) {
                var ingredient = Inputs.Pop();
                var item = Instantiate(ingredient.gameObject);
                item.name = ingredient.gameObject.name;
                inventory.AddItem(item);
            } else {
                print("Appliance has no items left.");
            }
        } else if (Busy) {
            print("Appliance is already producing something; please wait for it to complete.");
        } else {
            var item = Instantiate(Output);
            item.name = Output.name;
            inventory.AddItem(item.gameObject);
            Output = null;
        }
    }

    protected override void Insert()
    {
        if (IsBusyOrComplete())
            return;

        try {
            var ingredient = inventory.HeldItem.GetComponentInChildren<Ingredient>();
            Inputs.Push(ingredient);

            // Done last to ensure it has the component before removing it.
            var item = inventory.RemoveItem();

            Debug.Log($"Inserted {item.name} into the appliance.");
        } catch (MissingComponentException) {
            print($"Can't insert {inventory.HeldItem.name} into appliance: not an ingredient.");
        }
    }

    protected override void Interact()
    {
        if (IsBusyOrComplete())
            return;

        // TODO: disallow multiple products that require the same inputs?
        foreach (var product in Products) {
            // TODO: more robust (but still DRY) way to determine ingredient type
            var productNames = product.ChildIngredients.Select(i => i.name);
            var inputNames = Inputs.Select(i => i.name);

            if (productNames.SequenceEqual(inputNames)) {
                Output = product;
                StartCoroutine(Produce());

                return; // Found a valid recipe; no need to keep searching.
            }
        }

        print("Recipe not found!");
    }

    protected virtual IEnumerator Produce()
    {
        Busy = true;
        // TODO: make wait time configurable.
        yield return new WaitForSeconds(3f);

        Inputs.Clear();
        Busy = false;
    }

    private bool IsBusyOrComplete()
    {
        if (Busy) {
            print("Appliance is already producing something; please wait for it to complete.");

            return true;
        }

        if (Output != null) {
            print("Appliance already produced something; please extract it first.");

            return true;
        }

        return false;
    }
}
}

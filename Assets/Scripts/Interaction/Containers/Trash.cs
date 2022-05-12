using System.Collections;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;

namespace Interaction {
public class Trash : Container
{
    protected override void Interact() { }
    protected override void Extract() { }
    protected override void Insert()
    {
        Destroy(Inventory.RemoveItem());
    }
}

}

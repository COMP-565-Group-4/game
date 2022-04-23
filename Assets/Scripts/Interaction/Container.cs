using ScriptableObjects;

namespace Interaction {
public abstract class Container : Interactable
{
    protected override void Hold()
    {
        if (Inventory.HeldItem == null) {
            Extract();
        } else {
            Insert();
        }
    }

    protected abstract void Extract();

    protected abstract void Insert();
}

}

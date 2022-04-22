using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Inventory")]
    public class Inventory : ScriptableObject
    {
        public GameObject HeldItem;
        public UnityEvent<GameObject> itemChangedEvent;

        /// <summary>
        /// Holds the given item and deactivates it.
        /// </summary>
        /// <remarks>
        /// Make sure to clean up the object reference after calling this.
        /// </remarks>
        /// <param name="item">The item to hold.</param>
        public void AddItem(GameObject item)
        {
            HeldItem = item; // Store the held item.
            HeldItem.SetActive(false); // Hide the held item.
            itemChangedEvent.Invoke(HeldItem);
        }

        /// <summary>
        /// Removes the currently held item.
        /// </summary>
        /// <returns>The removed item.</returns>
        public GameObject RemoveItem()
        {
            GameObject item = HeldItem; // Temporarily save the held item.
            HeldItem = null; // Clear the held item.
            itemChangedEvent.Invoke(HeldItem);

            return item;
        }

        // TODO: "drop item" method, probably using a position parameter?
    }
}

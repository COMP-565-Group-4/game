using System;
using System.Linq;

using ScriptableObjects;

using TMPro;

using UnityEngine;

namespace UI {
public class HUDManager : MonoBehaviour
{
    public Inventory inventory;

    [Header("Text")]
    [Tooltip("TMP component for the timer text")]
    public TextMeshProUGUI TimeText;

    [Tooltip("TMP component for the round text")]
    public TextMeshProUGUI RoundText;

    [Tooltip("TMP component for the money text")]
    public TextMeshProUGUI MoneyText;

    [Tooltip("TMP component for the order's recipe's name text")]
    public TextMeshProUGUI OrderRecipeNameText;

    [Tooltip("TMP component for the order number text")]
    public TextMeshProUGUI OrderNumberText;

    [Tooltip("TMP component for the order's recipe text")]
    public TextMeshProUGUI OrderRecipeText;

    [Tooltip("TMP component for the held item name text")]
    public TextMeshProUGUI HeldItemNameText;

    private Order order;
    private uint currentOrder = 0;
    private uint totalOrders = 0;

    public void RoundStartEventHandler(Round round, uint number, uint total)
    {
        RoundText.text = $"{number}/{total}";
        order = null;
        currentOrder = 0;
        totalOrders = 0;
    }

    public void OrderCreateEventHandler(Order order)
    {
        totalOrders += 1;
        if (this.order is null) {
            // This is the first order; display it.
            this.order = order;
            currentOrder = 1;
            SetAllOrderText();
        } else {
            // Otherwise, just make sure the total is up to date.
            // New orders are added to the end, so the current number won't change.
            OrderNumberText.text = $"{currentOrder}/{totalOrders}";
        }
    }

    public void OrderCompleteEventHandler(Order order, uint currentOrder)
    {
        this.currentOrder = currentOrder + 1;
        DecrementTotalOrders();
    }

    public void OrderFailureEventHandler(Order order, uint currentOrder)
    {
        this.currentOrder = currentOrder + 1;
        DecrementTotalOrders();
    }

    public void OrderNextEventHandler(Order order, uint currentOrder)
    {
        this.order = order;
        this.currentOrder = currentOrder + 1;

        SetAllOrderText();
    }

    private void OnEnable()
    {
        inventory.itemChangedEvent.AddListener(ItemChangedEventHandler);
    }

    private void OnDisable()
    {
        inventory.itemChangedEvent.RemoveListener(ItemChangedEventHandler);
    }

    private void ItemChangedEventHandler(GameObject item)
    {
        HeldItemNameText.text = item is null ? "nothing" : item.name;
    }

    private void DecrementTotalOrders()
    {
        totalOrders -= 1;
        OrderNumberText.text = $"{currentOrder}/{totalOrders}";

        if (totalOrders == 0) {
            OrderRecipeNameText.text = "";
            OrderRecipeText.text = "No more orders currently available.";
        }
    }

    private void SetAllOrderText()
    {
        OrderRecipeNameText.text = order.Meal.name;
        OrderNumberText.text = $"{currentOrder}/{totalOrders}";

        // TODO: merge identical ingredient to display a count instead e.g. "2x egg".
        OrderRecipeText.text = string.Join("\n", order.Meal.ChildIngredients.Select(i => i.name));
    }
}
}

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
    private uint currentOrder;
    private uint totalOrders;

    public void RoundStartEventHandler(Round round, uint number, uint total)
    {
        RoundText.text = $"{number}/{total}";
        order = null;
        currentOrder = 0;
        totalOrders = 0;
    }

    public void OrderCreateEventHandler(Order order)
    {
        if (this.order is null) {
            // No order is being displayed, so display the created order.
            OrderNextEventHandler(order, 0, 1);
        } else {
            // An order is being displayed; just update the total.
            totalOrders += 1;
            SetOrderNumber();
        }
    }

    public void OrderNextEventHandler(Order order, uint currentOrder, uint totalOrders)
    {
        if (order is null) {
            currentOrder = 0;

            if (this.order != null) {
                OrderRecipeNameText.text = "";
                OrderRecipeText.text = "No more orders currently available.";
            }
        } else {
            ++currentOrder;

            if (this.order != order) {
                OrderRecipeNameText.text = order.Meal.name;
                // TODO: merge identical ingredient to display a count instead e.g. "2x egg".
                OrderRecipeText.text =
                    string.Join("\n", order.Meal.ChildIngredients.Select(i => i.name));
            }
        }

        this.order = order;
        this.currentOrder = currentOrder;
        this.totalOrders = totalOrders;

        SetOrderNumber();
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

    private void SetOrderNumber()
    {
        OrderNumberText.text = $"{currentOrder}/{totalOrders}";
    }
}
}

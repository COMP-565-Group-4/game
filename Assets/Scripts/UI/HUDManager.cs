using System;
using System.Collections.Generic;
using System.Linq;

using ScriptableObjects;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

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

    private LinkedListNode<Order> shownOrder;

    private void Update()
    {
        var minutes = Math.DivRem((long) RoundManager.Instance.Time, 60, out long seconds);
        TimeText.text = $"{minutes:00}:{seconds:00}";
    }

    public void RoundStartEventHandler(Round round, uint number, uint total)
    {
        RoundText.text = $"{number}/{total}";
        shownOrder = null;
    }

    public void OrderCreateEventHandler(LinkedListNode<Order> order)
    {
        if (shownOrder is null) {
            // No orders are being displayed. Thus, display this order.
            shownOrder = order;
            SetOrderText();
        }
    }

    public void OrderCompleteEventHandler(LinkedListNode<Order> order)
    {
        // TODO: Avoid parsing string?
        // Can't use RoundManager.Money cause it's updated by the same event.
        MoneyText.text = (int.Parse(MoneyText.text) + order.Value.Reward).ToString();

        HandleRemovedOrder(order);
    }

    public void OrderFailureEventHandler(LinkedListNode<Order> order)
    {
        HandleRemovedOrder(order);
    }

    public void NextOrderEventHandler(InputAction.CallbackContext context)
    {
        if (!context.performed || shownOrder is null || shownOrder.Next == shownOrder.List.First)
            return;

        shownOrder = shownOrder.Next ?? shownOrder.List.First;
        SetOrderText();
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

    private void HandleRemovedOrder(LinkedListNode<Order> order)
    {
        if (order.List.Count == 1) {
            shownOrder = null;
            ClearOrderText();
        } else if (order == shownOrder) {
            shownOrder = order.Next ?? order.List.First;
            SetOrderText();
        }
    }

    private void ClearOrderText()
    {
        OrderNumberText.text = "";
        OrderRecipeNameText.text = "";
        OrderRecipeText.text = "No more orders currently available.";
    }

    private void SetOrderText()
    {
        OrderNumberText.text = $"#{shownOrder.Value.ID}";
        OrderRecipeNameText.text = shownOrder.Value.Meal.name;

        // TODO: merge identical ingredient to display a count instead e.g. "2x egg".
        OrderRecipeText.text =
            string.Join("\n", shownOrder.Value.Meal.ChildIngredients.Select(i => i.name));
    }
}
}

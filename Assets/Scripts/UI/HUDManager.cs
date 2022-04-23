using System;
using System.Collections.Generic;
using System.Linq;

using ScriptableObjects;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI {
public class HUDManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Player's inventory")]
    private Inventory inventory;

    [Header("Text")]
    [SerializeField]
    [Tooltip("TMP component for the round timer text")]
    private TextMeshProUGUI roundTime;

    [SerializeField]
    [Tooltip("TMP component for the round text")]
    private TextMeshProUGUI round;

    [SerializeField]
    [Tooltip("TMP component for the money text")]
    private TextMeshProUGUI money;

    [SerializeField]
    [Tooltip("TMP component for the order's name text")]
    private TextMeshProUGUI orderName;

    [SerializeField]
    [Tooltip("TMP component for the order timer text")]
    private TextMeshProUGUI orderTime;

    [SerializeField]
    [Tooltip("TMP component for the order's required ingredients text")]
    private TextMeshProUGUI orderIngredients;

    [SerializeField]
    [Tooltip("TMP component for the held item name text")]
    private TextMeshProUGUI heldItemName;

    private LinkedListNode<Order> shownOrder;

    private void Update()
    {
        roundTime.text = FormatTime(RoundManager.Instance.Time);
        orderTime.text = shownOrder is null ? "" : FormatTime(shownOrder.Value.TimeRemaining);
    }

    public void RoundStartEventHandler(Round round, uint number, uint total)
    {
        this.round.text = $"{number}/{total}";
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
        money.text = (int.Parse(money.text) + order.Value.Reward).ToString();

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
        heldItemName.text = item is null ? "nothing" : item.name;

        // Apparently this isn't ideal for performance, but IDK why it doesn't update on its own.
        LayoutRebuilder.ForceRebuildLayoutImmediate(
            heldItemName.GetComponentInParent<RectTransform>()
        );
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
        orderName.text = "";
        orderIngredients.text = "No more orders currently available.";
    }

    private void SetOrderText()
    {
        orderName.text = shownOrder.Value.Meal.name + " (#" + shownOrder.Value.ID + ")";

        // TODO: merge identical ingredient to display a count instead e.g. "2x egg".
        orderIngredients.text =
            string.Join("\n", shownOrder.Value.Meal.ChildIngredients.Select(i => i.name));
    }

    private string FormatTime(float time)
    {
        var minutes = Math.DivRem((long) time, 60, out long seconds);
        return $"{minutes:00}:{seconds:00}";
    }
}
}

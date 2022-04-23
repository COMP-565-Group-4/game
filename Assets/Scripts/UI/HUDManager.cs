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

    public void RoundStartEventHandler(Round round, uint number, uint total)
    {
        RoundText.text = $"{number}/{total}";
    }

    public void OrderCompleteEventHandler(Order order)
    {
        // TODO: Avoid parsing string? But I don't want to duplicate tracking the money state...
        MoneyText.text = (int.Parse(MoneyText.text) + order.Reward).ToString();
    }

    public void OrderCreateEventHandler(Order order)
    {
        if (OrderManager.Instance.OrdersCount == 1) {
            // No order is being displayed, so display the created order.
            OrderNextEventHandler(order, 0, 1);
        } else {
            // An order is being displayed; just update the total.
            SetOrderNumber();
        }
    }

    public void OrderNextEventHandler(Order order, uint selectedOrder, uint ordersCount)
    {
        if (order is null) {
            OrderRecipeNameText.text = "";
            OrderRecipeText.text = "No more orders currently available.";
        } else if (OrderRecipeNameText.text != order.Meal.name) {
            OrderRecipeNameText.text = order.Meal.name;
            // TODO: merge identical ingredient to display a count instead e.g. "2x egg".
            OrderRecipeText.text =
                string.Join("\n", order.Meal.ChildIngredients.Select(i => i.name));
        }

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
        var selectedOrder = OrderManager.Instance.OrdersCount == 0
            ? 0
            : OrderManager.Instance.SelectedOrderNumber + 1;
        OrderNumberText.text = selectedOrder + "/" + OrderManager.Instance.OrdersCount;
    }
}
}

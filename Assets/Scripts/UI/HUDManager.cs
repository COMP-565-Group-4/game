using System;

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

    [Header("Values")]
    [SerializeField]
    [Tooltip("Current round")]
    private uint round;

    [SerializeField]
    [Tooltip("Total number of rounds")]
    private uint totalRounds;

    [SerializeField]
    [Tooltip("Amount of money the player possesses")]
    private uint money;

    [Header("Time")]
    [SerializeField]
    [Tooltip("Minutes remaining for the current round")]
    private uint minutes;

    [SerializeField]
    [Tooltip("Seconds remaining for the current round")]
    private uint seconds;

    [Header("Orders")]
    [SerializeField]
    [Tooltip("Currently displayed order's number")]
    private uint order;

    [SerializeField]
    [Tooltip("Total number of orders")]
    private uint totalOrders;

    [SerializeField]
    [Tooltip("Currently displayed order's recipe's name")]
    private string orderRecipeName;

    [SerializeField]
    [Tooltip("Currently displayed order's recipe")]
    private string orderRecipe;

    public uint Round
    {
        get => round;
        set {
            round = value;
            RoundText.text = $"{value}/{TotalRounds}";
        }
    }

    public uint TotalRounds
    {
        get => totalRounds;
        set {
            totalRounds = value;
            RoundText.text = $"{Round}/{value}";
        }
    }

    public uint Money
    {
        get => money;
        set {
            money = value;
            MoneyText.text = value.ToString();
        }
    }

    public uint Minutes
    {
        get => minutes;
        set {
            minutes = value;
            TimeText.text = $"{value:00}:{Seconds:00}";
        }
    }

    public uint Seconds
    {
        get => seconds;
        set {
            seconds = value;
            TimeText.text = $"{Minutes:00}:{value:00}";
        }
    }

    public uint Order
    {
        get => order;
        set {
            order = value;
            OrderNumberText.text = $"{value}/{TotalOrders}";
        }
    }

    public uint TotalOrders
    {
        get => totalOrders;
        set {
            totalOrders = value;
            OrderNumberText.text = $"{Order}/{value}";
        }
    }

    public string OrderRecipeName
    {
        get => orderRecipeName;
        set {
            orderRecipeName = value;
            OrderRecipeNameText.text = value;
        }
    }

    public string OrderRecipe
    {
        get => orderRecipe;
        set {
            orderRecipe = value;
            OrderRecipeText.text = value;
        }
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

#if UNITY_EDITOR
    /// <summary>
    /// Triggers setters of properties when a backing field is updated via the inspector.
    /// </summary>
    private void OnValidate()
    {
        Round = round;
        TotalRounds = totalRounds;
        Money = money;
        Minutes = minutes;
        Seconds = seconds;
        Order = order;
        TotalOrders = totalOrders;
        OrderRecipeName = orderRecipeName;
        OrderRecipe = orderRecipe;
    }
#endif
}
}

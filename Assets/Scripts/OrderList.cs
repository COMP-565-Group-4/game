using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UI;

public class OrderList : MonoBehaviour
{
    private List<Order> Orders = new List<Order>();
    public GameObject OrderPrefab; // is this a better way to do this? idk
    private static int _currentOrderIndex;
    private int _lastOrderIndex = -1;
    private HUDManager _hudManager;

    [Header("Order Attributes")]
    [Tooltip("Maximum number of orders at once")]
    public int MaxRunningOrders = 3; // currently unused
    [Tooltip("Maximum number of orders this round")]
    public int OrdersPerDay = 10; // currently unused
    [Tooltip("Minimum seconds between new orders")]
    public float OrderCooldown = 15;
    [Tooltip("The base time limit for each order this round")]
    public float BaseOrderTimeLimit = 30;
    [Tooltip("The default price of an order (if a price cannot be calculated dynamically)")]
    public int DefaultOrderPrice = 100;

    [Tooltip("The list of requested dishes for each round")]
    public string[] Dishes = {}; // todo: change this to GameObject list maybe?

    private float _cooldown;

    // Start is called before the first frame update
    void Start()
    {
        _hudManager = GameObject.Find("HUD").GetComponent<HUDManager>();
        // add a random delay before the first order shows up
        _cooldown = (float) Random.Range(0, 5);
        RefreshOrderList();
    }

    // Update is called once per frame
    void Update()
    {
        // try to make a new order
        if (_cooldown > 0) {
            // decrement order cooldown
            _cooldown = _cooldown - Time.deltaTime;
        } else {
            // check: are we under the concurrent orders limit?
            if (Orders.Count < MaxRunningOrders) {
                // generate a new order and reset the timer
                GenerateOrder();
                _cooldown = OrderCooldown;
            } else {
                // add a random amount of extra time until we try again
                _cooldown = _cooldown + (float) Random.Range(0, 5);
            }
        }

        // update/render order list
    }

    void GenerateOrder()
    {
        // pick a random item from the list of possible orders
        int index = Random.Range(0, Dishes.Length - 1);
        if (index == _lastOrderIndex) // are we repeating the same order twice in a row?
        {
            // if so, reroll the index so we'll be less likely to do that
            index = Random.Range(0, Dishes.Length - 1);
        }
        _lastOrderIndex = index;

        // create an order (empty game object with custom components) for that recipe
        GameObject newOrderObject = Instantiate(OrderPrefab, transform);
        Order newOrder = newOrderObject.GetComponent<Order>();
        Countdown newCountdown = newOrderObject.GetComponent<Countdown>();

        // define the order's attributes (countdown, price, requested item, etc.)
        newOrder.Price = DefaultOrderPrice;
        newOrder.RequestedItem = Dishes[index];
        newOrder.Recipe =
            RecipeBook.Recipes.FirstOrDefault(x => x.Value == newOrder.RequestedItem).Key;
        newCountdown.Length = BaseOrderTimeLimit;
        newOrderObject.name = "Order";

        // print("New order: " + newOrder.RequestedItem + " for " + newOrder.Price + " gold!");
        Orders.Add(newOrder);
        RefreshOrderList();
    }

    public bool CheckOrder(string dish)
    {
        if (Orders.Count != 0) {
            // does the order list have this item?
            if (Orders.Exists(x => x.RequestedItem == dish)) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
    }

    void SubmitOrder(string dish)
    {
        if (CheckOrder(dish) == true) {
            Orders.Find(x => x.RequestedItem == dish).SendMessage("Complete");
        } else {
            print("ERROR: Could not find order!");
        }
    }

    void RemoveOrder(Order ord)
    {
        Orders.Remove(ord);
        Destroy(ord.gameObject);
        RefreshOrderList();
    }

    void CycleActiveOrder()
    {
        if (Orders.Count == 0) {
            RefreshOrderList();
        } else {
            _currentOrderIndex = (_currentOrderIndex + 1) % (int) Orders.Count;
            ListOrder(_currentOrderIndex);
        }
    }

    void RefreshOrderList()
    {
        if (Orders.Count == 0) {
            _hudManager.Order = 0;
            _hudManager.TotalOrders = 0;
            _hudManager.OrderRecipeName = "No orders active!";
            _hudManager.OrderRecipe = "";
        } else {
            if (_hudManager.TotalOrders == 0) {
                CycleActiveOrder();
            }

            _hudManager.TotalOrders = (uint) Orders.Count;
        }
    }

    void ListOrder(int index)
    {
        Order currentOrder = Orders[index];
        string currentOrderRecipe = "";
        foreach (string ingredient in currentOrder.Recipe) {
            currentOrderRecipe = currentOrderRecipe + ingredient + "\n";
        }

        _hudManager.Order = (uint) index + 1;
        _hudManager.TotalOrders = (uint) Orders.Count;
        _hudManager.OrderRecipeName = currentOrder.RequestedItem;
        _hudManager.OrderRecipe = currentOrderRecipe;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderList : MonoBehaviour
{
    private List<Order> Orders = new List<Order>();
    public GameObject OrderPrefab; // is this a better way to do this? idk

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
    public string[] Dishes = { "Capsule" };

    private float _cooldown;

    // Start is called before the first frame update
    void Start()
    {
        // TODO: generate an inverse dictionary on round start based on the dish list
        // from which to retrieve recipes
        _cooldown = OrderCooldown;
    }

    // Update is called once per frame
    void Update()
    {
        // try to make a new order
        if (_cooldown > 0) {
            // decrement order cooldown
            _cooldown = _cooldown - Time.deltaTime;
        } else {
            // generate a new order and reset the timer
            GenerateOrder();
            _cooldown = OrderCooldown;
        }

        // update/render order list
    }

    void GenerateOrder()
    {
        // pick a random item from the list of possible orders
        int index = Random.Range(0, Dishes.Length - 1);

        // create an order (empty game object with custom components) for that recipe
        GameObject newOrderObject = Instantiate(OrderPrefab, transform);
        Order newOrder = newOrderObject.GetComponent<Order>();
        Countdown newCountdown = newOrderObject.GetComponent<Countdown>();

        // define the order's attributes (countdown, price, requested item, etc.)
        // newOrder.Recipe = ????
        newOrder.Price = DefaultOrderPrice;
        newOrder.RequestedItem = Dishes[index];
        newCountdown.Length = BaseOrderTimeLimit;

        print("New order: " + newOrder.RequestedItem + " for " + newOrder.Price + " gold!");
        Orders.Add(newOrder);
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
    }
}

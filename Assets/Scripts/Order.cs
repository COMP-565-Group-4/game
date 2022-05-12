using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Order : MonoBehaviour
{
    public int Price; // how much you get for completing the order
    public List<string> Recipe; // the recipe (for displaying on the hud)
    public string RequestedItem; // what player turns in to complete the order

    private OrderList _orderList; // the current level's OrderList

    // Start is called before the first frame update
    void Start()
    {
        _orderList = GameObject.Find("OrderList").GetComponent<OrderList>();
    }

    // Update is called once per frame
    void Update() { }

    void Complete()
    {
        // order was completed successfully, award money and get rid of this order
        print(RequestedItem + " delivered successfully!");
        GameState.AddMoney(Price);
        _orderList.SendMessage("RemoveOrder", this);
    }

    void Timeout()
    {
        // timer ran out, get rid of this order (and possibly penalize the player)
        print("Out of time, " + RequestedItem + " order failed!");
        _orderList.SendMessage("RemoveOrder", this);
    }
}

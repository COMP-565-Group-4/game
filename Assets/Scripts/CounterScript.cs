using UnityEngine;

public class CounterScript : MonoBehaviour
{
    public GameObject OrderList;

    // Start is called before the first frame update
    void Start()
    {
        OrderList = GameObject.Find("OrderList");
    }

    // Update is called once per frame
    void Update() { }

    void Insert()
    {
        // retrieve the object from player's inventory
        GameObject dish = Inventory.RemoveItem();

        // move dish to counter and display it
        dish.transform.position = transform.position;
        dish.SetActive(true);

        // see if anyone asked for this dish
        bool hasOrder = OrderList.GetComponent<OrderList>().CheckOrder(dish.name);
        if (hasOrder) {
            // submit the order and then get rid of the dish
            // (we can add an animation for this later)
            OrderList.SendMessage("SubmitOrder", dish.name);
            Destroy(dish, 3);
        } else {
            // just get rid of the dish
            print("Sorry, we couldn't find a matching order for that " + dish.name + "!");
            Destroy(dish, 3);
        }
    }

    void Extract()
    {
        print("There's nothing on the counter.");
    }
}

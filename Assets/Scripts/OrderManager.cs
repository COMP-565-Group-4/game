using System;
using System.Collections.Generic;
using System.Linq;

using ScriptableObjects;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

using Random = UnityEngine.Random;

// TODO: can a separate class be avoided?
// Since Order is a ScriptableObject, modifications to its fields persist.
public class OrderData
{
    public readonly Order Order;
    public float Time;

    public OrderData(Order order)
    {
        Order = order;
        Time = order.Time;
    }
}

public class OrderManager : MonoBehaviour
{
    public UnityEvent<Order> OrderCreateEvent;
    public UnityEvent<Order, uint> OrderCompleteEvent;
    public UnityEvent<Order, uint> OrderFailureEvent;
    public UnityEvent<Order, uint> OrderNextEvent;

    private Round round;
    private List<OrderData> orders = new List<OrderData>();
    private int currentOrder = 0;
    private float newOrderDelta;
    private bool isRunning = false;

    private void Update()
    {
        if (GameState.GamePaused || !isRunning)
            return;

        UpdateOrderTimes();
        MaybeCreateOrder();
    }

    /// <summary>
    /// Stops tracking the order if a related order is found for the <paramref name="ingredient"/>.
    /// </summary>
    /// <remarks>
    /// Invokes <see cref="OrderCompleteEvent"/> if the submission is accepted.
    /// Does nothing if a related order cannot be found.
    /// </remarks>
    /// <param name="ingredient">The final <see cref="Ingredient"/> submitted.</param>
    public void OrderSubmitEventHandler(Ingredient ingredient)
    {
        try {
            // TODO: A List isn't efficient for this.
            // TODO: More robust comparison than using names?
            var order = orders.First(order => order.Order.Meal.name == ingredient.name);
            orders.Remove(order);
            OrderCompleteEvent.Invoke(order.Order, (uint) currentOrder);

            Debug.Log($"Order for {order.Order.Meal.name} was completed!");
        } catch (InvalidOperationException) {
            Debug.Log("No associated order found; ignoring meal.");
        }
    }

    /// <summary>
    /// Resets internal state when a round starts.
    /// </summary>
    /// <param name="newRound">The <see cref="Round"/> that started.</param>
    /// <param name="roundNumber">The number of the round that started.</param>
    /// <param name="totalRounds">The total number of rounds in the game.</param>
    public void RoundStartEventHandler(Round newRound, uint roundNumber, uint totalRounds)
    {
        // Initialise it to be > the frequency so a new order is instantly created.
        newOrderDelta = newRound.OrderFrequency + 1;

        round = newRound;
        isRunning = true;
    }

    /// <summary>
    /// Stops round timers and clears orders when the round ends.
    /// </summary>
    /// <param name="endedRound">The <see cref="Round"/> that ended.</param>
    /// <param name="roundNumber">The number of the round that ended.</param>
    public void RoundEndEventHandler(Round endedRound, uint roundNumber)
    {
        isRunning = false;
        orders.Clear();
    }

    /// <summary>
    /// Cycles the current order to display and invokes <see cref="OrderNextEvent"/>.
    /// </summary>
    /// <param name="context">The context of the triggered action.</param>
    public void NextOrderInputEventHandler(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        currentOrder = (currentOrder + 1) % orders.Count;
        OrderNextEvent.Invoke(orders[currentOrder].Order, (uint) currentOrder);
    }

    /// <summary>
    /// Updates the remaining time for all active orders.
    /// </summary>
    /// <remarks>
    /// Invokes <see cref="OrderFailureEvent"/> if an order's time runs out.
    /// </remarks>
    private void UpdateOrderTimes()
    {
        // TODO: inefficient...
        // Can't modify the list while iterating it.
        var failures = new List<OrderData>();

        foreach (var order in orders) {
            order.Time -= Time.deltaTime;

            if (order.Time <= 0) {
                failures.Add(order);
                OrderFailureEvent.Invoke(order.Order, (uint) currentOrder);
            }
        }

        orders = orders.Except(failures).ToList();
    }

    /// <summary>
    /// Creates a new order based on the round's <see cref="Round.OrderFrequency"/> and
    /// <see cref="Round.MaxConcurrentOrders"/>.
    /// </summary>
    /// <remarks>
    /// Invokes <see cref="OrderCreateEvent"/> when a new order is created.
    /// </remarks>
    private void MaybeCreateOrder()
    {
        newOrderDelta += Time.deltaTime;

        if (newOrderDelta >= round.OrderFrequency && orders.Count < round.MaxConcurrentOrders) {
            var order = round.Orders[Random.Range(0, round.Orders.Length)];
            orders.Add(new OrderData(order));
            newOrderDelta = 0;
            OrderCreateEvent.Invoke(order);

            Debug.Log($"Created new order for {order.Meal.name}.");
        }
    }
}

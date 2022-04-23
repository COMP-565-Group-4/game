using System;
using System.Collections.Generic;
using System.Linq;

using ScriptableObjects;

using UnityEngine;
using UnityEngine.Events;

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
    public UnityEvent<Order> OrderCompleteEvent;
    public UnityEvent<Order> OrderFailureEvent;

    private Round round;
    private List<OrderData> orders = new List<OrderData>();
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
            var order = orders.First(order => order.Order.Meal == ingredient);
            orders.Remove(order);
            OrderCompleteEvent.Invoke(order.Order);
        } catch (InvalidOperationException) {
            Debug.Log("No associated order found; ignoring meal.");
        }
    }

    /// <summary>
    /// Resets internal state when a round starts.
    /// </summary>
    /// <param name="newRound">The <see cref="Round"/> that started.</param>
    /// <param name="roundNumber">The number of the round that started.</param>
    public void RoundStartEventHandler(Round newRound, uint roundNumber)
    {
        round = newRound;
        newOrderDelta = 0;
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
                OrderFailureEvent.Invoke(order.Order);
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
        }
    }
}

using System.Collections.Generic;

using ScriptableObjects;

using UnityEngine;
using UnityEngine.Events;

using Utils;

using Random = UnityEngine.Random;

public class OrderManager : Singleton<OrderManager>
{
    public UnityEvent<LinkedListNode<Order>> OrderCreateEvent;
    public UnityEvent<LinkedListNode<Order>> OrderCompleteEvent;
    public UnityEvent<LinkedListNode<Order>> OrderFailureEvent;

    private readonly LinkedList<Order> orders = new LinkedList<Order>();
    private float newOrderDelta = 0;
    private uint idCounter = 0;
    private bool started = false;

    public Round Round { get; private set; }

    private void Update()
    {
        if (GameState.Instance.GamePaused || !started)
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
        LinkedListNode<Order> node = orders.First;

        while (node != null) {
            // TODO: more robust comparison than using names
            if (node.Value.Meal.name == ingredient.name) {
                // Invoke event first so the node is still linked and Node.List isn't null.
                OrderCompleteEvent.Invoke(node);
                orders.Remove(node);
                Debug.Log($"Order for {ingredient.name} was completed!");

                return;
            }

            node = node.Next;
        }

        Debug.Log("No associated order found; ignoring meal.");
    }

    /// <summary>
    /// Resets internal state when a round starts.
    /// </summary>
    /// <param name="newRound">The <see cref="ScriptableObjects.Round"/> that started.</param>
    /// <param name="roundNumber">The number of the round that started.</param>
    /// <param name="totalRounds">The total number of rounds in the game.</param>
    public void RoundStartEventHandler(Round newRound, uint roundNumber, uint totalRounds)
    {
        // Initialise it to be > the frequency so a new order is instantly created.
        newOrderDelta = newRound.OrderFrequency + 1;

        Round = newRound;
        idCounter = 0;
        started = true;
    }

    /// <summary>
    /// Stops round timers and clears orders when the round ends.
    /// </summary>
    /// <param name="endedRound">The <see cref="ScriptableObjects.Round"/> that ended.</param>
    /// <param name="roundNumber">The number of the round that ended.</param>
    /// <param name="quit">Whether the player quit the round.</param>
    public void RoundEndEventHandler(Round endedRound, uint roundNumber, bool quit)
    {
        started = false;
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
        LinkedListNode<Order> node = orders.First;

        while (node != null) {
            node.Value.TimeRemaining -= Time.deltaTime;

            if (node.Value.TimeRemaining <= 0) {
                // Invoke event first so the node is still linked and Node.List isn't null.
                OrderFailureEvent.Invoke(node);
                orders.Remove(node);
            }

            node = node.Next;
        }
    }

    /// <summary>
    /// Creates a new order based on the round's <see
    /// cref="ScriptableObjects.Round.OrderFrequency"/> and <see
    /// cref="ScriptableObjects.Round.MaxConcurrentOrders"/>.
    /// </summary>
    /// <remarks>
    /// Invokes <see cref="OrderCreateEvent"/> when a new order is created.
    /// </remarks>
    private void MaybeCreateOrder()
    {
        newOrderDelta += Time.deltaTime;

        if (newOrderDelta < Round.OrderFrequency)
            return; // Too soon to create a new order.

        if (orders.Count >= Round.MaxConcurrentOrders)
            return; // Too many active orders already.

        if ((orders.Last?.Value.ID ?? 0) >= Round.OrderCount)
            return; // Maximum amount of new orders has been reached.

        // Select a random order from the pool of orders in the current round.
        var original = Round.Orders[Random.Range(0, Round.Orders.Length)];

        // Create a copy.
        var order = Instantiate(original);
        order.ID = ++idCounter;
        order.TimeRemaining = order.Time;

        var node = orders.AddLast(order);
        newOrderDelta = 0; // Reset how long it's been since a new order was created.

        OrderCreateEvent.Invoke(node);
        Debug.Log($"Created new order for {order.Meal.name}.");
    }
}

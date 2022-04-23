using System;

using ScriptableObjects;

using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour
{
    public Inventory Inventory;

    [Tooltip("Rounds that will be played (in the listed order)")]
    public Round[] Rounds;

    [Header("Events")]
    public UnityEvent<Round, uint> RoundStartEvent;
    public UnityEvent<Round, uint> RoundEndEvent;
    public UnityEvent<Round, uint> RoundNextEvent;

    private uint roundNumber = 1;
    private float time;
    private uint completedOrders = 0;
    private bool isRunning = false;

    private Round Round => Rounds[roundNumber - 1];

    private void Start()
    {
        // TODO: for testing purposes; remove once hooked up to start menu
        StartRound();
    }

    private void Update()
    {
        if (GameState.GamePaused || !isRunning)
            return;

        time -= Time.deltaTime;

        if (time <= 0) {
            EndRound();
        }
    }

    /// <summary>
    /// Starts the round and invokes <see cref="RoundStartEvent"/>.
    /// </summary>
    /// <remarks>
    /// Clears the held item in the inventory.
    /// </remarks>
    public void StartRound()
    {
        // TODO: is there a better place to do this? Maybe GameState, using RoundStartEvent?
        Inventory.HeldItem = null;

        time = Round.Time;
        completedOrders = 0;
        isRunning = true;
        RoundStartEvent.Invoke(Round, roundNumber);
    }

    /// <summary>
    /// Stops the round and invokes <see cref="RoundEndEvent"/>.
    /// </summary>
    public void EndRound()
    {
        isRunning = false;
        RoundEndEvent.Invoke(Round, roundNumber);
    }

    /// <summary>
    /// Increments the round number and invokes <see cref="RoundNextEvent"/>.
    /// </summary>
    /// <exception cref="Exception">The current round is the last round.</exception>
    public void NextRound()
    {
        if (roundNumber == Rounds.Length)
            throw new Exception("Cannot continue past the last round.");

        roundNumber += 1;
        RoundNextEvent.Invoke(Round, roundNumber);
    }

    /// <summary>
    /// Increments the number of completed orders and end the round if it exceeds
    /// <see cref="Round.OrderCount"/>.
    /// </summary>
    /// <param name="order">The completed <see cref="Order"/>.</param>
    public void OrderCompleteEventHandler(Order order)
    {
        completedOrders += 1;

        if (completedOrders == Round.OrderCount) {
            EndRound();
        }
    }
}

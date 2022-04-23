using System;

using ScriptableObjects;

using UnityEngine;
using UnityEngine.Events;

using Utils;

public class RoundManager : Singleton<RoundManager>
{
    public Inventory Inventory;

    [Tooltip("Rounds that will be played (in the listed order)")]
    public Round[] Rounds;

    [Header("Events")]
    public UnityEvent<Round, uint, uint> RoundStartEvent;
    public UnityEvent<Round, uint> RoundEndEvent;
    public UnityEvent<Round, uint> RoundNextEvent;

    public Round Round => Rounds[RoundNumber - 1];

    public uint RoundNumber { get; private set; } = 1;

    public float Time { get; private set; } = 0;

    public uint OrdersCompleted { get; private set; } = 0;

    public bool Started { get; private set; } = false;

    private void Start()
    {
        // TODO: for testing purposes; remove once hooked up to start menu
        StartRound();
    }

    private void Update()
    {
        if (GameState.Instance.GamePaused || !Started)
            return;

        Time -= UnityEngine.Time.deltaTime;

        if (Time <= 0) {
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

        Time = Round.Time;
        OrdersCompleted = 0;
        Started = true;
        RoundStartEvent.Invoke(Round, RoundNumber, (uint) Rounds.Length);
    }

    /// <summary>
    /// Stops the round and invokes <see cref="RoundEndEvent"/>.
    /// </summary>
    public void EndRound()
    {
        Started = false;
        RoundEndEvent.Invoke(Round, RoundNumber);
    }

    /// <summary>
    /// Increments the round number and invokes <see cref="RoundNextEvent"/>.
    /// </summary>
    /// <exception cref="Exception">The current round is the last round.</exception>
    public void NextRound()
    {
        if (RoundNumber == Rounds.Length)
            throw new Exception("Cannot continue past the last round.");

        RoundNumber += 1;
        RoundNextEvent.Invoke(Round, RoundNumber);
    }

    /// <summary>
    /// Increments the number of completed orders and end the round if it exceeds
    /// <see cref="Round.OrderCount"/>.
    /// </summary>
    /// <param name="order">The completed <see cref="Order"/>.</param>
    public void OrderCompleteEventHandler(Order order)
    {
        OrdersCompleted += 1;

        if (OrdersCompleted == Round.OrderCount) {
            EndRound();
        }
    }
}

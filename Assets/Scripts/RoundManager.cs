using System;
using System.Collections.Generic;

using ScriptableObjects;

using UnityEngine;
using UnityEngine.Events;

using Utils;

public enum RoundEndReason
{
    RoundWon,
    GameWon,
    TimedOut,
    Restarted,
    Quit
}

public class RoundManager : Singleton<RoundManager>
{
    [SerializeField]
    private Inventory inventory;

    [Tooltip("Rounds that will be played (in the listed order)")]
    public Round[] Rounds;

    [Header("Events")]
    public UnityEvent<Round, uint, uint> RoundStartEvent;
    public UnityEvent<Round, uint, RoundEndReason> RoundEndEvent;
    public UnityEvent<Round, uint> RoundNextEvent;

    public Round Round => Rounds[RoundNumber - 1];

    public uint RoundNumber { get; private set; } = 1;

    public float Time { get; private set; } = 0;

    public float Money { get; private set; } = 0;

    public uint OrdersCompleted { get; private set; } = 0;

    public bool Started { get; private set; } = false;

    private void Update()
    {
        if (GameState.Instance.GamePaused || !Started)
            return;

        Time -= UnityEngine.Time.deltaTime;

        if (Time <= 0) {
            EndRound(RoundEndReason.TimedOut);
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
        inventory.HeldItem = null;

        Time = Round.Time;
        OrdersCompleted = 0;
        Started = true;
        RoundStartEvent.Invoke(Round, RoundNumber, (uint) Rounds.Length);

        // TODO: reload the scene somehow
        // Or otherwise somehow reset all containers and spawned items.

        GameState.Instance.Resume();
    }

    /// <summary>
    /// Stops the round and invokes <see cref="RoundEndEvent"/>.
    /// </summary>
    /// <param name="reason">The reason for the round ending.</param>
    public void EndRound(RoundEndReason reason)
    {
        Started = false;
        RoundEndEvent.Invoke(Round, RoundNumber, reason);

        GameState.Instance.Pause();
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
    public void OrderCompleteEventHandler(LinkedListNode<Order> order)
    {
        OrdersCompleted += 1;
        Money += order.Value.Reward;

        // Check if the final order was completed.
        if (order.List.Count == 1 && order.Value.ID >= Round.OrderCount) {
            EndRound(
                RoundNumber == Rounds.Length ? RoundEndReason.GameWon : RoundEndReason.RoundWon
            );
        }
    }

    public void RestartRoundEventHandler() { }
}

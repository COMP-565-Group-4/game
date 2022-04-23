using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Round", menuName = "ScriptableObjects/Round", order = 0)]
    public class Round : ScriptableObject
    {
        [Tooltip("Amount of seconds the player has to complete this round")]
        public uint Time;

        [Tooltip("Possible orders to give during this round")]
        public Order[] Orders;

        [Tooltip("Amount of orders to give during this round")]
        public uint OrderCount;

        [Tooltip("Maximum amount of orders available concurrently")]
        public uint MaxConcurrentOrders;

        [Tooltip("Amount of seconds to wait before giving a new order")]
        public uint OrderFrequency;
    }
}

using UnityEngine;
using UnityEngine.Events;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Order", menuName = "ScriptableObjects/Order")]
    public class Order : ScriptableObject
    {
        [Tooltip("Item that needs to be turned in to complete the order")]
        public Ingredient Meal;

        [Tooltip("Monetary reward for completing the order")]
        public uint Reward;

        [Tooltip("Amount of seconds the player is granted to complete the order")]
        public uint Time;
    }
}

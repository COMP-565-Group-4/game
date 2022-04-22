using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData")]
    public class PlayerData : ScriptableObject
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip(
            "Time required to pass before being able to jump again. Set to 0f to instantly jump again"
        )]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs"
        )]
        public float FallTimeout = 0.15f;

        [Tooltip(
            "Maximum distance from which the player can hover their mouse over an interactable object"
        )]
        public float HoverDistance = 2;

        [Header("Player Items")]
        [Tooltip("Whether the player can swap items.")]
        public bool SwapItems = true;

        [Header("Player Grounded")]
        [Tooltip(
            "If the character is grounded or not. Not part of the CharacterController built in grounded check"
        )]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController"
        )]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
    }
}

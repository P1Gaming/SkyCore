using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Motion
{
    /// <summary>
    /// Settings for player movement.
    /// </summary>
    [System.Serializable] // to show in inspector
    public class PlayerMovementSettings
    {
        [Header("Running")]
        [SerializeField, Tooltip("Maximum speed, ignoring the vertical component")]
        private float _maxSpeed = 1f;
        [SerializeField, Tooltip("The settings while the player is touching the ground")]
        private PlayerHorizontalMovementSettings _groundedHorizontalMovementSettings;
        [SerializeField, Tooltip("The settings for horizontal movement while the player isn't touching the ground")]
        private PlayerHorizontalMovementSettings _nonGroundedHorizontalMovementSettings;

        [Header("Jumping")]
        [SerializeField, Tooltip("How high a jump will reach")]
        private float _jumpHeight = 1f;
        [SerializeField, Tooltip("How many seconds for a jump to reach its full height")]
        private float _jumpUpwardsTime = 1f;
        [SerializeField, Tooltip("How many seconds for a jump to go from full height to back down," +
            " disregarding drag (which is for terminal velocity)")]
        private float _jumpDownwardsTime = 1f;
        [SerializeField, Tooltip("The maximum speed in the vertical downwards direction")]
        private float _terminalVelocity = float.PositiveInfinity;
        [SerializeField, Tooltip("The drag force while falling is proportional to fall speed to this power")]
        private float _fallDragProportionalityExponent = 2f;
        [SerializeField, Range(0, 1), Tooltip("While falling, drag makes the player stop accelerating at Terminal Velocity." +
            " The force is multiplied by this, but fall speed is still capped to terminal velocity.")]
        private float _fallDragForceMultiplier = 1f;
        [SerializeField, Tooltip("How long after running off an edge the player can still jump")]
        private float _coyoteTime = 0f;
        [SerializeField, Tooltip("How long a jump while non-grounded is remembered, to be executed upon reaching the ground")]
        private float _jumpBufferTime = 0f;
        [SerializeField, Tooltip("Whether to use the gravity based on Jump Downwards Time, rather than the gravity" +
            " based on Jump Upwards Time, after walking off an edge.")]
        private bool _sameGravityWhenFallOffEdgeAsWhenFallDuringJump;

        // Properties for horizontal movement
        public float MaxHorizontalSpeed => _maxSpeed;
        public PlayerHorizontalMovementSettings GroundedHorizontalMovementSettings 
            => _groundedHorizontalMovementSettings;
        public PlayerHorizontalMovementSettings NonGroundedHorizontalMovementSettings 
            => _nonGroundedHorizontalMovementSettings;

        // Properties for vertical movement
        public float GravityAccel { get; private set; }
        public float GravityAccelWhileFallingDuringJump { get; private set; }
        public float JumpVelocity { get; private set; }
        public float CoyoteTime => _coyoteTime;
        public float TerminalVelocity => _terminalVelocity;
        public float FallDragProportionalityExponent => _fallDragProportionalityExponent;
        public float FallDragForceConstant { get; private set; }
        public float JumpBufferTime => _jumpBufferTime;
        public bool SameGravityWhenFallOffEdgeAsWhenFallDuringJump => _sameGravityWhenFallOffEdgeAsWhenFallDuringJump;

        public void Initialize()
        {
            _groundedHorizontalMovementSettings.Initialize(_maxSpeed);
            _nonGroundedHorizontalMovementSettings.Initialize(_maxSpeed);

            // jump velocity = sqrt(height * 2 g) = _jumpUpwardsTime * g, where g is gravity acceleration
            GravityAccel = 2 * _jumpHeight / (_jumpUpwardsTime * _jumpUpwardsTime);
            GravityAccelWhileFallingDuringJump = 2 * _jumpHeight / (_jumpDownwardsTime * _jumpDownwardsTime);

            JumpVelocity = _jumpUpwardsTime * GravityAccel;

            // At terminal velocity, gravity accel = drag accel = constant * velocity ^ _fallDragProportionalityExponent
            // GravityAccel varies based on whether jumping or just walked off an edge, so will multiply the drag
            // accel by gravity accel.
            FallDragForceConstant = 1f / Mathf.Pow(_terminalVelocity, _fallDragProportionalityExponent);
            FallDragForceConstant *= _fallDragForceMultiplier;
        }
    }
}
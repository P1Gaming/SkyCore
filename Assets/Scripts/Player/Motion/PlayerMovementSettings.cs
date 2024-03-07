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
        public const float _fallDragProportionalityExponent = 2f;

        public const float _slopeDegreesForNoGravity = 5f;
        public const float _slopeDegreesToNotBeFalling = 90f; // 90 means the player would be touching a vertical cliff
        public const float _slopeDegreesToJump = 60f;

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
        [SerializeField, Tooltip("How long after running off an edge the player can still jump")]
        private float _coyoteTime = 0f;
        [SerializeField, Tooltip("How long a jump while non-grounded is remembered, to be executed upon reaching the ground")]
        private float _jumpBufferTime = 0f;

        // Properties for horizontal movement
        public float MaxHorizontalSpeed => _maxSpeed;
        public PlayerHorizontalMovementSettings GroundedHorizontalMovementSettings 
            => _groundedHorizontalMovementSettings;
        public PlayerHorizontalMovementSettings NonGroundedHorizontalMovementSettings 
            => _nonGroundedHorizontalMovementSettings;

        // Properties for vertical movement
        public float GravityAccelWhileJumpingUp { get; private set; }
        public float GravityAccel { get; private set; }
        public float JumpVelocity { get; private set; }
        public float CoyoteTime => _coyoteTime;
        public float TerminalVelocity => _terminalVelocity;
        public float FallDragForceConstant { get; private set; }
        public float DownwardsVelocityAtEndOfJump { get; private set; }
        public float JumpBufferTime => _jumpBufferTime;

        public void Initialize()
        {
            _groundedHorizontalMovementSettings.Initialize(_maxSpeed);
            _nonGroundedHorizontalMovementSettings.Initialize(_maxSpeed);

            // jump velocity = sqrt(height * 2 g) = _jumpUpwardsTime * g, where g is gravity acceleration
            GravityAccelWhileJumpingUp = 2 * _jumpHeight / (_jumpUpwardsTime * _jumpUpwardsTime);
            GravityAccel = 2 * _jumpHeight / (_jumpDownwardsTime * _jumpDownwardsTime);

            JumpVelocity = _jumpUpwardsTime * GravityAccelWhileJumpingUp;

            
            DownwardsVelocityAtEndOfJump = _jumpDownwardsTime * GravityAccel;
            if (DownwardsVelocityAtEndOfJump >= _terminalVelocity)
            {
                throw new System.Exception("Invalid settings. Make the terminal velocity larger than " + DownwardsVelocityAtEndOfJump);
            }

            // At terminal velocity, gravity accel = drag accel = constant * velocity ^ _fallDragProportionalityExponent
            // Also, no drag is applied until the fall speed is higher than the speed at the end of a jump, to avoid affecting
            // that. So the terminal velocity is adjusted.
            FallDragForceConstant = GravityAccel 
                / Mathf.Pow(_terminalVelocity - DownwardsVelocityAtEndOfJump, _fallDragProportionalityExponent);
        }
    }
}
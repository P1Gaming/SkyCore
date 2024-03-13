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

        // The "slope" is really the angle between the vertical and whatever surface the player is touching.
        // So if the player is touching a ceiling or cliff, still use settings to make it feel more floaty.
        // A setting of 90 means the player would be touching a vertical cliff.
        public const float SLOPE_DEGREES_TO_BE_CONSIDERED_MIDAIR = 90f;

        // No gravity if the player is standing on a gentle slope. When there's gravity, the player slowly
        // slides down slopes because there's no friction / drag. It's very slow because PlayerMovement makes
        // the player's horizontal velocity 0 each FixedUpdate but not zero because the physics engine updates after.
        public const float SLOPE_DEGREES_TO_NOT_SLIDE_DOWN = 50f;

        public const float SLOPE_DEGREES_TO_JUMP = 60f;

        [Header("Running")]
        [SerializeField, Tooltip("Maximum speed, ignoring the vertical component")]
        private float _maxSpeed = 1f;
        [SerializeField, Tooltip("The settings for horizontal movement while the player is touching the ground")]
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

        /// <summary>
        /// Do math to convert the inspector settings into settings which are easier to use in code.
        /// </summary>
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
            // that. So the terminal velocity is adjusted in this.
            FallDragForceConstant = GravityAccel / Mathf.Pow(_terminalVelocity - DownwardsVelocityAtEndOfJump, 2f);
        }

        public float DecideGravityAcceleration(float verticalVelocity, bool jumping)
        {
            if (verticalVelocity > 0 && jumping)
            {
                // Platformers have different gravity depending on whether the jump is rising or falling
                // as a way to control how it feels.
                return GravityAccelWhileJumpingUp;
            }
            else
            {
                return GravityAccel;
            }
        }

        public float DecideHorizontalAccelerationMagnitude(float slopeAngle, Vector2 targetMovementDirection)
        {
            bool falling = slopeAngle < SLOPE_DEGREES_TO_BE_CONSIDERED_MIDAIR;
            bool tryingToStop = targetMovementDirection.magnitude == 0;

            PlayerHorizontalMovementSettings settings;
            if (falling)
            {
                settings = GroundedHorizontalMovementSettings;
            }
            else
            {
                settings = NonGroundedHorizontalMovementSettings;
            }

            if (tryingToStop)
            {
                return settings.AccelToStop;
            }
            else
            {
                return settings.AccelToSpeedUp;
            }
        }
    }
}
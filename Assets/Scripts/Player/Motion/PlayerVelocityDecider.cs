using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Motion
{
    /// <summary>
    /// Decides the player's velocity each frame.
    /// </summary>
    public class PlayerVelocityDecider
    {
        private PlayerMovementSettings _settings;
        private bool _jumping;
        private bool _wasGrounded;
        private float _jumpBufferTimeLeft;
        private float _coyoteTimeLeft;

        public PlayerVelocityDecider(PlayerMovementSettings settings)
        {
            _settings = settings;
        }

        #region Update Horizontal Velocity Components (x and z)
        /// <summary>
        /// Applies acceleration to the velocity for running.
        /// </summary>
        /// <param name="grounded">Whether the player is on the ground.</param>
        /// <param name="targetDirection">The direction the player wants to run in, or a zero vector to not run.
        /// The new direction will either be in this direction, or opposite it.</param>
        /// <param name="currentVelocity">The player's horizontal velocity (x-z plane) before updating it.</param>
        /// <returns>The player's new velocity this frame. Only the horizontal part (x and z).</returns>
        private Vector2 UpdateHorizontalVelocity(bool grounded, Vector2 targetDirection, Vector2 currentVelocity)
        {
            targetDirection.Normalize();

            PlayerHorizontalMovementSettings horizontalSettings = grounded ? _settings.GroundedHorizontalMovementSettings
                : _settings.NonGroundedHorizontalMovementSettings;

            Vector2 result = currentVelocity;

            if (targetDirection == Vector2.zero)
            {
                // Trying to stop moving.
                result -= Time.deltaTime * horizontalSettings.AccelToStop * currentVelocity.normalized;

                // Dot product < 0 means they're over 90 degrees apart. In this case, they'll be
                // either in the same direction or pointing opposite direction, but float imprecision
                // means that's not quite true so check for that like this.
                bool overshotAndChangedDirection = Vector2.Dot(result, currentVelocity) < 0;
                if (overshotAndChangedDirection)
                {
                    result = Vector2.zero;
                }
            }
            else
            {
                // maybe this should be based on the local target direction (the one directly from input,
                // before it is rotated with the transform), so it doesn't happen if you flick the mouse.
                bool turning = Vector2.Angle(currentVelocity, targetDirection) > horizontalSettings.TurnMinDegrees;

                float accelTowardsTargetDirection;
                if (turning)
                {
                    accelTowardsTargetDirection = horizontalSettings.AccelToTurn;
                }
                else
                {
                    accelTowardsTargetDirection = horizontalSettings.AccelToSpeedUp;
                }

                // The point of this redirection stuff is so when the player changes direction, they slow
                // down, sort of like keeping their turning radius low as the mouse moves.

                // The player's velocity will always be pointing in the target direction (the camera direction,
                // if they're just pressing w). When the target direction changes, it hasn't yet updated to be
                // in that direction, so it's aiming in a somewhat different direction.

                // 1. Determine the non-updated current velocity's component along the target direction.

                Vector2 perpendicularDirection = Vector2.Perpendicular(targetDirection);
                if (Vector2.Dot(perpendicularDirection, currentVelocity) < 0f)
                {
                    perpendicularDirection = -perpendicularDirection;
                }

                Vector2 velocityInPerpendicularDirection = Vector2.Dot(currentVelocity, perpendicularDirection) * perpendicularDirection;
                Vector2 velocityInTargetDirection = currentVelocity - velocityInPerpendicularDirection;

                float speedInTargetDirection = Vector2.Dot(velocityInTargetDirection, targetDirection);

                // 2. If redirection setting is 0, the speed is only the component from step 1. If the setting is 1, the speed
                // doesn't get reduced at all. Interpolate between those if the setting is something else.

                float redirection = turning ? horizontalSettings.RedirectionWhenTurning : horizontalSettings.Redirection;
                float newSpeed = Mathf.Lerp(speedInTargetDirection, currentVelocity.magnitude, redirection);

                newSpeed += Time.deltaTime * accelTowardsTargetDirection;
                result = targetDirection * newSpeed;
            }

            if (result.magnitude > _settings.MaxHorizontalSpeed)
            {
                result = result.normalized * _settings.MaxHorizontalSpeed;
            }

            return result;
        }
        #endregion

        #region Update Vertical Velocity Component (y)
        /// <summary>
        /// Applies acceleration to the vertical velocity for gravity, and can suddenyl change it to jump.
        /// </summary>
        /// <returns>The new vertical velocity for this frame.</returns>
        private float UpdateVerticalVelocityAndMaybeJump(bool grounded, float currentVerticalVelocity, bool tryJump
            , out bool jumped)
        {
            currentVerticalVelocity = UpdateVerticalVelocity(grounded, currentVerticalVelocity, out jumped);
            if (tryJump)
            {
                currentVerticalVelocity = TryJumpAndGetNewVerticalVelocity(grounded, currentVerticalVelocity
                    , out bool normalJump);
                jumped |= normalJump;
            }
            return Mathf.Max(-_settings.TerminalVelocity, currentVerticalVelocity);
        }

        /// <summary>
        /// Applies acceleration to the velocity for falling, and can suddenly change velocity for buffered jump.
        /// </summary>
        /// <param name="grounded">Whether the player is on the ground.</param>
        /// <param name="currentVerticalVelocity">The player's vertical velocity (y axis) before updating it.</param>
        /// <param name="executedBufferedJump">Whether it executed a buffered jump.</param>
        /// <returns>The new vertical velocity.</returns>
        private float UpdateVerticalVelocity(bool grounded, float currentVerticalVelocity, out bool executedBufferedJump)
        {
            executedBufferedJump = false;
            _jumpBufferTimeLeft = Mathf.Max(0f, _jumpBufferTimeLeft - Time.deltaTime);
            _coyoteTimeLeft = Mathf.Max(0f, _coyoteTimeLeft - Time.deltaTime);

            if (_wasGrounded && !grounded && !_jumping)
            {
                _coyoteTimeLeft = _settings.CoyoteTime;
            }
            _wasGrounded = grounded;

            // The 2nd thing in this || probably isn't necessary. Maybe in weird cases involving a roof very close
            // over the player's head, dunno.
            if ((grounded || _coyoteTimeLeft > 0) && _jumpBufferTimeLeft > 0 && !_jumping)
            {
                // Execute a buffered jump.
                return TryJumpAndGetNewVerticalVelocity(grounded, currentVerticalVelocity, out executedBufferedJump);
            }

            if (grounded && currentVerticalVelocity <= 0)
            {
                // Need the 2nd part of the if statement for when jump and move forwards onto a
                // higher block, so it continues moving upwards rather than sticking to the ground.
                _jumping = false;
                return 0f;
            }

            float gravityAccel = _settings.GravityAccel;
            if (currentVerticalVelocity < 0 && (_jumping || _settings.SameGravityWhenFallOffEdgeAsWhenFallDuringJump))
            {
                gravityAccel = _settings.GravityAccelWhileFallingDuringJump;
            }

            float accelDownwards = gravityAccel;

            if (currentVerticalVelocity < 0)
            {
                // Multiply by gravityAccel here because the formula for the drag is proportional to gravityAccel,
                // and that wasn't included in _settings.FallDragForceConstant 
                float dragAccel = gravityAccel * _settings.FallDragForceConstant
                    * Mathf.Pow(currentVerticalVelocity, _settings.FallDragProportionalityExponent);
                accelDownwards -= dragAccel;
                accelDownwards = Mathf.Max(0, accelDownwards); // this probably isnt necessary
            }

            return currentVerticalVelocity - (accelDownwards * Time.deltaTime);
        }

        /// <summary>
        /// Tries to execute a jump and returns the new vertical velocity.
        /// </summary>
        /// <param name="grounded">Whether the player is on the ground.</param>
        /// <param name="currentVerticalVelocity">The player's vertical velocity (y axis) before updating it for jumping.</param>
        /// <param name="jumped">Whether it executed a jump.</param>
        /// <returns>The new vertical velocity.</returns>
        private float TryJumpAndGetNewVerticalVelocity(bool grounded, float currentVerticalVelocity, out bool jumped)
        {
            jumped = (grounded || _coyoteTimeLeft > 0) && !_jumping;
            if (jumped)
            {
                _jumping = true;
                return _settings.JumpVelocity;
            }
            else
            {
                _jumpBufferTimeLeft = _settings.JumpBufferTime;
                return currentVerticalVelocity;
            }
        }
        #endregion

        /// <summary>
        /// Updates the player's velocity this frame.
        /// </summary>
        /// <param name="grounded">Whether the player is on the ground.</param>
        /// <param name="horizontalDirection">The direction the player will move, ignoring vertical. E.g. WASD input.</param>
        /// <param name="currentVelocity">The player's velocity before updating it.</param>
        /// <param name="tryJump">Whether to try to jump.</param>
        /// <param name="jumped">Whether the player jumps this frame. Can be true even if tryJump is false.</param>
        /// <returns>The player's new velocity this frame.</returns>
        public Vector3 UpdateVelocity(bool grounded, Vector2 horizontalDirection, Vector3 currentVelocity, bool tryJump
            , out bool jumped)
        {
            Vector2 currentHorizontalVelocity = new Vector2(currentVelocity.x, currentVelocity.z);
            float currentVerticalVelocity = currentVelocity.y;

            Vector2 newHorizontalVelocity = UpdateHorizontalVelocity(grounded
                , horizontalDirection, currentHorizontalVelocity);

            float newVerticalVelocity = UpdateVerticalVelocityAndMaybeJump(grounded
                , currentVerticalVelocity, tryJump, out jumped);

            return new Vector3(newHorizontalVelocity.x, newVerticalVelocity, newHorizontalVelocity.y);
        }
    }
}
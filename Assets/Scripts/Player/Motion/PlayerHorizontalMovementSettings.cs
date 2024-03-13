using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Motion
{
    /// <summary>
    /// Settings for player movement in the horizontal direction. There's one of these for when
    /// grounded and one for when not grounded.
    /// </summary>
    [System.Serializable] // to show in inspector
    public class PlayerHorizontalMovementSettings
    {
        [SerializeField, Tooltip("How long it takes to accelerate to max speed from no movement.")]
        private float _accelTime = 1f;

        [SerializeField, Tooltip("How many seconds to decelerate after moving at max speed.")]
        private float _stopTime = 1f;

        private float _maxSpeed;

        public float AccelToSpeedUp => _maxSpeed / _accelTime;
        public float AccelToStop => _maxSpeed / _stopTime;

        public void Initialize(float maxSpeed)
        {
            _maxSpeed = maxSpeed;
        }
    }
}
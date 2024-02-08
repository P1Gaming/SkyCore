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

        [SerializeField, Tooltip("How long it takes to change direction when doing a sudden turn (more than Turn Min Degrees" +
            " in a single frame, e.g. press w then s).")]
        private float _turnTime = 1f;

        [SerializeField, Tooltip("The minimum angle between prior and new movement direction to be considered turning.")]
        private float _turnMinDegrees = 120f;

        [SerializeField, Range(0, 1), Tooltip("0 means keep none of the prior velocity component which is perpendicular" +
            " to the new movement direction. 1 means keep the same speed when turning.")]
        private float _redirection = 1f;

        [SerializeField, Range(0, 1), Tooltip("Same as redirection, but for when turning more than Turn Min Degrees.")]
        private float _redirectionWhenTurning = 1f;

        private float _maxSpeed;

        public float AccelToSpeedUp => _maxSpeed / _accelTime;
        public float AccelToStop => _maxSpeed / _stopTime;
        public float AccelToTurn => _maxSpeed / _turnTime;
        public float TurnMinDegrees => _turnMinDegrees;
        public float Redirection => _redirection;
        public float RedirectionWhenTurning => _redirectionWhenTurning;

        public void Initialize(float maxSpeed)
        {
            _maxSpeed = maxSpeed;
        }
    }
}
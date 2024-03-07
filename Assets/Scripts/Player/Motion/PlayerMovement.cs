using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Motion
{
    /// <summary>
    /// Controls the players movement.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private PlayerMovementSettings _settings;
        [SerializeField]
        private LayerMask _groundLayer;
        [SerializeField] 
        private GameEventScriptableObject _playerMovementW;
        [SerializeField]
        private GameEventScriptableObject _playerMovementA;
        [SerializeField]
        private GameEventScriptableObject _playerMovementS;
        [SerializeField]
        private GameEventScriptableObject _playerMovementD;

        private Rigidbody _rigidbody;

        private float _groundingSurfaceSlope;
        private ContactPoint[] _collisionContacts = new ContactPoint[100];

        public bool GroundedEnoughToJump { get; private set; }
        public bool GroundedEnoughForNoGravity { get; private set; }
        public bool GroundedEnoughForNormalAccelerationSettings { get; private set; }


        public static PlayerMovement Instance { get; private set; }

        public PlayerMovementSettings Settings => _settings;

        public bool StandingOnGroundLayer => Physics.Raycast(transform.position + Vector3.up, Vector3.down, 1f, _groundLayer);

        public bool IsMoving => _rigidbody.velocity.magnitude > .001f;

        private float _timeSinceGroundedEnoughToJump = Mathf.Infinity;

        private float _jumpInputTime = float.NegativeInfinity;
        private bool _fixedUpdateRanThisFrame;
        private bool _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun;

        private bool _jumping;
        private bool _jumpedInPriorFixedUpdate;
        private int _frameWhenGotJumpInputInFixedUpdate = -1;

        private PlayerHorizontalMovementSettings _currentHorizontalMovementSettings;

        private Vector2 HorizontalVelocity 
        { 
            get => new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z); 
            set => _rigidbody.velocity = new Vector3(value.x, _rigidbody.velocity.y, value.y); 
        }
        private float VerticalVelocity 
        { 
            get => _rigidbody.velocity.y; 
            set => _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, value, _rigidbody.velocity.z); 
        }

        private void Awake()
        {
            _settings.Initialize();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.sleepThreshold = -1; // don't sleep
            _currentHorizontalMovementSettings = _settings.GroundedHorizontalMovementSettings;
            Instance = this;
        }

        private void FixedUpdate()
        {
            GroundedDetection();
            UpdateHorizontalVelocity();
            UpdateVerticalVelocity();
            _fixedUpdateRanThisFrame = true;
            _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun = false;
        }

        private void OnCollisionEnter(Collision collision) => CheckGrounded(collision);
        private void OnCollisionStay(Collision collision) => CheckGrounded(collision);

        private void Update()
        {
            if (!_fixedUpdateRanThisFrame && GetJumpInput())
            {
                _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun = true;
            }
            CheckRaiseWASDEvents();
            _fixedUpdateRanThisFrame = false;
        }
       

        private void CheckGrounded(Collision collision)
        {
            int numContacts = collision.GetContacts(_collisionContacts);
            for (int i = 0; i < numContacts; i++)
            {
                float angle = Vector3.Angle(_collisionContacts[i].normal, Vector3.up);
                _groundingSurfaceSlope = Mathf.Min(_groundingSurfaceSlope, angle);
            }
        }

        /// <summary>
        /// This moves the player, but does not determine which direction the player goes.
        /// </summary>
        private void UpdateHorizontalVelocity()
        {
            //Move Speed Power

            GetWASDInputAxes(out float rightLeft, out float forwardsBackwards);
            Vector3 targetMoveDirection = transform.forward * forwardsBackwards + transform.right * rightLeft;

            Vector2 targetVelocity = _settings.MaxHorizontalSpeed * new Vector2(targetMoveDirection.x, targetMoveDirection.z);
            Vector2 currentVelocity = HorizontalVelocity;
            Vector2 velocityChangeDirection = (targetVelocity - currentVelocity).normalized;

            float accel = targetMoveDirection.magnitude != 0 ? _currentHorizontalMovementSettings.AccelToSpeedUp
                : _currentHorizontalMovementSettings.AccelToStop;

            Vector2 newVelocity = currentVelocity + Time.deltaTime * accel * velocityChangeDirection;
            if (newVelocity.magnitude > _settings.MaxHorizontalSpeed)
            {
                newVelocity = newVelocity.normalized * _settings.MaxHorizontalSpeed;
            }

            Vector2 currentVelocityError = currentVelocity - targetVelocity;
            Vector2 newVelocityError = newVelocity - targetVelocity;
            if (Vector2.Dot(currentVelocityError, newVelocityError) < 0)
            {
                // It overshot the target velocity, so do this to avoid jittering around targetVelocity.
                newVelocity = targetVelocity;
            }

            HorizontalVelocity = newVelocity;
        }

        /// <summary>
        /// If the player gets the jump input, add upwards force.
        /// </summary>
        private void UpdateVerticalVelocity()
        {
            float gravityAccel = _settings.GravityAccel;

            if (_rigidbody.velocity.y > 0 && _jumping)
            {
                gravityAccel = _settings.GravityAccelWhileJumpingUp;
            }

            bool getJumpInputFirstTimeThisFrame = GetJumpInput() && Time.frameCount != _frameWhenGotJumpInputInFixedUpdate;

            if (getJumpInputFirstTimeThisFrame || _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun)
            {
                _frameWhenGotJumpInputInFixedUpdate = Time.frameCount;
                _jumpInputTime = Time.time;
            }

            bool canJump = _timeSinceGroundedEnoughToJump < _settings.CoyoteTime && !_jumping;
            bool recentJumpInput = Time.time <= _jumpInputTime + _settings.JumpBufferTime;
            if (canJump && recentJumpInput)
            {
                // Jump Power
                VerticalVelocity = _settings.JumpVelocity;
                _jumpInputTime = float.NegativeInfinity;
                _timeSinceGroundedEnoughToJump = float.PositiveInfinity;
                _jumping = true;
                _jumpedInPriorFixedUpdate = true;
            }
            else if (!GroundedEnoughForNoGravity)
            {
                // Gravity Power and drag for terminal velocity
                float totalAccel = -gravityAccel;

                if (VerticalVelocity < -_settings.DownwardsVelocityAtEndOfJump)
                {
                    // E.g. if it falls at 2m/s at the end of a jump on a flat surface, and it's falling at 3m/s,
                    // then this would equal 1.
                    float fallSpeedBeyondThreshold = -VerticalVelocity - _settings.DownwardsVelocityAtEndOfJump;

                    float dragAccel = _settings.FallDragForceConstant
                        * Mathf.Pow(fallSpeedBeyondThreshold, PlayerMovementSettings._fallDragProportionalityExponent);
                    totalAccel += dragAccel;
                }

                VerticalVelocity += totalAccel * Time.deltaTime;
            }
        }

        /// <summary>
        /// Checks if the player is falling.
        /// </summary>
        private void GroundedDetection()
        {
            if (_jumpedInPriorFixedUpdate)
            {
                // For some reason, in this case it incorrectly thinks it's grounded. Maybe the OnCollisionStay is called
                // before the velocity moves it.

                // Without doing this, with a jump height setting of 2.25, it jumps 2.34 units (with monitor refresh rate of 60.
                // It'd be closer to 2.25 with a faster monitor because we set Time.fixedDeltaTime based on that.)

                // With this check, it jumps 2.28 units, which is probably small enough error to be from physics innaccuracy.

                GroundedEnoughForNoGravity = false;
                GroundedEnoughForNormalAccelerationSettings = false;
                GroundedEnoughToJump = false;
            }
            else
            {
                GroundedEnoughForNoGravity = _groundingSurfaceSlope < PlayerMovementSettings._slopeDegreesForNoGravity;
                GroundedEnoughForNormalAccelerationSettings = _groundingSurfaceSlope < PlayerMovementSettings._slopeDegreesToNotBeFalling;
                GroundedEnoughToJump = _groundingSurfaceSlope < PlayerMovementSettings._slopeDegreesToJump;
            }
            _jumpedInPriorFixedUpdate = false;

            // Do this so will get the minimum slope angle when the physics engine updates after FixedUpdate runs.
            _groundingSurfaceSlope = float.PositiveInfinity; 

            if (GroundedEnoughToJump)
            {
                _timeSinceGroundedEnoughToJump = 0.0f;
                _jumping = false;
            }
            else
            {
                _timeSinceGroundedEnoughToJump += Time.deltaTime;
            }

            _currentHorizontalMovementSettings = GroundedEnoughForNormalAccelerationSettings
                ? _settings.GroundedHorizontalMovementSettings : _settings.NonGroundedHorizontalMovementSettings;
        }

        private void GetWASDInputAxes(out float rightLeft, out float forwardsBackwards)
        {
            rightLeft = Input.GetAxisRaw("Horizontal");
            forwardsBackwards = Input.GetAxisRaw("Vertical");
        }

        private bool GetJumpInput()
        {
            return Input.GetButtonDown("Jump");
        }

        private void CheckRaiseWASDEvents()
        {
            GetWASDInputAxes(out float rightLeft, out float forwardsBackwards);
            if (rightLeft < 0)
            {
                _playerMovementA.Raise();
            }
            if (rightLeft > 0)
            {
                _playerMovementD.Raise();
            }
            if (forwardsBackwards > 0)
            {
                _playerMovementW.Raise();
            }
            if (forwardsBackwards < 0)
            {
                _playerMovementS.Raise();
            }
        }


        /// <summary>
        /// Moves the player to the position.
        /// </summary>
        public void Teleport(Vector3 position)
        {
            _rigidbody.position = position;
            transform.position = position;
        }



        /// <summary>
        /// Sets Interacting state and toggles enabled
        /// </summary>
        /// <param name="isInteracting"></param>
        public void SetInteract(bool isInteracting)
        {
            // We might need a combined way for disabling this script, since it'll also
            // be disabled during inventory and will be able to open inventory while interacting.
            // E.g. an int for the number of reasons to disable this script.
            enabled = !isInteracting;
            _rigidbody.isKinematic = isInteracting;
        }
    }
}
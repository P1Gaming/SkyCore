using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Motion
{
    /// <summary>
    /// Controls the player's movement.
    /// </summary>
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private PlayerMovementSettings _settings;
        [SerializeField]
        private LayerMask _groundLayer;

        [Header("Player Input Actions")]
        [SerializeField]
        private InputActionReference _moveAction;
        [SerializeField]
        private InputActionReference _jumpAction;

        [SerializeField] 
        private GameEventScriptableObject _playerMovementW;
        [SerializeField]
        private GameEventScriptableObject _playerMovementA;
        [SerializeField]
        private GameEventScriptableObject _playerMovementS;
        [SerializeField]
        private GameEventScriptableObject _playerMovementD;

        private Rigidbody _rigidbody;

        private float _timeSinceStoodOnJumpableSurface = float.PositiveInfinity;

        private float _jumpInputTime = float.NegativeInfinity;
        private bool _fixedUpdateRanThisFrame;
        private bool _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun;
        private int _frameWhenGotJumpInputInFixedUpdate = -1;

        private bool _jumping;
        private bool _startedJumpingInMostRecentFixedUpdate;

        private float _angleOfContactedSurface;
        private int _layerOfContactedSurface;
        private ContactPoint[] _collisionContacts = new ContactPoint[100];

        // Directly control the rigidbody velocity rather than using AddForce. These properties are to make that easier.
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

        public bool StandingOnGroundLayer => _angleOfContactedSurface < PlayerMovementSettings.SLOPE_DEGREES_TO_JUMP
            && ((1 << _layerOfContactedSurface) & _groundLayer.value) != 0;

        public bool IsMoving => _rigidbody.velocity.magnitude > .001f;

        private static PlayerMovement _instance;
        public static PlayerMovement Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
                }
                return _instance;
            }
        }

        private int _numberOfReasonsToIgnoreInputs = 0;
        public int NumberOfReasonsToIgnoreInputs
        {
            get => _numberOfReasonsToIgnoreInputs;
            set
            {
                _numberOfReasonsToIgnoreInputs = value;
                if (_numberOfReasonsToIgnoreInputs < 0)
                {
                    throw new System.Exception("In PlayerMovement, _numberOfReasonsToIgnoreInputs < 0: " + _numberOfReasonsToIgnoreInputs);
                }
                if (_numberOfReasonsToIgnoreInputs > 0)
                {
                    // Clear recent inputs which could otherwise cause jumping later.
                    _jumpInputTime = float.NegativeInfinity;
                    _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun = false;
                }
            }
        }
        private bool IgnoreInputs => NumberOfReasonsToIgnoreInputs > 0;


        private void Awake()
        {
            _settings.Initialize();
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.sleepThreshold = -1; // don't sleep
            _instance = this;
        }

        private void OnEnable() => _rigidbody.isKinematic = false;

        private void OnDisable() => _rigidbody.isKinematic = true;

        private void FixedUpdate()
        {
            _fixedUpdateRanThisFrame = true;

            UpdateHorizontalVelocity();
            UpdateVerticalVelocity();

            // The physics engine updates immediately after FixedUpdate, calling OnCollisionStay and updating _groundingSurfaceSlope.
            _angleOfContactedSurface = float.PositiveInfinity; 
        }

        private void Update()
        {
            CheckRaiseWASDEvents();

            // Keyboard input happens each frame, but fixed update isn't guaranteed to run every frame.
            // If it didn't run this frame and the player is trying to jump, cache the jump input and
            // will handle it next time FixedUpdate runs.
            if (!_fixedUpdateRanThisFrame && GetJumpInput())
            {
                _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun = true;
            }
            _fixedUpdateRanThisFrame = false;
        }

        private void OnCollisionEnter(Collision collision) => CheckGrounded(collision);
        private void OnCollisionStay(Collision collision) => CheckGrounded(collision);

        private void CheckGrounded(Collision collision)
        {
            // The player is touching another collider. Check how close that is to
            // standing on a flat surface.

            if (_startedJumpingInMostRecentFixedUpdate)
            {
                // For some reason, it thinks it's still touching the surface in this case.
                // Need to handle this or the jump height will be like .1 units higher than the setting.
                return;
            }

            int numContacts = collision.GetContacts(_collisionContacts);
            for (int i = 0; i < numContacts; i++)
            {
                float angle = Vector3.Angle(_collisionContacts[i].normal, Vector3.up);
                int layer = _collisionContacts[i].otherCollider.gameObject.layer;
                if (angle < _angleOfContactedSurface)
                {
                    _angleOfContactedSurface = angle;
                    _layerOfContactedSurface = layer;
                }
            }
        }

        /// <summary>
        /// Decide the player's horizontal velocity.
        /// </summary>
        private void UpdateHorizontalVelocity()
        {
            GetWASDInputAxes(out float rightLeft, out float forwardsBackwards);

            // Calculate some vectors for later.
            Vector3 targetMovementDirection = transform.forward * forwardsBackwards + transform.right * rightLeft;
            Vector2 targetVelocity = _settings.MaxHorizontalSpeed * new Vector2(targetMovementDirection.x, targetMovementDirection.z);
            Vector2 currentVelocity = HorizontalVelocity;

            // Depending on whether it's falling and whether it's trying to speed up or stop moving, decide how quickly to accelerate.
            float accel = _settings.DecideHorizontalAccelerationMagnitude(_angleOfContactedSurface, targetMovementDirection);

            // Move the current velocity towards the target velocity.
            Vector2 newVelocity = currentVelocity + Time.deltaTime * accel * (targetVelocity - currentVelocity).normalized;

            // Cap the velocity.
            if (newVelocity.magnitude > _settings.MaxHorizontalSpeed)
            {
                newVelocity = newVelocity.normalized * _settings.MaxHorizontalSpeed;
            }

            // If it moved the velocity too far and overshot the target velocity, make it equal the target velocity.
            // Otherwise it'll jitter around zero velocity rather than stopping.
            Vector2 currentVelocityError = currentVelocity - targetVelocity;
            Vector2 newVelocityError = newVelocity - targetVelocity;
            if (Vector2.Dot(currentVelocityError, newVelocityError) < 0)
            {
                newVelocity = targetVelocity;
            }

            // Set the rigidbody's velocity.
            HorizontalVelocity = newVelocity;
        }

        /// <summary>
        /// If the player gets the jump input, change vertical velocity.
        /// </summary>
        private void UpdateVerticalVelocity()
        {
            // If there's a jump input, update the time when got that input.
            bool getJumpInputFirstTimeThisFrame = GetJumpInput() && Time.frameCount != _frameWhenGotJumpInputInFixedUpdate;
            if (getJumpInputFirstTimeThisFrame || _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun)
            {
                _jumpInputTime = Time.time;
                _frameWhenGotJumpInputInFixedUpdate = Time.frameCount;
                _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun = false;
            }

            if (_angleOfContactedSurface < PlayerMovementSettings.SLOPE_DEGREES_TO_JUMP)
            {
                _timeSinceStoodOnJumpableSurface = 0.0f;
                _jumping = false; // It's standing on a flat surface so if it was jumping, it's not anymore
            }
            else
            {
                _timeSinceStoodOnJumpableSurface += Time.deltaTime;
            }

            // If it can jump and got jump input recently, execute the jump.
            bool canExecuteJump = _timeSinceStoodOnJumpableSurface < _settings.CoyoteTime && !_jumping;
            bool gotJumpInputRecently = Time.time <= _jumpInputTime + _settings.JumpBufferTime;
            bool executeJump = canExecuteJump && gotJumpInputRecently;
            _startedJumpingInMostRecentFixedUpdate = executeJump;
            if (executeJump)
            {
                VerticalVelocity = _settings.JumpVelocity;

                _jumping = true;
                _jumpInputTime = float.NegativeInfinity;
                _timeSinceStoodOnJumpableSurface = float.PositiveInfinity;
                _angleOfContactedSurface = float.PositiveInfinity;
            }

            // Apply gravity unless standing still on a gentle slope.

            // The 2nd line of this if statement is b/c when the player walks on a flat surface made of blocks,
            // there are some extraneous collisions or something which bump the player up slightly. It still happens
            // with this but much less noticeable.
            // Need to do this when there's WASD input b/c when the player is walking down a slope, even if they get
            // bumped up relative to the slope, vertical velocity can be negative.
            GetWASDInputAxes(out float rightLeft, out float forwardsBackwards);
            if (_angleOfContactedSurface > PlayerMovementSettings.SLOPE_DEGREES_TO_NOT_SLIDE_DOWN
                || VerticalVelocity > 0 || rightLeft != 0 || forwardsBackwards != 0)
            {
                float totalAccel = -_settings.DecideGravityAcceleration(VerticalVelocity, _jumping);

                // Add drag force for terminal velocity, so it doesn't fall absurdly fast.
                // Don't just cap the velocity because that feels too sudden.
                if (VerticalVelocity < -_settings.DownwardsVelocityAtEndOfJump)
                {
                    // E.g. if it falls at 2m/s at the end of a jump on a flat surface, and it's falling at 3m/s,
                    // then this would equal 1.
                    float fallSpeedBeyondThreshold = -VerticalVelocity - _settings.DownwardsVelocityAtEndOfJump;

                    float dragAccel = _settings.FallDragForceConstant * Mathf.Pow(fallSpeedBeyondThreshold, 2f);
                    totalAccel += dragAccel;
                }

                if (executeJump)
                {
                    // This makes the jump height equal the setting. I don't know why. Otherwise it's like .03 off.
                    // Doesn't really matter but hopefully this makes it feel slightly better, e.g. without this it's
                    // at the top position of the jump for 2 fixed updates in a row rather than 1. No clue why.
                    totalAccel /= 2;
                }

                VerticalVelocity += totalAccel * Time.deltaTime;
            }
        }

        private void GetWASDInputAxes(out float rightLeft, out float forwardsBackwards)
        {
            if (IgnoreInputs)
            {
                rightLeft = 0;
                forwardsBackwards = 0;
                return;
            }

            Vector2 input = _moveAction.action.ReadValue<Vector2>();

            rightLeft = input.x;
            forwardsBackwards = input.y;
        }

        private bool GetJumpInput()
        {
            if (IgnoreInputs)
            {
                return false;
            }
            return _jumpAction.action.IsPressed();
        }

        private void CheckRaiseWASDEvents()
        {
            if (IgnoreInputs)
            {
                return;
            }

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
    }
}
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

        public bool IsMoving => _rigidbody.velocity != Vector3.zero;

        private float _timeSinceGroundedEnoughToJump = Mathf.Infinity;

        private float _jumpInputTime = float.NegativeInfinity;
        private bool _fixedUpdateRanThisFrame;
        private bool _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun;

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
            MovePlayer();
            Jump();
            CapMovementVelocity();
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

        private void CapMovementVelocity()
        {
            if (HorizontalVelocity.magnitude > _settings.MaxHorizontalSpeed)
            {
                HorizontalVelocity = HorizontalVelocity.normalized * _settings.MaxHorizontalSpeed;
            }
        }

        /// <summary>
        /// This moves the player, but does not determine which direction the player goes.
        /// </summary>
        private void MovePlayer()
        {
            //Move Speed Power

            GetWASDInputAxes(out float horizontal, out float vertical);
            Vector3 _moveDirection = transform.forward * vertical + transform.right * horizontal;


            _rigidbody.velocity += Time.deltaTime * _currentHorizontalMovementSettings.AccelToSpeedUp * _moveDirection;

            if (_moveDirection.magnitude == 0)
            {
                Vector2 priorHorizontalVelocity = HorizontalVelocity;
                HorizontalVelocity -= Time.deltaTime * _currentHorizontalMovementSettings.AccelToStop * priorHorizontalVelocity.normalized;
                if (Vector2.Dot(priorHorizontalVelocity, HorizontalVelocity) < 0)
                {
                    // If the dot product is negative, the velocity has changed direction. It's trying to stop,
                    // so it overshot 0. So just make it 0, so it doesn't jitter.
                    HorizontalVelocity = Vector2.zero;
                }
            }
        }

        /// <summary>
        /// If the player gets the jump input, add upwards force.
        /// </summary>
        private void Jump()
        {
            float gravityAccel = _settings.GravityAccel;
            

            if (_rigidbody.velocity.y < 0)
            {
                gravityAccel = _settings.GravityAccelWhileFallingDuringJump;
            }

            if (GetJumpInput() || _jumpInputHappenedInAFrameWhereFixedUpdateDidntRun)
            {
                _jumpInputTime = Time.time;
            }

            bool canJump = _timeSinceGroundedEnoughToJump < _settings.CoyoteTime;
            bool recentJumpInput = Time.time <= _jumpInputTime + _settings.JumpBufferTime;
            if (canJump && recentJumpInput)
            {
                // Jump Power
                VerticalVelocity = _settings.JumpVelocity;
                _jumpInputTime = float.NegativeInfinity;
                _timeSinceGroundedEnoughToJump = float.PositiveInfinity;
            }
            else if (!GroundedEnoughForNoGravity)
            {
                // Gravity Power
                VerticalVelocity -= gravityAccel * Time.deltaTime;
            }
        }

        /// <summary>
        /// Checks if the player is falling.
        /// </summary>
        private void GroundedDetection()
        {
            GroundedEnoughForNoGravity = _groundingSurfaceSlope < 5f;
            GroundedEnoughForNormalAccelerationSettings = _groundingSurfaceSlope < 90f;
            GroundedEnoughToJump = _groundingSurfaceSlope < 60f;

            // Do this so will get the minimum slope angle when the physics engine updates after FixedUpdates run.
            _groundingSurfaceSlope = float.PositiveInfinity; 

            if (GroundedEnoughToJump)
            {
                _timeSinceGroundedEnoughToJump = 0.0f;
            }
            else
            {
                _timeSinceGroundedEnoughToJump += Time.deltaTime;
            }

            _currentHorizontalMovementSettings = GroundedEnoughForNormalAccelerationSettings
                ? _settings.GroundedHorizontalMovementSettings : _settings.NonGroundedHorizontalMovementSettings;
        }

        private void GetWASDInputAxes(out float horizontal, out float vertical)
        {
            horizontal = Input.GetAxisRaw("Horizontal");
            vertical = Input.GetAxisRaw("Vertical");
        }

        private bool GetJumpInput()
        {
            return Input.GetButtonDown("Jump");
        }

        private void CheckRaiseWASDEvents()
        {
            GetWASDInputAxes(out float horizontal, out float vertical);
            if (horizontal < 0)
            {
                _playerMovementA.Raise();
            }
            if (horizontal > 0)
            {
                _playerMovementD.Raise();
            }
            if (vertical > 0)
            {
                _playerMovementW.Raise();
            }
            if (vertical < 0)
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
        }
    }
}
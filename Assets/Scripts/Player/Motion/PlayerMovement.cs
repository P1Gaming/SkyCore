using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Motion
{
    /// <summary>
    /// Controls the players movement.
    /// </summary>

    [RequireComponent(typeof(PlayerInput))]

    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private Transform _forwardReference;
        [SerializeField, Tooltip("Point where the raycast is fired to ground the player for interactions.")]
        private Transform _forceGround;
        [SerializeField, Tooltip("Decides whether the player is on the ground.")]
        private GroundedDecider _groundedDecider;
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
        [SerializeField]
        private LayerMask _groundLayer;

        private Rigidbody _rigidbody;
        private PlayerVelocityDecider _velocityDecider;
        private bool _tryJump;
        private bool _wasGrounded;
        private bool _isInteracting;
        private float _horizontalMovement;
        private float _verticalMovement;
        private Vector3 _moveDirection;

        private static PlayerMovement _instance;
        public static PlayerMovement Instance;

        public PlayerMovementSettings Settings => _settings;

        public bool IsMoving => _rigidbody.velocity != Vector3.zero;

        public bool isInteracting => _isInteracting;

        private float _timeSinceLastGrounded = Mathf.Infinity;

        /// <summary>
        /// Fired when player is no longer grounded.
        /// </summary>
        public event Action Fallen;

        /// <summary>
        /// Fired when player becomes grounded.
        /// </summary>
        public event Action Landed;

        /// <summary>
        /// Fired when player moves, disregarding vertical.
        /// </summary>
        public event Action MovedHorizontally;

        /// <summary>
        // Fired when player executes a jump (not always the same as when the button is pressed.)
        /// </summary>
        public event Action Jumped;

        /// <summary>
        /// Fired every frame and sends whether grounded.
        /// </summary>
        public event Action<bool> UpdateBasedOnGrounded;

        /// <summary>
        /// Trigger on enable to setup movement.
        /// </summary> 

        private void OnEnable()
        {
            if (!TryGetComponent(out PlayerInput handler))
            {
                return;
            }
        }

        /// <summary>
        /// Trigger on disable to clear movement.
        /// Bug where key up is missed when disabled for interaction
        /// </summary> 

        private void OnDisable()
        {
            if (!TryGetComponent(out PlayerInput handler))
            {
                return;
            }

        }

        private void Awake()
        {
            _settings.Initialize();
            _velocityDecider = new PlayerVelocityDecider(_settings);
            _rigidbody = GetComponent<Rigidbody>();
            Instance = this;
        }

        private void Update()
        {
            bool grounded = _groundedDecider.IsGrounded();

            GetInput();
        }

        /// <summary>
        /// This determines which direction the player is going, but does not actually move the player.
        /// </summary>
        private void GetInput()
        {
            _horizontalMovement = Input.GetAxisRaw("Horizontal");
            _verticalMovement = Input.GetAxisRaw("Vertical");

            _moveDirection = transform.forward * _verticalMovement + transform.right * _horizontalMovement;
        }


        private void FixedUpdate()
        {
            MovePlayer();
            Jump();
            FallDetection();
            CapMovementVelocity();
        }

        private void CapMovementVelocity()
        {
            if (_rigidbody.velocity.magnitude > _settings.MaxHorizontalSpeed)
            {
                _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _settings.MaxHorizontalSpeed);
            }
        }

        /// <summary>
        /// This moves the player, but does not determine which direction the player goes.
        /// </summary>
        private void MovePlayer()
        {
            _rigidbody.AddForce(_moveDirection.normalized * _settings.GroundedHorizontalMovementSettings.AccelToSpeedUp, ForceMode.Acceleration);
        }

        /// <summary>
        /// If the player gets the jump input, add upwards force.
        /// </summary>
        private void Jump()
        {
            _tryJump = false;
            if (_timeSinceLastGrounded < _settings.CoyoteTime)
            {
                _tryJump = true;
            }

            if (Input.GetAxisRaw("Jump") > 0 && _tryJump)
            {
                _rigidbody.AddForce(Physics.gravity * _settings.JumpVelocity * -1f, ForceMode.Acceleration);
                _wasGrounded = false;
                StartCoroutine(ResetCoyote());
            }
            else if (!_wasGrounded)
            {
                _rigidbody.AddForce(Physics.gravity * _settings.FallDragForceConstant, ForceMode.Acceleration);
            }
        }

        IEnumerator ResetCoyote()
        {
            yield return new WaitForSeconds(.1f);
            _timeSinceLastGrounded = Mathf.Infinity;
        }

        /// <summary>
        /// Checks if the player is falling.
        /// </summary>
        private void FallDetection()
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), transform.TransformDirection(Vector3.down), 1f, _groundLayer))
            {
                _timeSinceLastGrounded = 0.0f;
                _wasGrounded = true;
            }
            else if (_timeSinceLastGrounded < _settings.CoyoteTime)
            {
                _timeSinceLastGrounded += Time.deltaTime;
                _wasGrounded = false;
            }
        }

        /// <summary>
        /// Moves the player to the position.
        /// </summary>
        public void Teleport(Vector3 position)
        {
            // The enable/disable here doesn't seem to be necessary, but some people on unity
            // forums say the character controller overrides transform position, so maybe it's
            // necessary depending on order of execution or something.
            //_rigidbody.isKinematic = false;
            transform.position = position;
            //_rigidbody.isKinematic = true;
        }

        /// <summary>
        /// Sets Interacting state and toggles enabled
        /// </summary>
        /// <param name="isInteracting"></param>
        public void SetInteract(bool isInteracting)
        {
            // Game Design advised keep pause/disabled player during interaction
            _isInteracting = isInteracting;
            this.enabled = !isInteracting;            
        }

        public void SetLocalHorizontalDirection(PlayerMovement temp)
        {
            //This does nothing
        }
    }
}
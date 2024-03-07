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
        private bool _isInteracting;
        private Vector3 _moveDirection;

        public static PlayerMovement Instance { get; private set; }

        public PlayerMovementSettings Settings => _settings;

        public bool IsMoving => _rigidbody.velocity != Vector3.zero;

        public bool isInteracting => _isInteracting;

        private float _timeSinceLastGrounded = Mathf.Infinity;

        private float _jumpInputTime = float.NegativeInfinity;

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
            _currentHorizontalMovementSettings = _settings.GroundedHorizontalMovementSettings;
            Instance = this;
        }

        private void Update()
        {
            GetInput();
        }

        /// <summary>
        /// This determines which direction the player is going, but does not actually move the player.
        /// </summary>
        private void GetInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            _moveDirection = transform.forward * vertical + transform.right * horizontal;

            if (horizontal > 0)
            {
                _playerMovementA.Raise();
            }
            if (horizontal < 0)
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


        private void FixedUpdate()
        {
            MovePlayer();
            Jump();
            FallDetection();
            CapMovementVelocity();
        }

        private void CapMovementVelocity()
        {
            Vector2 playerVelocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
            if (HorizontalVelocity.magnitude > _settings.MaxHorizontalSpeed)
            {
                playerVelocity.Normalize();
                HorizontalVelocity = _settings.MaxHorizontalSpeed * playerVelocity;
            }
        }

        /// <summary>
        /// This moves the player, but does not determine which direction the player goes.
        /// </summary>
        private void MovePlayer()
        {
            //Move Speed Power
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
            bool canJump = _timeSinceLastGrounded < _settings.CoyoteTime;

            if (_rigidbody.velocity.y < 0 && (canJump || _settings.SameGravityWhenFallOffEdgeAsWhenFallDuringJump))
            {
                gravityAccel = _settings.GravityAccelWhileFallingDuringJump;
            }

            if (Input.GetButtonDown("Jump"))
            {
                _jumpInputTime = Time.time;
            }

            if (canJump && Time.time <= _jumpInputTime + _settings.JumpBufferTime)
            {
                // Jump Power
                VerticalVelocity = _settings.JumpVelocity;
                _jumpInputTime = float.NegativeInfinity;
                _timeSinceLastGrounded = float.PositiveInfinity;
            }
            else if (_timeSinceLastGrounded != 0)
            {
                // Gravity Power
                VerticalVelocity -= gravityAccel * Time.deltaTime;
            }
        }

        /// <summary>
        /// Checks if the player is falling.
        /// </summary>
        private void FallDetection()
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z)
                , transform.TransformDirection(Vector3.down), 1f, _groundLayer))
            {
                _timeSinceLastGrounded = 0.0f;
                _currentHorizontalMovementSettings = _settings.GroundedHorizontalMovementSettings;
            }
            else
            {
                _timeSinceLastGrounded += Time.deltaTime;
                _currentHorizontalMovementSettings = _settings.NonGroundedHorizontalMovementSettings;
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
            _isInteracting = isInteracting;
            enabled = !isInteracting;            
        }
    }
}
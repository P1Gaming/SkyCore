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
        private float _horizontalMovement;
        private float _verticalMovement;
        private Vector3 _moveDirection;

        private static PlayerMovement _instance;
        public static PlayerMovement Instance;

        public PlayerMovementSettings Settings => _settings;

        public bool IsMoving => _rigidbody.velocity != Vector3.zero;

        public bool isInteracting => _isInteracting;

        private float _timeSinceLastGrounded = Mathf.Infinity;

        private float _jumpInputTime = -1f;

        private PlayerHorizontalMovementSettings _currentHorizontalMovement;

        private Vector2 HorizontalVelocity { get => new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z); set => _rigidbody.velocity = new Vector3(value.x, _rigidbody.velocity.y, value.y); }
        private float VerticalVelocity { get => _rigidbody.velocity.y; set => _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, value, _rigidbody.velocity.z); }

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
            _rigidbody = GetComponent<Rigidbody>();
            _currentHorizontalMovement = _settings.GroundedHorizontalMovementSettings;
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
            _horizontalMovement = Input.GetAxisRaw("Horizontal");
            _verticalMovement = Input.GetAxisRaw("Vertical");

            if (_horizontalMovement > 0)
            {
                _playerMovementA.Raise();
            }
            if (_horizontalMovement < 0)
            {
                _playerMovementD.Raise();
            }
            if (_verticalMovement > 0)
            {
                _playerMovementW.Raise();
            }
            if (_verticalMovement < 0)
            {
                _playerMovementS.Raise();
            }

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
            _rigidbody.velocity += Time.deltaTime * _currentHorizontalMovement.AccelToSpeedUp * _moveDirection;

            if (_moveDirection.magnitude < 0.1f)
            {
                Vector2 playerVelocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.z);
                HorizontalVelocity -= Time.deltaTime * _currentHorizontalMovement.AccelToStop * playerVelocity.normalized;
                if (Vector2.Dot(playerVelocity, HorizontalVelocity) < 0)
                {
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
            bool canJump = false;

            if (_timeSinceLastGrounded < _settings.CoyoteTime)
            {
                canJump = true;
            }

            if (_rigidbody.velocity.y < 0 && (canJump || _settings.SameGravityWhenFallOffEdgeAsWhenFallDuringJump))
            {
                gravityAccel = _settings.GravityAccelWhileFallingDuringJump;
            }

            if (Input.GetButtonDown("Jump"))
            {
                _jumpInputTime = Time.time;
            }

            if (canJump && Time.time < _jumpInputTime + _settings.JumpBufferTime)
            {
                //Jump Power
                VerticalVelocity = _settings.JumpVelocity;
                StartCoroutine(ResetCoyote());
                _jumpInputTime = 0f;
            }
            else
            {
                //Gravity Power
                VerticalVelocity -= gravityAccel * Time.deltaTime;
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
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.TransformDirection(Vector3.down), 1f, _groundLayer))
            {
                _timeSinceLastGrounded = 0.0f;
                _currentHorizontalMovement = _settings.GroundedHorizontalMovementSettings;
            }
            else
            {
                _timeSinceLastGrounded += Time.deltaTime;
                _currentHorizontalMovement = _settings.NonGroundedHorizontalMovementSettings;
            }
        }

        /// <summary>
        /// Moves the player to the position.
        /// </summary>
        public void Teleport(Vector3 position)
        {
            transform.position = position;
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
    }
}
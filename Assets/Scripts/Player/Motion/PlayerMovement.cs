using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
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

        private CharacterController _characterController;
        private PlayerVelocityDecider _velocityDecider;
        private Vector2 _localHorizontalDirection = Vector2.zero;
        private bool _tryJump;
        private bool _wasGrounded;
        private bool _isInteracting;

        private static PlayerMovement _instance;
        public static PlayerMovement Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<PlayerMovement>();
                }
                return _instance;
            }
        }

        public float HorizontalMovementInputMagnitude => _localHorizontalDirection.magnitude;

        public PlayerMovementSettings Settings => _settings;

        public bool IsMoving => _characterController.velocity != Vector3.zero;

        public bool isInteracting => _isInteracting;

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

            RegisterEventHandlers(handler);
        }

        /// <summary>
        /// Trigger on disable to clear movement.
        /// Bug where key up is missed when disabled for interaction
        /// </summary> 

        private void OnDisable()
        {
            if (isInteracting)
            {
                _localHorizontalDirection = Vector2.zero;

                // Stop micro movement after interaction from remaining velocity
                if (_wasGrounded)
                {
                    _characterController.Move(Vector3.zero);
                }
            }
            if (!TryGetComponent(out PlayerInput handler))
            {
                return;
            }

            UnregisterEventHandlers(handler);

        }

        private void Awake()
        {
            _settings.Initialize();
            _velocityDecider = new PlayerVelocityDecider(_settings);
            _characterController = GetComponent<CharacterController>();
            _groundedDecider.Initialize(_characterController);

            _wasGrounded = _groundedDecider.IsGrounded();
        }

        private void Update()
        {
            bool grounded = _groundedDecider.IsGrounded();

            Vector2 targetHorizontalDirection = ConvertLocalHorizontalDirectionToGlobal();

            Vector3 newVelocity = _velocityDecider.UpdateVelocity(grounded, targetHorizontalDirection
                , _characterController.velocity, _tryJump, out bool jumped);

            _tryJump = false;

            _characterController.Move(newVelocity * Time.deltaTime);

            CheckFireEvents(grounded, newVelocity, jumped);
        }

        private void CheckFireEvents(bool grounded, Vector3 newVelocity, bool jumped)
        {
            if (grounded && !_wasGrounded)
            {
                Landed?.Invoke();
            }
            if (!grounded && _wasGrounded)
            {
                Fallen?.Invoke();
            }
            _wasGrounded = grounded;

            if (newVelocity.x != 0 || newVelocity.z != 0)
            {
                MovedHorizontally?.Invoke();
            }

            if (jumped)
            {
                Jumped?.Invoke();
            }

            UpdateBasedOnGrounded?.Invoke(grounded);
        }

        /// <summary>
        /// Converts the local target horizontal direction to global, so it rotates with the 
        /// transform as you move the mouse.
        /// If forward remefernce, the camera target or any other object, is assigned,
        /// will use that transform to decide forward directions, with rotation of
        /// the camera.
        /// </summary>
        /// <returns>The direction to move in the world on the horizontal plane.</returns>
        private Vector2 ConvertLocalHorizontalDirectionToGlobal()
        {
            if (_localHorizontalDirection == Vector2.zero)
            {
                return Vector2.zero;
            }
            Vector3 globalDirection;
            if (_forwardReference != null)
            {
                globalDirection = (_forwardReference.forward * _localHorizontalDirection.y)
                + (_forwardReference.right * _localHorizontalDirection.x);
            }
            else
            {
                globalDirection = (transform.right * _localHorizontalDirection.x)
                + (transform.forward * _localHorizontalDirection.y);
            }
            

            Vector2 result = new Vector2(globalDirection.x, globalDirection.z);
            return result.normalized;
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (context.canceled || _isInteracting)
            {
                _localHorizontalDirection = Vector2.zero;
                return;
            }

            _localHorizontalDirection = context.ReadValue<Vector2>();

            if (_localHorizontalDirection == Vector2.up)
            {
                _playerMovementW.Raise();
            }

            if (_localHorizontalDirection == Vector2.left)
            {
                _playerMovementA.Raise();
            }

            if (_localHorizontalDirection == Vector2.right)
            {
                _playerMovementD.Raise();
            }

            if (_localHorizontalDirection == Vector2.down)
            {
                _playerMovementS.Raise();
            }
        }

        private void OnJump(InputAction.CallbackContext context)
        {
            _tryJump = !_isInteracting;
        }

        protected void RegisterEventHandlers(PlayerInput input)
        {
            InputAction moveAction = input.actions.FindAction("Move");
            if (moveAction != null)
            {
                moveAction.canceled += OnMove;
                moveAction.performed += OnMove;
            }

            InputAction jumpAction = input.actions.FindAction("Jump");
            if (jumpAction != null)
            {
                jumpAction.performed += OnJump;
            }
        }

        protected void UnregisterEventHandlers(PlayerInput input)
        {
            InputAction moveAction = input.actions.FindAction("Move");
            if (moveAction != null)
            {
                moveAction.canceled -= OnMove;
                moveAction.performed -= OnMove;
            }

            InputAction jumpAction = input.actions.FindAction("Jump");
            if (jumpAction != null)
            {
                jumpAction.performed -= OnJump;
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
            _characterController.enabled = false;
            transform.position = position;
            _characterController.enabled = true;
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

        /// <summary>
        /// Sets the direction of player movement.
        /// </summary>
        /// <param name="direction"></param>
        public void SetLocalHorizontalDirection(Vector2 direction)
        {
            _localHorizontalDirection = direction;
        }
    }
}
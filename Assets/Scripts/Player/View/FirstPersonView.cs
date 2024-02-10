using UnityEngine;
using UnityEngine.InputSystem;

///<summary>
/// This class handles the use of the target of a cinemachine camera
/// to rotate said target, thus rotating the camera. Dampening is used
/// on the cinemachine virtual camera to remove jitters from this script,
/// making the cmaera turn smoother.
///</summary>
namespace Player.View
{

    public class FirstPersonView : MonoBehaviour
    {
        [Header("Modifiable by player")]
        [SerializeField]
        private bool _isInverted = false;

        private float _sensitivitySetting = 1f;

        [Space(5)]
        [Header("Hiden from player")]
        [SerializeField, Range(0.01f, 20f)]
        private float _baseSpeed;

        [SerializeField]
        private float _lookThreshold;

        [SerializeField, Range(0, 89.9f)]
        private float _clampAngle = 89.9f;

        [SerializeField]
        private GameObject _cameraTarget;

        [SerializeField]
        private PlayerInput _playerControls;

        [SerializeField]
        private GameEventScriptableObject _lookDirectionChangeEvent;

        private Vector2 _lookDirection;

        private float _xRot = 0f;

        private void OnEnable()
        {
            if (!TryGetComponent(out PlayerInput handler))
            {
                return;
            }

            RegisterEventHandlers(handler);
        }

        private void OnDisable()
        {
            if (!TryGetComponent(out PlayerInput handler))
            {
                return;
            }

            UnregisterEventHandlers(handler);

        }

        /// <summary>
        /// Get the starting local rotation of the look point in the x direction.
        /// Hide and lock cursor to center of screen.
        /// </summary> 
        protected void Awake()
        {
            _xRot = _cameraTarget.transform.rotation.eulerAngles.x;
            Cursor.lockState = (CursorLockMode.Locked);
            Cursor.visible = false;
        }

        /// <summary>
        /// Call update each frame, to run the look fuction.
        /// </summary> 
        void Update()
        {
            PerformLook();
        }

        /// <summary>
        /// Rotates the target of the cinemachine camera,
        /// based on current delta input of the mouse from player input.
        /// Sensitivity is controlled in settings by player.
        /// </summary> 

        private void PerformLook()
        {
            if (_lookDirection.sqrMagnitude < _lookThreshold)
            {
                return;
            }
            _lookDirectionChangeEvent.Raise();
            _xRot -= _lookDirection.y * (_sensitivitySetting * _baseSpeed);
            _xRot = Mathf.Clamp(_xRot, -_clampAngle, _clampAngle);
            transform.Rotate(0f, _lookDirection.x * (_sensitivitySetting * _baseSpeed), 0f);
            if(_isInverted)
            {
                _cameraTarget.transform.localRotation = Quaternion.Euler(_xRot, 0, 0);
            }
            else
            {
                _cameraTarget.transform.localRotation = Quaternion.Euler(-_xRot, 0, 0);
            }
        }

        private void OnLook(InputAction.CallbackContext context)
        {
            _lookDirection = context.ReadValue<Vector2>();
        }

        private void RegisterEventHandlers(PlayerInput input)
        {
            InputAction moveAction = input.actions.FindAction("Look");
            if (moveAction != null)
            {
                moveAction.started += OnLook;
                moveAction.performed += OnLook;
                moveAction.canceled += OnLook;
            }
        }

        private void UnregisterEventHandlers(PlayerInput input)
        {
            InputAction moveAction = input.actions.FindAction("Look");
            if (moveAction != null)
            {
                moveAction.started -= OnLook;
                moveAction.performed -= OnLook;
                moveAction.canceled -= OnLook;
            }
        }

        /// <summary>
        /// Function to allow slider settings script to set sensitivity
        /// as the player desires.
        /// </summary> 
        public void SetSensitivity(float newSens)
        {
            _sensitivitySetting = newSens;
        }

        public float GetSensitivity()
        {
            return _sensitivitySetting;
        }

    }
}
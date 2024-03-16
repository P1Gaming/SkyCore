using UnityEngine;
using UnityEngine.InputSystem;

///<summary>
/// This class handles the use of the target of a cinemachine camera
/// to rotate said target, thus rotating the camera. Dampening is used
/// on the cinemachine virtual camera to remove jitters from this script,
/// making the camera turn smoother.
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

        private static FirstPersonView _instance;
        public static FirstPersonView Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindWithTag("Player").GetComponent<FirstPersonView>();
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
                    throw new System.Exception("In FirstPersonView, _numberOfReasonsToIgnoreInputs < 0: " + _numberOfReasonsToIgnoreInputs);
                }
                if (_numberOfReasonsToIgnoreInputs > 0)
                {
                    // Clear recent inputs
                    _lookDirection = Vector2.zero;
                }
            }
        }
        private bool IgnoreInputs => NumberOfReasonsToIgnoreInputs > 0;


        /// <summary>
        /// Get the starting local rotation of the look point in the x direction.
        /// Hide and lock cursor to center of screen.
        /// </summary> 
        protected void Awake()
        {
            _instance = this;
            _xRot = _cameraTarget.transform.rotation.eulerAngles.x;
            CursorMode.Initialize();


            // Get the sensitivity value if one is saved, or use the default.
            float sensitivity = PlayerPrefs.GetFloat(PlayerPrefsKeys.FirstPersonViewSensitivity,
                                                            DefaultSettingsMenuValues.FirstPersonViewSensitivity);

            _sensitivitySetting = sensitivity / 100f;
        }

        private void OnEnable()
        {
            _lookDirection = Vector2.zero;
            RegisterEventHandlers(GetComponent<PlayerInput>());
        }

        private void OnDisable()
        {
            UnregisterEventHandlers(GetComponent<PlayerInput>());
        }

        /// <summary>
        /// Call update each frame, to run the look fuction.
        /// </summary> 
        private void Update()
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
            if (_lookDirection.sqrMagnitude == 0)
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
            if (!IgnoreInputs)
            {
                _lookDirection = context.ReadValue<Vector2>();
            }
        }

        private void RegisterEventHandlers(PlayerInput input)
        {
            InputAction lookAction = input.actions.FindAction("Look");
            if (lookAction != null)
            {
                lookAction.started += OnLook;
                lookAction.performed += OnLook;
                lookAction.canceled += OnLook;
            }
        }

        private void UnregisterEventHandlers(PlayerInput input)
        {
            InputAction lookAction = input.actions.FindAction("Look");
            if (lookAction != null)
            {
                lookAction.started -= OnLook;
                lookAction.performed -= OnLook;
                lookAction.canceled -= OnLook;
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
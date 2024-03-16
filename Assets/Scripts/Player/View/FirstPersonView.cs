using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

using Cursor = UnityEngine.Cursor;


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
        [SerializeField]
        private InputActionReference _cameraLookAction;


        [Header("Modifiable by player")]
        [SerializeField]
        private bool _isInverted = false;

        private float _sensitivitySetting = 1f;

        [Space(5)]
        [Header("Hiden from player")]
        [SerializeField, Range(0.01f, 20f)]
        private float _baseSpeed;

        [SerializeField, Range(0, 89.9f)]
        private float _clampVerticalAngle = 89.9f;


        [SerializeField]
        private GameObject _cameraTarget;

        [SerializeField]
        private GameEventScriptableObject _lookDirectionChangeEvent;

        private Vector2 _lookDirection;

        private float _verticalRotation = 0f;

        private float _horizontalClampAngle;



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






        private void OnEnable()
        {
            _lookDirection = Vector2.zero;
            RegisterEventHandlers();
        }

        private void OnDisable()
        {
            UnregisterEventHandlers();
        }

        /// <summary>
        /// Get the starting local rotation of the look point in the x direction.
        /// Hide and lock cursor to center of screen.
        /// </summary> 
        protected void Awake()
        {
            _instance = this;
            _verticalRotation = _cameraTarget.transform.rotation.eulerAngles.x;
            CursorMode.Initialize();


            // Get the sensitivity value if one is saved, or use the default.
            float sensitivity = PlayerPrefs.GetFloat(PlayerPrefsKeys.FirstPersonViewSensitivity,
                                                     DefaultSettingsMenuValues.FirstPersonViewSensitivity);

            _sensitivitySetting = sensitivity / 100f;
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
            if (_lookDirection.sqrMagnitude == 0)
            {
                return;
            }
            _lookDirectionChangeEvent.Raise();



            // Clamp the camera's vertical rotation.
            _verticalRotation -= _lookDirection.y * (_sensitivitySetting * _baseSpeed);
            _verticalRotation = Mathf.Clamp(_verticalRotation, -_clampVerticalAngle, _clampVerticalAngle);

            transform.Rotate(0f, _lookDirection.x * (_sensitivitySetting * _baseSpeed), 0f);
            if(_isInverted)
            {
                _cameraTarget.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
            }
            else
            {
                _cameraTarget.transform.localRotation = Quaternion.Euler(-_verticalRotation, 0, 0);
            }


            // Clamp the camera's horizontal rotation if that option is enabled. This is used by the movement tutorial.
            if (ClampHorizontalAngle)
            {
                Quaternion q = transform.localRotation;
                Vector3 lookRotation = q.eulerAngles;
               
                lookRotation.y = AngleUtils.ConvertAngleToPlusMinus180(lookRotation.y); // Covert from 0-360f to -180 to 180 degrees.
                lookRotation.y = Mathf.Clamp(lookRotation.y, -_horizontalClampAngle, _horizontalClampAngle);
                lookRotation.y = AngleUtils.ConvertAngleBackTo0To360(lookRotation.y);
                
                q.eulerAngles = lookRotation;
                transform.localRotation = q;
            }

        }

        private void OnLook(InputAction.CallbackContext context)
        {
            if (!IgnoreInputs)
            {
                _lookDirection = context.ReadValue<Vector2>();
            }
        }

        private void RegisterEventHandlers()
        {
            if (_cameraLookAction != null)
            {
                _cameraLookAction.action.started += OnLook;
                _cameraLookAction.action.performed += OnLook;
                _cameraLookAction.action.canceled += OnLook;
            }
        }

        private void UnregisterEventHandlers()
        {
            if (_cameraLookAction != null)
            {
                _cameraLookAction.action.started -= OnLook;
                _cameraLookAction.action.performed -= OnLook;
                _cameraLookAction.action.canceled -= OnLook;
            }
        }

        /// <summary>
        /// Function to allow slider settings script to set sensitivity
        /// as the player desires.
        /// </summary> 
        public void SetSensitivity(float newSens)
        {
            Debug.Log(newSens);
            _sensitivitySetting = newSens;
        }

        public float GetSensitivity()
        {
            return _sensitivitySetting;
        }

        /// <summary>
        /// When enabled, the camera's horizontal angle will be clamped so it can't rotate left or right by more than the
        /// amount specified by ClampingAngleY.
        /// </summary>
        public bool ClampHorizontalAngle { get; set; }
        
        /// <summary>
        /// Gets/Sets how much to limit the camera's horizontal rotation by. This parameter is only used when ClampHorizontalAngle is enabled.
        /// </summary>
        public float HorizontalClampingAngle
        {
            get { return _horizontalClampAngle; }
            set { _horizontalClampAngle = Mathf.Clamp(value, 0.0f, 259.9f); }
        }

    }
}
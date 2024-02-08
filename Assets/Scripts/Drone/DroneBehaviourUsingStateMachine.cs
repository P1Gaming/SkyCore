using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

// Just a way to test FiniteStateMachine. We should use DroneBehaviour, not this.
public class DroneBehaviourUsingStateMachine : MonoBehaviour
{
    [SerializeField]
    private DroneBehaviourSettings _settings;
    [SerializeField]
    private FSMDefinition _stateMachineDefinition;

    [Header("Visuals")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
    [SerializeField]
    private LineRenderer _placeholderScanningVisual;
    [SerializeField]
    private Sprite _droneMedical;
    [SerializeField]
    private Sprite _droneCameraControls;
    [SerializeField]
    private Sprite _droneMovementControls;
    [SerializeField]
    private Sprite _droneTutorialComplete;

    [Header("Finite State Machine Parameters")]
    [SerializeField]
    private FSMParameter _distanceFromPlayerParameter;
    [SerializeField]
    private FSMParameter _distanceFromUnscannedItemParameter;
    [SerializeField]
    private FSMParameter _seesUnscannedItemParameter;
    [SerializeField]
    private FSMParameter _scanTimeLeftParameter;
    [SerializeField]
    private FSMParameter _finishedTutorialParameter;
    [SerializeField]
    private FSMParameter _maxDistanceFromTargetPositionToStopParameter;
    [SerializeField]
    private FSMParameter _minDistanceFromTargetPositionToResumeMoving;
    [SerializeField]
    private FSMParameter _minDistanceFromTargetPositionToResumeMovingDuringTutorial;

    [Header("Finite State Machine Events")]
    [SerializeField]
    private GameEventScriptableObject _updateIdleEvent;
    [SerializeField]
    private GameEventScriptableObject _enterTutorialEvent;
    [SerializeField]
    private GameEventScriptableObject _updateTutorialEvent;
    [SerializeField]
    private GameEventScriptableObject _exitTutorialEvent;
    [SerializeField]
    private GameEventScriptableObject _updateFollowPlayerEvent;
    [SerializeField]
    private GameEventScriptableObject _updateMoveTowardsItemToScanEvent;
    [SerializeField]
    private GameEventScriptableObject _enterScanningItemEvent;
    [SerializeField]
    private GameEventScriptableObject _updateScanningItemEvent;
    [SerializeField]
    private GameEventScriptableObject _exitScanningItemEvent;

    [Header("Events for Tutorial Detecting Controls")]
    [SerializeField]
    private GameEventScriptableObject _playerLookControlsEvent;
    [SerializeField]
    private GameEventScriptableObject _playerMovementWEvent;
    [SerializeField]
    private GameEventScriptableObject _playerMovementSEvent;
    [SerializeField]
    private GameEventScriptableObject _playerMovementAEvent;
    [SerializeField]
    private GameEventScriptableObject _playerMovementDEvent;

    private FiniteStateMachineInstance _stateMachine;
    private Transform _player;

    private MonoBehaviour _currentThingToScan;
    private DroneScannableLocator _itemLocator = new();
    private float _scanTimeLeft;

    private bool _movedCamera = false;
    private bool _playerPressedW = false;
    private bool _playerPressedA = false;
    private bool _playerPressedS = false;
    private bool _playerPressedD = false;
    private bool _medicalPromptShown = false;
    private float _droneTutorialTimer;

    private void Awake()
    {
        _stateMachine = new FiniteStateMachineInstance(_stateMachineDefinition);
        _player = Player.Motion.PlayerMovement.Instance.transform;

        _stateMachine.SetFloat(_maxDistanceFromTargetPositionToStopParameter
            , _settings.MaxDistanceFromTargetPositionToStop);
        _stateMachine.SetFloat(_minDistanceFromTargetPositionToResumeMoving
           , _settings.MinDistanceFromTargetPositionToResumeMoving);
        _stateMachine.SetFloat(_minDistanceFromTargetPositionToResumeMovingDuringTutorial
            , _settings.MinDistanceFromTargetPositionToResumeMovingDuringTutorial);
    }

    private void Start()
    {
        // this is in start just to maybe make it easier to load skip tutorial from save data
        _stateMachine.SetBool(_finishedTutorialParameter, _settings.SkipTutorial);
    }

    private void OnEnable()
    {
        _updateIdleEvent.OnRaise += UpdateIdle;
        _enterTutorialEvent.OnRaise += EnterTutorial;
        _updateTutorialEvent.OnRaise += UpdateTutorial;
        _exitTutorialEvent.OnRaise += ExitTutorial;
        _updateFollowPlayerEvent.OnRaise += UpdateMoveTowardsPlayer;
        _updateMoveTowardsItemToScanEvent.OnRaise += UpdateMoveTowardsItemToScan;
        _enterScanningItemEvent.OnRaise += EnterScanningItem;
        _updateScanningItemEvent.OnRaise += UpdateScanningItem;
        _exitScanningItemEvent.OnRaise += ExitScanningItem;

        _playerLookControlsEvent.OnRaise += PlayerMovedCamera;
        _playerMovementWEvent.OnRaise += PlayerMoved;
        _playerMovementSEvent.OnRaise += PlayerMoved;
        _playerMovementAEvent.OnRaise += PlayerMoved;
        _playerMovementDEvent.OnRaise += PlayerMoved;
    }

    private void OnDisable()
    {
        _updateIdleEvent.OnRaise -= UpdateIdle;
        _enterTutorialEvent.OnRaise -= EnterTutorial;
        _updateTutorialEvent.OnRaise -= UpdateTutorial;
        _exitTutorialEvent.OnRaise -= ExitTutorial;
        _updateFollowPlayerEvent.OnRaise -= UpdateMoveTowardsPlayer;
        _updateMoveTowardsItemToScanEvent.OnRaise -= UpdateMoveTowardsItemToScan;
        _enterScanningItemEvent.OnRaise -= EnterScanningItem;
        _updateScanningItemEvent.OnRaise -= UpdateScanningItem;
        _exitScanningItemEvent.OnRaise -= ExitScanningItem;

        _playerLookControlsEvent.OnRaise -= PlayerMovedCamera;
        _playerMovementWEvent.OnRaise -= PlayerMoved;
        _playerMovementSEvent.OnRaise -= PlayerMoved;
        _playerMovementAEvent.OnRaise -= PlayerMoved;
        _playerMovementDEvent.OnRaise -= PlayerMoved;
    }

    private void Update()
    {
        UpdateCurrentItemToScan();

        _stateMachine.SetFloat(_distanceFromPlayerParameter, FromDroneToNear(_player).magnitude);

        _stateMachine.Update();
    }

    private void UpdateCurrentItemToScan()
    {
        if (_currentThingToScan == null || _itemLocator.AlreadyScanned(_currentThingToScan))
        {
            _currentThingToScan = _itemLocator.TryGetThingToScan(transform, _player
                , _settings.DetectionRange, _settings.MaxDistanceFromPlayerToScan);
            _scanTimeLeft = _settings.ScanningTime;
        }
        float distanceToUnscannedItem = float.PositiveInfinity;
        if (_currentThingToScan != null)
        {
            distanceToUnscannedItem = FromDroneToNear(_currentThingToScan.transform, backAwayIfTooClose: false).magnitude;
        }
        _stateMachine.SetFloat(_distanceFromUnscannedItemParameter, distanceToUnscannedItem);
        _stateMachine.SetBool(_seesUnscannedItemParameter, _currentThingToScan != null);
        _stateMachine.SetFloat(_scanTimeLeftParameter, _scanTimeLeft);
    }

    #region Drone Movement
    //---------------------------------------------------------------

    /// <summary>
    /// Moves the drone along a vector, but not past the end of that vector.
    /// </summary>
    private void MoveDrone(Vector3 toTarget)
    {
        float maxMovementDistance = Time.deltaTime * _settings.MovementSpeed;
        transform.position += Vector3.MoveTowards(Vector3.zero, toTarget, maxMovementDistance);
    }

    /// <summary>
    /// Returns a vector from the drone to a point near the target.
    /// </summary>
    private Vector3 FromDroneToNear(Transform toHoverNear, bool backAwayIfTooClose = true)
    {
        Vector3 targetPosition = MoveUpAndTowardsDrone(toHoverNear.position, transform.position, _settings.HoverHeight
            , _settings.MoveToHorizontalDistanceFromTarget);
        Vector3 toTargetPosition = targetPosition - transform.position;

        if (!backAwayIfTooClose)
        {
            // If it's already closer horizontally than the target position, use its current horizontal position
            Vector3 toSamePosition = toHoverNear.position - transform.position;
            float horizontalDistanceToSamePosition = (new Vector2(toSamePosition.x, toSamePosition.z)).magnitude;
            float horizontalDistanceToNear = (new Vector2(toTargetPosition.x, toTargetPosition.z)).magnitude;
            if (horizontalDistanceToSamePosition < horizontalDistanceToNear)
            {
                toTargetPosition.x = 0;
                toTargetPosition.z = 0;
            }
        }

        return toTargetPosition;
    }

    /// <summary>
    /// Moves a vector up and towards the drone (but not vertically towards the drone, only 
    /// horizontally.) The point is to get a vector near an object, for the drone to hover 
    /// next to.
    /// </summary>
    private static Vector3 MoveUpAndTowardsDrone(Vector3 toMove, Vector3 dronePosition
        , float upDistance, float horizontalDistance)
    {
        Vector3 horizontalDirection = toMove - dronePosition;
        horizontalDirection.y = 0;
        horizontalDirection.Normalize();

        return toMove + (upDistance * Vector3.up) - (horizontalDistance * horizontalDirection);
    }

    /// <summary>
    /// Rotates the drone towards the provided transform.
    /// </summary>
    private void RotateTowardsTarget(Transform target)
    {
        Quaternion priorRotation = transform.rotation;

        // is there a way to get this rotation w/o changing the transform's rotation?
        transform.LookAt(target);
        Quaternion rotateTowards = transform.rotation;

        transform.rotation = Quaternion.RotateTowards(priorRotation, rotateTowards, _settings.RotationSpeed * Time.deltaTime);
    }
    #endregion


    #region State Machine Event Responses
    //---------------------------------------------------------------

    private void UpdateIdle()
    {
        //RotateTowardsTarget(_player);
        float rotateDegrees = _settings.IdleRotationSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0, rotateDegrees, 0));
    }

    private void UpdateMoveTowardsPlayer()
    {
        RotateTowardsTarget(_player);
        MoveDrone(FromDroneToNear(_player));
    }

    private void UpdateMoveTowardsItemToScan()
    {
        RotateTowardsTarget(_currentThingToScan.transform);
        MoveDrone(FromDroneToNear(_currentThingToScan.transform, backAwayIfTooClose: false));
    }

    private void EnterScanningItem()
    {
        _scanTimeLeft = _settings.ScanningTime;
        _stateMachine.SetFloat(_scanTimeLeftParameter, _scanTimeLeft);
        _placeholderScanningVisual.enabled = true;
    }

    private void UpdateScanningItem()
    {
        _scanTimeLeft -= Time.deltaTime;
        _stateMachine.SetFloat(_scanTimeLeftParameter, _scanTimeLeft);
        _placeholderScanningVisual.SetPosition(0, transform.position);
        _placeholderScanningVisual.SetPosition(1, _currentThingToScan.transform.position);
    }

    private void ExitScanningItem()
    {
        if (_scanTimeLeft <= 0)
        {
            _itemLocator.OnFinishedScanning(_currentThingToScan);

            PickupItem item = _currentThingToScan as PickupItem;
            JellyInteractBase jelly = _currentThingToScan as JellyInteractBase;
            if (item != null)
            {
                bool isCraftingMaterial = item.ItemInfo.Attributes.Contains(ItemAttribute.CraftingMaterial);
                bool isJellyFood = item.ItemInfo.Attributes.Contains(ItemAttribute.JellyFood);
                Debug.Log("scanned item is crafting material: " + isCraftingMaterial
                    + ", isJellyFood: " + isJellyFood
                    + ", item position: " + item.transform.position);
            }
            else if (jelly != null)
            {
                Debug.Log("scanned a jelly! yay! position: " + jelly.transform.position);
            }
            else
            {
                Debug.LogError("log for this type isnt implemented (are we scanning another thing besides jellies and items?");
            }
        }
        _placeholderScanningVisual.enabled = false;

        UpdateCurrentItemToScan();
    }

    private void EnterTutorial()
    {
        // This & exit will happen multiple times if the drone moves to be sufficiently close to the player
        // and then shows the current tutorial step's pictogram.
        _pictogramBehaviour.SetImageActive(true);
        _pictogramBehaviour.SetBackdropImage();
    }

    private void ExitTutorial()
    {
        _pictogramBehaviour.SetImageActive(false);
    }

    private void UpdateTutorial()
    {
        RotateTowardsTarget(_player); // still might want to make the pictogram directly face the camera

        if (!_medicalPromptShown)
        {
            _pictogramBehaviour.ChangePictogramImage(_droneMedical);
            _droneTutorialTimer += Time.deltaTime;

            if (_droneTutorialTimer >= 5f) // If 5 seconds have passed
            {
                _medicalPromptShown = true;
                _droneTutorialTimer = 0f;
            }
        }
        else
        {
            if (!_movedCamera)
            {
                _pictogramBehaviour.ChangePictogramImage(_droneCameraControls);
            }
            else
            {
                if (!_playerPressedW && !_playerPressedA && !_playerPressedS && !_playerPressedD)
                {
                    _pictogramBehaviour.ChangePictogramImage(_droneMovementControls);
                }
                else
                {
                    _pictogramBehaviour.ChangePictogramImage(_droneTutorialComplete);
                    _droneTutorialTimer += Time.deltaTime;

                    if (_droneTutorialTimer >= 2f) // If 2 seconds have passed
                    {
                        _stateMachine.SetBool(_finishedTutorialParameter, true);
                    }
                }
            }
        }
    }
    #endregion


    public void PlayerMovedCamera()
    {
        _movedCamera = true;
    }

    public void PlayerMoved()
    {
        if (_movedCamera)
        {
            _playerPressedW = true;
            _playerPressedA = true;
            _playerPressedS = true;
            _playerPressedD = true;
        }
    }
}

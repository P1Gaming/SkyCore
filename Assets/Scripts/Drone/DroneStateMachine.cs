using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

public class DroneStateMachine : MonoBehaviour
{
    [SerializeField]
    private bool _logStateMachineTransitions;
    [SerializeField]
    private DroneBehaviourSettings _settings;
    [SerializeField]
    private FSMDefinition _stateMachineDefinition;

    [Header("Visuals")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
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
    private FSMParameter _distanceFromHoverbyPlayerParameter;
    [SerializeField]
    private FSMParameter _finishedTutorialParameter;

    [Header("Finite State Machine Events")]
    [SerializeField]
    private GameEventScriptableObject _idleUpdate;
    [SerializeField]
    private GameEventScriptableObject _tutorialEnter;
    [SerializeField]
    private GameEventScriptableObject _tutorialUpdate;
    [SerializeField]
    private GameEventScriptableObject _tutorialExit;
    [SerializeField]
    private GameEventScriptableObject _followPlayerUpdate;

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

    [SerializeField]
    private DroneScanning _scanning;

    private FiniteStateMachineInstance _stateMachineInstance;

    private Transform _player;

    private bool _movedCamera = false;
    private bool _playerPressedWASD = false;
    private bool _medicalPromptShown = false;
    private float _droneTutorialTimer;
    private GameEventResponses _gameEventResponses = new();

    


    private void Awake()
    {
        if (_stateMachineInstance == null)
        {
            _stateMachineInstance = new FiniteStateMachineInstance(_stateMachineDefinition, this, _logStateMachineTransitions);
        }
        _player = Player.Motion.PlayerMovement.Instance.transform;
        _stateMachineInstance.SetBool(_finishedTutorialParameter, _settings.SkipTutorial);

        _gameEventResponses.SetResponses(
            (_playerLookControlsEvent, PlayerMovedCamera)
            , (_playerMovementWEvent, PlayerMoved)
            , (_playerMovementSEvent, PlayerMoved)
            , (_playerMovementAEvent, PlayerMoved)
            , (_playerMovementDEvent, PlayerMoved)
            );

        // There's only 1 drone, so don't really need to use selective responses for the drone, but might as well.
        _gameEventResponses.SetSelectiveResponses(this
            , (_idleUpdate, UpdateIdle)
            , (_tutorialEnter, EnterTutorial)
            , (_tutorialUpdate, UpdateTutorial)
            , (_tutorialExit, ExitTutorial)
            , (_followPlayerUpdate, UpdateMoveTowardsPlayer)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        _scanning.OnPreStateMachineInstanceUpdate();

        _stateMachineInstance.SetFloat(_distanceFromHoverbyPlayerParameter, FromDroneToNear(_player).magnitude);
        _stateMachineInstance.Update();

    }

    #region Drone Movement
    //---------------------------------------------------------------

    /// <summary>
    /// Moves the drone along a vector, but not past the end of that vector.
    /// </summary>
    public void MoveDrone(Vector3 toTarget)
    {
        float maxMovementDistance = Time.deltaTime * _settings.MovementSpeed;
        transform.position += Vector3.MoveTowards(Vector3.zero, toTarget, maxMovementDistance);
    }

    /// <summary>
    /// Returns a vector from the drone to a point near the target.
    /// </summary>
    public Vector3 FromDroneToNear(Transform toHoverNear, bool backAwayIfTooClose = true)
    {
        Vector3 targetPosition = MoveUpAndTowardsDrone(toHoverNear.position, transform.position, _settings.HoverHeight
            , _settings.MoveToHorizontalDistanceFromTarget);
        Vector3 toTargetPosition = targetPosition - transform.position;

        if (!backAwayIfTooClose)
        {
            // If it's already closer horizontally than the target position, use its current horizontal position
            Vector3 toSamePosition = toHoverNear.position - transform.position;
            float horizontalDistanceToSamePosition = (new Vector2(toSamePosition.x, toSamePosition.z)).magnitude;
            if (horizontalDistanceToSamePosition < _settings.MoveToHorizontalDistanceFromTarget)
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
    public static Vector3 MoveUpAndTowardsDrone(Vector3 toMove, Vector3 dronePosition
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
    public void RotateTowardsTarget(Transform target)
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
        float rotateDegrees = _settings.IdleRotationSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0, rotateDegrees, 0));
    }

    private void UpdateMoveTowardsPlayer()
    {
        RotateTowardsTarget(_player);
        MoveDrone(FromDroneToNear(_player));
    }

    //private void UpdateMoveTowardsToScan()
    //{
    //    RotateTowardsTarget(_toScan.transform);
    //    MoveDrone(FromDroneToNear(_toScan.transform, backAwayIfTooClose: false));
    //}

    //private void EnterScanAttempting()
    //{
    //    _placeholderScanAttemptingVisual.enabled = true;
    //}

    //private void UpdateScanAttempting()
    //{
    //    _placeholderScanAttemptingVisual.SetPosition(0, transform.position);
    //    _placeholderScanAttemptingVisual.SetPosition(1, _toScan.transform.position);
    //}

    //private void EnterScanAwaitingButton()
    //{
    //    _scanButtonUI.SetActive(true);
    //}

    //private void UpdateScanAwaitingButton()
    //{
    //    _placeholderScanAttemptingVisual.SetPosition(0, transform.position);
    //    _placeholderScanAttemptingVisual.SetPosition(1, _toScan.transform.position);

    //    if (Input.GetKeyDown(KeyCode.Return))
    //    {
    //        _stateMachine.SetBool(_buttonPressedParameter, true);
    //    }
    //}

    //private void ExitScanAwaitingButton()
    //{
    //    _scanButtonUI.SetActive(false);
    //    _stateMachine.SetBool(_buttonPressedParameter, false);
    //}

    //private void ExitScanFailed()
    //{
    //    _toScan = null;
    //    _placeholderScanAttemptingVisual.enabled = false;
    //    UpdateThingToScan();
    //}

    //private void EnterScanSucceeded()
    //{
    //    _placeholderScanAttemptingVisual.enabled = false;
    //    _placeholderScanSuccessVisual.enabled = true;
    //}

    //private void UpdateScanSucceeded()
    //{
    //    _placeholderScanSuccessVisual.SetPosition(0, transform.position);
    //    _placeholderScanSuccessVisual.SetPosition(1, _toScan.transform.position);
    //}

    //private void ExitScanSucceeded()
    //{
    //    // it can be null if it was destroyed (e.g. item picked up)
    //    // during the time when it's successfully scanning it.
    //    if (_toScan != null)
    //    {
    //        _scannableLocator.MarkAsScanned(_toScan);

    //        PickupItem item = _toScan as PickupItem;
    //        Jellies.Parameters jelly = _toScan as Jellies.Parameters;
    //        if (item != null)
    //        {
    //            _scannedItemEvent.Raise(item.ItemInfo);
    //        }
    //        else if (jelly != null)
    //        {
    //            _scannedJellyEvent.Raise(jelly.TypeOfThisJelly());
    //        }
    //        else
    //        {
    //            Debug.LogError("log for this type isnt implemented (are we scanning" +
    //                " another thing besides jellies and items?", _toScan);
    //        }
    //    }

    //    _placeholderScanSuccessVisual.enabled = false;
    //    UpdateThingToScan();
    //}

    private void EnterTutorial()
    {
        // This & exit will happen multiple times if the drone moves to be sufficiently close to the player
        // and then shows the current tutorial step's pictogram.
        _pictogramBehaviour.SetImageActive(true);
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
                if (!_playerPressedWASD)
                {
                    _pictogramBehaviour.ChangePictogramImage(_droneMovementControls);
                }
                else
                {
                    _pictogramBehaviour.ChangePictogramImage(_droneTutorialComplete);
                    _droneTutorialTimer += Time.deltaTime;

                    if (_droneTutorialTimer >= 2f) // If 2 seconds have passed
                    {
                        _stateMachineInstance.SetBool(_finishedTutorialParameter, true);
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
            _playerPressedWASD = true;
        }
    }



    public FiniteStateMachineInstance GetStateMachineInstance()
    {
        if (_stateMachineInstance == null)
        {
            // this script's Awake() hasn't run yet.
            _stateMachineInstance = new FiniteStateMachineInstance(_stateMachineDefinition, this, _logStateMachineTransitions);
        }
        return _stateMachineInstance;
    }
}

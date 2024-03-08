using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

public class DroneScanning : MonoBehaviour
{
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _movement;

    [SerializeField, Tooltip("Whether to require the player to press a button to finish scanning.")]
    private bool _buttonPressScanning = true;
    [SerializeField, Tooltip("Max distance from the drone to scan things")]
    private float _detectionRange = 10f;
    [SerializeField, Tooltip("Max distance from the player things can be and still will be scanned")]
    private float _maxDistanceFromPlayerToScan = 10f;

    [Header("Visuals")]
    [SerializeField]
    private GameObject _scanButtonUI;
    [SerializeField]
    private LineRenderer _placeholderScanAttemptingVisual;
    [SerializeField]
    private LineRenderer _placeholderScanSuccessVisual;

    [Header("Finite State Machine Parameters")]
    [SerializeField]
    private FSMParameter _distanceFromHoverbyUnscannedParameter;
    [SerializeField]
    private FSMParameter _distanceBetweenUnscannedAndPlayerParameter;
    [SerializeField]
    private FSMParameter _seesUnscannedParameter;
    [SerializeField]
    private FSMParameter _scanButtonPressedParameter;
    [SerializeField]
    private FSMParameter _buttonPressScanningParameter;

    [Header("Finite State Machine Events")]
    [SerializeField]
    private GameEventScriptableObject _moveToUnscannedUpdate;
    [SerializeField]
    private GameEventScriptableObject _scanAttemptingEnter;
    [SerializeField]
    private GameEventScriptableObject _scanAttemptingUpdate;
    [SerializeField]
    private GameEventScriptableObject _scanAwaitingButtonEnter;
    [SerializeField]
    private GameEventScriptableObject _scanAwaitingButtonUpdate;
    [SerializeField]
    private GameEventScriptableObject _scanAwaitingButtonExit;
    [SerializeField]
    private GameEventScriptableObject _scanFailedExit;
    [SerializeField]
    private GameEventScriptableObject _scanSucceededEnter;
    [SerializeField]
    private GameEventScriptableObject _scanSucceededUpdate;
    [SerializeField]
    private GameEventScriptableObject _scanSucceededExit;

    [Header("Events for When Scanned Something")]
    [SerializeField]
    private GameEventScriptableObject _scannedItemEvent;
    [SerializeField]
    private GameEventScriptableObject _scannedJellyEvent;

    private FiniteStateMachineInstance _stateMachineInstance;

    private Transform _player;

    private MonoBehaviour _toScan;
    private DroneScannableLocator _scannableLocator;

    private GameEventResponses _gameEventResponses = new();


    private void Awake()
    {
        _stateMachineInstance = _drone.GetStateMachineInstance();
        _scannableLocator = new DroneScannableLocator();

        _player = Player.Motion.PlayerMovement.Instance.transform;

        _stateMachineInstance.SetBool(_buttonPressScanningParameter, _buttonPressScanning);

        _gameEventResponses.SetResponses(
            (_moveToUnscannedUpdate, UpdateMoveTowardsToScan)
            , (_scanAttemptingEnter, EnterScanAttempting)
            , (_scanAttemptingUpdate, UpdateScanAttempting)
            , (_scanFailedExit, ExitScanFailed)
            , (_scanSucceededEnter, EnterScanSucceeded)
            , (_scanSucceededUpdate, UpdateScanSucceeded)
            , (_scanSucceededExit, ExitScanSucceeded)
            , (_scanAwaitingButtonEnter, EnterScanAwaitingButton)
            , (_scanAwaitingButtonUpdate, UpdateScanAwaitingButton)
            , (_scanAwaitingButtonExit, ExitScanAwaitingButton)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();



    public void OnPreStateMachineInstanceUpdate()
    {
        TrackThingToScan();
    }

    private void TrackThingToScan()
    {
        if (_toScan == null || _scannableLocator.AlreadyScanned(_toScan))
        {
            _toScan = _scannableLocator.TryGetThingToScan(transform, _player
                , _detectionRange, _maxDistanceFromPlayerToScan);
        }

        float distanceToThingToScan = float.PositiveInfinity;
        float distanceBetweenThingToScanAndPlayer = float.PositiveInfinity;
        if (_toScan != null)
        {
            distanceToThingToScan = _movement.FromDroneToNear(_toScan.transform, backAwayIfTooClose: false).magnitude;
            distanceBetweenThingToScanAndPlayer = (_toScan.transform.position - _player.position).magnitude;
        }

        _stateMachineInstance.SetFloat(_distanceFromHoverbyUnscannedParameter, distanceToThingToScan);
        _stateMachineInstance.SetFloat(_distanceBetweenUnscannedAndPlayerParameter, distanceBetweenThingToScanAndPlayer);
        _stateMachineInstance.SetBool(_seesUnscannedParameter, _toScan != null);
    }


    private void UpdateMoveTowardsToScan()
    {
        _movement.RotateTowardsTarget(_toScan.transform);
        _movement.MoveDrone(_movement.FromDroneToNear(_toScan.transform, backAwayIfTooClose: false));
    }

    private void EnterScanAttempting()
    {
        _placeholderScanAttemptingVisual.enabled = true;
    }

    private void UpdateScanAttempting()
    {
        _placeholderScanAttemptingVisual.SetPosition(0, transform.position);
        _placeholderScanAttemptingVisual.SetPosition(1, _toScan.transform.position);
    }

    private void EnterScanAwaitingButton()
    {
        _scanButtonUI.SetActive(true);
    }

    private void UpdateScanAwaitingButton()
    {
        _placeholderScanAttemptingVisual.SetPosition(0, transform.position);
        _placeholderScanAttemptingVisual.SetPosition(1, _toScan.transform.position);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            _stateMachineInstance.SetBool(_scanButtonPressedParameter, true);
        }
    }

    private void ExitScanAwaitingButton()
    {
        _scanButtonUI.SetActive(false);
        _stateMachineInstance.SetBool(_scanButtonPressedParameter, false);
    }

    private void ExitScanFailed()
    {
        _toScan = null;
        _placeholderScanAttemptingVisual.enabled = false;
        TrackThingToScan();
    }

    private void EnterScanSucceeded()
    {
        _placeholderScanAttemptingVisual.enabled = false;
        _placeholderScanSuccessVisual.enabled = true;
    }

    private void UpdateScanSucceeded()
    {
        _placeholderScanSuccessVisual.SetPosition(0, transform.position);
        _placeholderScanSuccessVisual.SetPosition(1, _toScan.transform.position);
    }

    private void ExitScanSucceeded()
    {
        // it can be null if it was destroyed (e.g. item picked up)
        // during the time when it's successfully scanning it.
        if (_toScan != null)
        {
            _scannableLocator.MarkAsScanned(_toScan);

            PickupItem item = _toScan as PickupItem;
            Jellies.Parameters jelly = _toScan as Jellies.Parameters;
            if (item != null)
            {
                _scannedItemEvent.Raise(item.ItemInfo);
            }
            else if (jelly != null)
            {
                _scannedJellyEvent.Raise(jelly.TypeOfThisJelly());
            }
            else
            {
                Debug.LogError("log for this type isnt implemented (are we scanning" +
                    " another thing besides jellies and items?", _toScan);
            }
        }

        _placeholderScanSuccessVisual.enabled = false;
        TrackThingToScan();
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Tutorial_Movement03_VisionTest : MonoBehaviour
{
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _movement;
    [SerializeField]
    private CinemachineVirtualCamera _playerCamera;
    [Tooltip("This is the camera we switch to when the tutorial starts, as it needs some different settings than the regular player camera.")]
    [SerializeField]
    private CinemachineVirtualCamera _tutorialCamera;


    [Header("Tutorial Options")]
    [Tooltip("When the player first moves the mouse, this much time will elapse before the tutorial advances. If the value is 0, it would advance instantly when the player first moves the camera, which would be odd.")]
    [SerializeField, Min(0f)]
    private float _VisionTestSuccessDelay = 2.0f;

    [Header("Drone Pictogram")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
    [SerializeField]
    private Sprite _sprite_DroneCameraControls;


    [Header("Player Input Actions")]
    [SerializeField]
    InputActionReference _cameraLookAction;


    [Header("Finite State Machine Parameters")]
    [Tooltip("This parameter tracks what step the tutorial is currently in.")]
    [SerializeField]
    private FSMParameter _triggerForNextPartOfTutorial;

    [Header("Tutorial State Machine Events - Stage 03 - Vision Test")]
    [SerializeField]
    private GameEventScriptableObject _Enter;
    [SerializeField]
    private GameEventScriptableObject _Update;
    [SerializeField]
    private GameEventScriptableObject _Exit;

    [Header("Events for Tutorial Detecting Controls")]
    [SerializeField]
    private GameEventScriptableObject _playerLookControlsEvent;



    private FiniteStateMachineInstance _stateMachineInstance;

    private Transform _player;


    private bool _visionTestPassed;


    private GameEventResponses _gameEventResponses = new();

    private float _firstCameraMoveTime;



    private void Awake()
    {
        _stateMachineInstance = _drone.GetStateMachineInstance();

        _player = Player.Motion.PlayerMovement.Instance.transform;

        _gameEventResponses.SetResponses(
            (_playerLookControlsEvent, PlayerMovedCamera),
            (_Enter, EnterTutorial),
            (_Update, UpdateTutorial),
            (_Exit, ExitTutorial)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    public void PlayerMovedCamera()
    {
        if (_visionTestPassed)
        {
            return;
        }

        _visionTestPassed = true;
        _firstCameraMoveTime = Time.time;
    }


    private void EnterTutorial()
    {
        // Switch back to the player camera. This is needed as this is the one that is moved by the look controls.
        _playerCamera.MoveToTopOfPrioritySubqueue();

        _cameraLookAction.action.Enable();

        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneCameraControls);
    }

    private void UpdateTutorial()
    {
        if (_visionTestPassed && 
            Time.time - _firstCameraMoveTime >= _VisionTestSuccessDelay)
        {
            _stateMachineInstance.SetTrigger(_triggerForNextPartOfTutorial);
        }
    }

    private void ExitTutorial()
    {
        // Switch back to the tutorial camera.
        _tutorialCamera.MoveToTopOfPrioritySubqueue();
        _tutorialCamera.LookAt = _drone.transform;
    }

}

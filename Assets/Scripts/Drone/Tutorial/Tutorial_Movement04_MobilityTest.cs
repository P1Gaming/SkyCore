using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;

public class Tutorial_Movement04_MobilityTest : MonoBehaviour
{
    [Header("Tutorial Options")]
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
    [Tooltip("This specifies whether or not the player must press all four movement keys to pass the mobility test, or just any one of them.")]
    [SerializeField]
    private bool _RequireAll4MovementKeysPressed = true;
    [Tooltip("This is how long the drone will display the tutorial complete image.")]
    [SerializeField, Min(0f)]
    private float _droneTutorialCompleteDisplayDuration = 3f;

    [Header("Drone Pictogram")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
    [SerializeField]
    private Sprite _sprite_DroneMovementControls;
    [SerializeField]
    private Sprite _sprite_DroneTutorialComplete;

    [Header("Player Input Actions")]
    [SerializeField]
    InputActionReference _cameraLookAction;
    [SerializeField]
    InputActionReference _playerMovementAction;


    [Header("Finite State Machine Parameters")]
    [Tooltip("This parameter tracks what step the tutorial is currently in.")]
    [SerializeField]
    private FSMParameter _triggerForNextPartOfTutorial;
    [SerializeField]
    private FSMParameter _finishedMovementTutorial;

    [Header("Tutorial State Machine Events - Stage 04 - Mobility Test")]
    [SerializeField]
    private GameEventScriptableObject _Enter;
    [SerializeField]
    private GameEventScriptableObject _Update;
    [SerializeField]
    private GameEventScriptableObject _Exit;


    [Header("Events for Tutorial Detecting Controls")]
    [SerializeField]
    private GameEventScriptableObject _playerMovementWEvent;
    [SerializeField]
    private GameEventScriptableObject _playerMovementSEvent;
    [SerializeField]
    private GameEventScriptableObject _playerMovementAEvent;
    [SerializeField]
    private GameEventScriptableObject _playerMovementDEvent;



    private FiniteStateMachineInstance _stateMachineInstance;

    private Transform _player;

    // This variable uses a flags enum, so we can track all four buttons separately with a single variable.
    private MovementKeysPressed _keysPressed;

    private bool _mobilityTestPassed;

    private GameEventResponses _gameEventResponses = new();

    private bool _coroutineIsRunning;



    [Flags]
    private enum MovementKeysPressed
    {
        W = 1, 
        A = 2, 
        S = 4, 
        D = 8,
        All = 15,
    }



    private void Awake()
    {
        _stateMachineInstance = _drone.GetStateMachineInstance();

        _player = Player.Motion.PlayerMovement.Instance.transform;

        // Clear the variable that tracks which keys have been pressed.
        _keysPressed = 0f;

        _gameEventResponses.SetResponses(
            // Player controls events
            (_playerMovementWEvent, PlayerMovedW),
            (_playerMovementSEvent, PlayerMovedS),
            (_playerMovementAEvent, PlayerMovedA),
            (_playerMovementDEvent, PlayerMovedD),
            (_Enter, EnterTutorial),
            (_Update, UpdateTutorial),
            (_Exit, ExitTutorial)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    public void PlayerMovedW()
    {
        _keysPressed |= MovementKeysPressed.W;
    }
    public void PlayerMovedA()
    {
        _keysPressed |= MovementKeysPressed.A;
    }
    public void PlayerMovedS()
    {
        _keysPressed |= MovementKeysPressed.S;
    }
    public void PlayerMovedD()
    {
        _keysPressed |= MovementKeysPressed.D;
    }

    private void EnterTutorial()
    {
        _coroutineIsRunning = false;
        _mobilityTestPassed = false;

        // Switch back to the player camera. This is needed as this is the one that is moved by the look controls.
        _playerCamera.MoveToTopOfPrioritySubqueue();
        
        _playerMovementAction.action.Enable();

        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneMovementControls);
    }

    private void UpdateTutorial()
    {
        if (_mobilityTestPassed && !_coroutineIsRunning)
        {
            _pictogramBehaviour.ChangePictogramImage(_sprite_DroneTutorialComplete);

            StartCoroutine(WaitForTutorialDonePictureDisplayTimeToEnd());
        }
        else
        {
            CheckIfPlayerHasPassedMobilityTest();
        }
    }

    private void ExitTutorial()
    {
        // Switch back to the normal player camera.
        _playerCamera.MoveToTopOfPrioritySubqueue();

        _cameraLookAction.action.Enable();
    }

    private void CheckIfPlayerHasPassedMobilityTest()
    {
        if (_RequireAll4MovementKeysPressed &&        
            _keysPressed == MovementKeysPressed.All)
        {
            _mobilityTestPassed = true;
        }
        else if (!_RequireAll4MovementKeysPressed &&
                 _keysPressed > 0)
        {
            // _requireAll4MovementKeysPressed is false, so as long as at least one key has been pressed we can return true.
            _mobilityTestPassed = true;
        }
        
    }

    private IEnumerator WaitForTutorialDonePictureDisplayTimeToEnd()
    {
        _coroutineIsRunning = true;

        float elapsedTime = 0f;
        while (elapsedTime <= _droneTutorialCompleteDisplayDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }


        _stateMachineInstance.SetTrigger(_triggerForNextPartOfTutorial);
        _stateMachineInstance.SetBool(_finishedMovementTutorial, true);

        _pictogramBehaviour.SetImageActive(false);

        _coroutineIsRunning = false;
    }


}

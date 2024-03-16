using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;
using Player.View;

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
    [Tooltip("When the player has passed the mobility test, this much time (in seconds) will elapse before the tutorial advances.")]
    [SerializeField, Min(0f)]
    private float _MobilityTestSuccessDelay = 2.0f;

    [Header("Drone Pictogram")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
    [SerializeField]
    private Sprite _sprite_DroneMovementControls;
    [SerializeField]
    private Sprite _sprite_DroneSuccess;


    [Header("Player Input Actions")]
    [SerializeField]
    InputActionReference _cameraLookAction;
    [SerializeField]
    InputActionReference _playerMovementAction;


    [Header("Finite State Machine Parameters")]
    [Tooltip("This parameter tracks what step the tutorial is currently in.")]
    [SerializeField]
    private FSMParameter _triggerForNextPartOfTutorial;

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

    // This variable uses a flags enum, so we can track all four buttons separately with a single variable.
    private MovementKeysPressed _keysPressed;

    private bool _mobilityTestPassed;

    private GameEventResponses _gameEventResponses = new();

    private bool _waitingForTutorialDronePictureTimeToEnd;

    private bool _playerLookedAtDrone;



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
        _stateMachineInstance = _drone.GetActionStateMachineInstance();

        // Clear the variable that tracks which keys have been pressed.
        _keysPressed = 0f;

        _gameEventResponses.SetResponses(
            // Player controls events
            (_Enter, EnterTutorial),
            (_Update, UpdateTutorial),
            (_Exit, ExitTutorial)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();


    private void EnterTutorial()
    {
        _waitingForTutorialDronePictureTimeToEnd = false;
        _mobilityTestPassed = false;

        // Switch back to the player camera. This is needed as this is the one that is moved by the look controls.
        _playerCamera.MoveToTopOfPrioritySubqueue();


        StartCoroutine(WaitForPlayerToLookAtDrone());
    }

    private void UpdateTutorial()
    {
        if (_playerLookedAtDrone && _mobilityTestPassed && !_waitingForTutorialDronePictureTimeToEnd)
        {
            StartCoroutine(WaitForTutorialDonePictureDisplayTimeToEnd());
        }
        else
        {
            CheckPlayerInput();
            CheckIfPlayerHasPassedMobilityTest();
        }
    }

    private void ExitTutorial()
    {
        // Switch back to the normal player camera.
        _playerCamera.MoveToTopOfPrioritySubqueue();
    }
   
    private void CheckPlayerInput()
    {
        Vector2 direction = _playerMovementAction.action.ReadValue<Vector2>();
        if (direction.x < 0)
        {
            _keysPressed |= MovementKeysPressed.A;
        }
        else if (direction.x > 0)
        {
            _keysPressed |= MovementKeysPressed.D;
        }
        if (direction.y < 0)
        {
            _keysPressed |= MovementKeysPressed.S;
        }
        else if (direction.y > 0)
        {
            _keysPressed |= MovementKeysPressed.W;
        }
    }

    private IEnumerator WaitForPlayerToLookAtDrone()
    {
        _cameraLookAction.action.Enable();


        yield return new WaitForSeconds(1.0f);

        HUD_Manager.Instance.EnableDroneIndicatorIcon(true);


        while (!Camera.main.IsObjectInFrontOfCamera(_drone.gameObject))
        {
            yield return null;
        }


        HUD_Manager.Instance.EnableDroneIndicatorIcon(false);

        _cameraLookAction.action.Disable();
        _tutorialCamera.MoveToTopOfPrioritySubqueue();
        _playerLookedAtDrone = true;

        yield return new WaitForSeconds(1.0f);

        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneMovementControls);
        _playerMovementAction.action.Enable();
    }

    private void CheckIfPlayerHasPassedMobilityTest()
    {
        if (_RequireAll4MovementKeysPressed &&        
            _keysPressed == MovementKeysPressed.All)
        {
            _mobilityTestPassed = true;

            _playerMovementAction.action.Disable();
        }
        else if (!_RequireAll4MovementKeysPressed &&
                 _keysPressed > 0)
        {
            // _requireAll4MovementKeysPressed is false, so as long as at least one key has been pressed we can return true.
            _mobilityTestPassed = true;

            _playerMovementAction.action.Disable();
        }

    }

    private IEnumerator WaitForTutorialDonePictureDisplayTimeToEnd()
    {
        _waitingForTutorialDronePictureTimeToEnd = true;

        // Display the success image.
        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneSuccess);
        yield return new WaitForSeconds(_MobilityTestSuccessDelay);

        // Trigger the next part of the tutorial.
        _stateMachineInstance.SetTrigger(_triggerForNextPartOfTutorial);


        // Switch back to the normal player cam.
        _waitingForTutorialDronePictureTimeToEnd = false;
    }


}

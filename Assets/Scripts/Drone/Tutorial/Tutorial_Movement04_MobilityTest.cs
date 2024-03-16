using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using UnityEngine.InputSystem;
using Cinemachine;
using System;
using Player.View;
using Player.Motion;

public class Tutorial_Movement04_MobilityTest : MonoBehaviour
{
    [Header("Tutorial Options")]
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _movement;

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
        CameraSystem.SwitchToFirstPersonCamera();


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
        CameraSystem.SwitchToFirstPersonCamera();
    }
   
    private void CheckPlayerInput()
    {
        Player.Motion.PlayerMovement.Instance.GetWASDInputAxes(out float rightLeft, out float forwardsBackwards);
        if (rightLeft < 0)
        {
            _keysPressed |= MovementKeysPressed.A;
        }
        else if (rightLeft > 0)
        {
            _keysPressed |= MovementKeysPressed.D;
        }
        if (forwardsBackwards < 0)
        {
            _keysPressed |= MovementKeysPressed.S;
        }
        else if (forwardsBackwards > 0)
        {
            _keysPressed |= MovementKeysPressed.W;
        }
    }

    private IEnumerator WaitForPlayerToLookAtDrone()
    {
        FirstPersonView.Instance.NumberOfReasonsToIgnoreInputs--;


        yield return new WaitForSeconds(1.0f);

        HUD_Manager.Instance.EnableDroneIndicatorIcon(true);


        while (!Camera.main.IsObjectInFrontOfCamera(_drone.gameObject))
        {
            yield return null;
        }


        HUD_Manager.Instance.EnableDroneIndicatorIcon(false);

        FirstPersonView.Instance.NumberOfReasonsToIgnoreInputs++;
        CameraSystem.SwitchToTutorialCamera();
        _playerLookedAtDrone = true;

        yield return new WaitForSeconds(1.0f);

        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneMovementControls);
        FirstPersonView.Instance.NumberOfReasonsToIgnoreInputs--;
    }

    private void CheckIfPlayerHasPassedMobilityTest()
    {
        if (_RequireAll4MovementKeysPressed &&        
            _keysPressed == MovementKeysPressed.All)
        {
            _mobilityTestPassed = true;

            PlayerMovement.Instance.NumberOfReasonsToIgnoreWASDInputs++;
        }
        else if (!_RequireAll4MovementKeysPressed &&
                 _keysPressed > 0)
        {
            // _requireAll4MovementKeysPressed is false, so as long as at least one key has been pressed we can return true.
            _mobilityTestPassed = true;

            PlayerMovement.Instance.NumberOfReasonsToIgnoreWASDInputs++;
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

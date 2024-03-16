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


    [Header("Tutorial Options")]
    [Tooltip("This specifies how much the player has to move their view horizontally (in degrees) in either direction to pass the vision test.")]
    [SerializeField, Min(0f)]
    private float _VisionTestRequiredMovementAmount = 360f;

    [Tooltip("When the player has passed the vision test, this much time (in seconds) will elapse before the tutorial advances.")]
    [SerializeField, Min(0f)]
    private float _VisionTestSuccessDelay = 2.0f;

    [Header("Drone Pictogram")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
    [SerializeField]
    private Sprite _sprite_DroneCameraControls;
    [SerializeField]
    private Sprite _sprite_DroneSuccess;


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


    private FiniteStateMachineInstance _stateMachineInstance;


    private bool _visionTestPassed;


    private GameEventResponses _gameEventResponses = new();



    private void Awake()
    {
        _stateMachineInstance = _drone.GetActionStateMachineInstance();

        _gameEventResponses.SetResponses(
            (_Enter, EnterTutorial),
            (_Update, UpdateTutorial),
            (_Exit, ExitTutorial)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();



    private void EnterTutorial()
    {
        // Switch back to the player camera. This is needed as this is the one that is moved by the look controls.
        CameraSystem.SwitchToFirstPersonCamera();

        _cameraLookAction.action.Enable();

        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneCameraControls);

        StartCoroutine(WaitForPlayerToRotateViewAFull360Degrees());
    }

    private void UpdateTutorial()
    {
        if (_visionTestPassed)
        {
            _stateMachineInstance.SetTrigger(_triggerForNextPartOfTutorial);
        }
    }

    private void ExitTutorial()
    {
        // Switch back to the tutorial camera.
        CameraSystem.SwitchToTutorialCamera();
        CameraSystem.TutorialCamera.LookAt = _drone.transform;
    }

    /// <summary>
    /// This coroutine waits until the player has rotated his view at least 360 degrees left or right.
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForPlayerToRotateViewAFull360Degrees()
    {
        float totalDegreesMoved = 0f;
        float lastFrameHorizLookAngle = Camera.main.transform.rotation.eulerAngles.y;


        while (Mathf.Abs(totalDegreesMoved) < _VisionTestRequiredMovementAmount)
        {
            float curFrameHorizLookAngle = Camera.main.transform.rotation.eulerAngles.y;

            // Calculate how much the angle of the camera changed since the last frame.
            float angleDelta = curFrameHorizLookAngle - lastFrameHorizLookAngle;

            // Check if the player's view angle crossed the 0/360 degree boundary during the last frame.
            // If so, it means the angle went from 0 to 359 or vice versa, so we have to correct for this or we'll get
            // erroneous results on frames where this occurs, which throws off the total angle delta we're tracking (totalDegreesMoved).
            if (Mathf.Abs(angleDelta) > 300f)
            {
                angleDelta = angleDelta > 0 ? angleDelta - 360
                                           : angleDelta + 360;
            }


            totalDegreesMoved += angleDelta;

            //Debug.Log($"CurFrameDelta: {angleDelta}    TotalDelta: {totalDegreesMoved}    CurFrame: {curFrameHorizLookAngle}    LastFrame: {lastFrameHorizLookAngle}");


            // Cache the current look angle so we can use it on the next frame.
            lastFrameHorizLookAngle = curFrameHorizLookAngle;


            // Wait for the next frame.
            yield return null;

        } // end while


        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneSuccess);

        yield return new WaitForSeconds(_VisionTestSuccessDelay);

        _visionTestPassed = true;
    }

}

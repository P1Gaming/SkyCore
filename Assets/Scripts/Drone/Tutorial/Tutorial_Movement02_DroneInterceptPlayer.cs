using Cinemachine;
using FiniteStateMachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tutorial_Movement02_DroneInterceptPlayer : MonoBehaviour
{
    [Header("Tutorial Options")]
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _movement;
    [Tooltip("This is the camera we switch to when the tutorial starts, as it needs some different settings than the regular player camera.")]
    [SerializeField]
    private CinemachineVirtualCamera _tutorialCamera;


    [Header("Tutorial Settings")]
    [SerializeField]
    private float _droneDistanceToStopInFrontOfPlayer;
    [Tooltip("After the cutscene has ended, this is the delay between when the drone starts moving toward the player and when the camera starts to turn to look at the drone.")]
    [SerializeField, Min(0)]
    private float _cameraLookAtDroneDelay = 1f;
    [Tooltip("This sets how long it takes the drone to perform medical assistance when it intercepts the player right after the cutscene plays.")]
    [SerializeField, Min(0)]
    private float _medicalAssistanceDuration = 3f;

    [Header("Drone Pictogram")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;
    [SerializeField]
    private Sprite _sprite_DroneMedicalAssistance;

    [Header("Finite State Machine Parameters")]
    [Tooltip("This parameter tracks what step the tutorial is currently in.")]
    [SerializeField]
    private FSMParameter _triggerForNextPartOfTutorial;

    [Header("Tutorial State Machine Events - Stage 02 - Drone Intercepts Player")]
    [SerializeField]
    private GameEventScriptableObject _Enter;
    [SerializeField]
    private GameEventScriptableObject _Update;
    [SerializeField]
    private GameEventScriptableObject _Exit;



    private FiniteStateMachineInstance _stateMachineInstance;

    private Transform _player;


    private GameEventResponses _gameEventResponses = new();

    private bool _startedMedicalAssistance;
    private bool _medicalAssistanceComplete;



    private void Awake()
    {
        _stateMachineInstance = _drone.GetStateMachineInstance();

        _player = Player.Motion.PlayerMovement.Instance.transform;

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
        // Switch to the tutorial camera.
        _tutorialCamera.MoveToTopOfPrioritySubqueue();

        _medicalAssistanceComplete = false;
        _startedMedicalAssistance = false;

        _pictogramBehaviour.ChangePictogramImage(_sprite_DroneMedicalAssistance);

        StartCoroutine(WaitToFocusCamera());
    }

    private void UpdateTutorial()
    {
        // Make the drone move into view until it is right in front of the player.
        if (!_startedMedicalAssistance &&
            _movement.MoveDroneInFrontOfPlayer(_droneDistanceToStopInFrontOfPlayer, true))
        {
            // The drone has reached the player, so display the health image.
            _pictogramBehaviour.SetImageActive(true);

            _startedMedicalAssistance = true;

            StartCoroutine(WaitForMedicalAssistanceToFinish());
        }


        if (_medicalAssistanceComplete)
        {
            // Trigger transition to next part of the tutorial.
            _stateMachineInstance.SetTrigger(_triggerForNextPartOfTutorial);
        }

    }

    private IEnumerator WaitForMedicalAssistanceToFinish()
    {
        float elapsedTime = 0f;
        while (elapsedTime <= _medicalAssistanceDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _medicalAssistanceComplete = true;
    }

    private IEnumerator WaitToFocusCamera()
    {
        float elapsedTime = 0f;
        while (elapsedTime <= _cameraLookAtDroneDelay)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _tutorialCamera.LookAt = _drone.transform;
    }

    private void ExitTutorial()
    {
        _tutorialCamera.LookAt = null;
    }

   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Tutorial_Movement01_PlayCutScene : MonoBehaviour
{
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _movement;


    [Header("Tutorial Options")]
    [SerializeField]
    private bool _skipTutorial;
    [SerializeField]
    private float _droneDistanceToStopInFrontOfPlayer;
    [Tooltip("This specifies how long of a delay between the end of the cutscene and when the drone flies onto the screen to help the player.")]
    [SerializeField, Min(0)]
    private float _droneInterceptDelay = 3f;
    [Tooltip("After the cutscene has ended, this is the delay between when the drone starts moving toward the player and when the camera starts to turn to look at the drone.")]


    [Header("Drone Pictogram")]
    [SerializeField]
    private PictogramBehavior _pictogramBehaviour;

    [Header("Player Input Actions")]
    [SerializeField]
    InputActionReference _playerMovementAction;
    [SerializeField]
    InputActionReference _cameraLookAction;


    [Header("Finite State Machine Parameters")]
    [Tooltip("This parameter tracks what step the tutorial is currently in.")]
    [SerializeField]
    private FSMParameter _tutorialProgressParameter;
    [SerializeField]
    private FSMParameter _finishedTutorialParameter;


    [Header("Tutorial State Machine Events - Stage 01 - Play CutScene")]
    [SerializeField]
    private GameEventScriptableObject _tutorialStage01_Enter;
    [SerializeField]
    private GameEventScriptableObject _tutorialStage01_Update;
    [SerializeField]
    private GameEventScriptableObject _tutorialStage01_Exit;



    private FiniteStateMachineInstance _stateMachineInstance;

    private Transform _player;


    private GameEventResponses _gameEventResponses = new();

    private float _timer;



    private void Awake()
    {
        _stateMachineInstance = _drone.GetStateMachineInstance();
        _stateMachineInstance.SetBool(_finishedTutorialParameter, _skipTutorial);

        _pictogramBehaviour.SetImageActive(false);

        // Skip the rest of this function if _skipTutorial is enabled.
        if (_skipTutorial)
            return;


        //_stateMachineInstance.LogTransitions = true;

        _player = Player.Motion.PlayerMovement.Instance.transform;

        _gameEventResponses.SetResponses(
            (_tutorialStage01_Enter, EnterTutorial),
            (_tutorialStage01_Update, UpdateTutorial)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();


   
    private void EnterTutorial()
    {
        _timer = 0f;

        _cameraLookAction.action.Disable();
        _playerMovementAction.action.Disable();

        // Trigger cutscene.
        // CALL CUTSCENEPLAYER HERE WHEN ITS DONE.
    }

    private void UpdateTutorial()
    {
        _timer += Time.deltaTime;
        if (_timer >= _droneInterceptDelay)
        {
            _stateMachineInstance.SetFloat(_tutorialProgressParameter, 1); // Trigger transition to next part of the tutorial.
        }
    }
   
}

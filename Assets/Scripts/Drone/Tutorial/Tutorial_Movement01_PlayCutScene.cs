using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using UnityEngine.InputSystem;
using Cinemachine;

using Player.View;


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
    private bool _skipCutscene;

    [SerializeField]
    private float _droneDistanceToStopInFrontOfPlayer;
    [Tooltip("This specifies how long of a delay there is between the player looking at the drone and when it flies over to help the player.")]
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
    InputActionReference _playerJumpAction;
    [SerializeField]
    InputActionReference _cameraLookAction;


    [Header("Finite State Machine Parameters")]
    [Tooltip("This parameter tracks what step the tutorial is currently in.")]
    [SerializeField]
    private FSMParameter _triggerForNextPartOfTutorial;
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
    private CutScenePlayer _cutScenePlayer;

    private bool _cutSceneHasFinished;
    private bool _playerLookedAtDrone;
    private MeshRenderer _droneRenderer;

    private GameEventResponses _gameEventResponses = new();

    private float _timer;



    private void Awake()
    {
        _cutScenePlayer = FindObjectOfType<CutScenePlayer>();

        _stateMachineInstance = _drone.GetActionStateMachineInstance();
        _stateMachineInstance.SetBool(_finishedTutorialParameter, _skipTutorial);

        _pictogramBehaviour.SetImageActive(false);

        // Skip the rest of this function if _skipTutorial is enabled.
        if (_skipTutorial && _skipCutscene)
        {
            _stateMachineInstance.SetBool(_finishedTutorialParameter, true);
            return;
        }

        //_stateMachineInstance.LogTransitions = true;

        _droneRenderer = _drone.GetComponentInChildren<MeshRenderer>();

        _gameEventResponses.SetResponses(
            (_tutorialStage01_Enter, EnterTutorial),
            (_tutorialStage01_Update, UpdateTutorial)
            );


        if (!_skipTutorial || !_skipCutscene)
        {
            _playerMovementAction.action.Disable();
            _playerJumpAction.action.Disable();
        }
    }


    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();


   
    private void EnterTutorial()
    {
        _timer = 0f;


        if (!_skipCutscene)
        {
            // Trigger cutscene.
            _cutScenePlayer.OnCutSceneStopped += OnCutSceneEnded;
            _cutScenePlayer.PlayCutScene("TestCutScene"); // There is an overload of this function that lets you pass in the index of the cutscene in the CutScenePlayer's list if you have multiple in its list.            
        }
        else
        {
            _cutSceneHasFinished = true;

            if (!_skipTutorial)
            {
                // Call the cut scene ended event handler to trigger the code that normally runs when the cut scene has ended.
                OnCutSceneEnded(this, new CutScenePlayerEventArgs() { CutScene = null });
            }
        }

    }

    private void UpdateTutorial()
    {
        if (_cutSceneHasFinished && _playerLookedAtDrone)
        {
            _timer += Time.deltaTime;

            if (_timer >= _droneInterceptDelay)
            {
                _stateMachineInstance.SetTrigger(_triggerForNextPartOfTutorial); // Trigger transition to next part of the tutorial.

                FirstPersonView.Instance.ClampHorizontalAngle = false;
                _cameraLookAction.action.Disable();
            }
        }

    }
   
    private void OnCutSceneEnded(object sender, CutScenePlayerEventArgs e)
    {
        _cutSceneHasFinished = true;
      
        _cutScenePlayer.OnCutSceneStopped -= OnCutSceneEnded;


        if (_skipTutorial)
        {
            _stateMachineInstance.SetBool(_finishedTutorialParameter, true);

            _cameraLookAction.action.Enable();
            _playerMovementAction.action.Enable();
            _playerJumpAction.action.Enable();

            return;
        }

        StartCoroutine(WaitForPlayerToLookAtDrone());
    }

    private IEnumerator WaitForPlayerToLookAtDrone()
    {
        _cameraLookAction.action.Enable();


        // Limit player look to 90 degrees left or right.
        FirstPersonView.Instance.HorizontalClampingAngle = 90f;
        FirstPersonView.Instance.ClampHorizontalAngle = true;


        yield return new WaitForSeconds(1.0f);

        HUD_Manager.Instance.EnableDroneIndicatorIcon(true);


        while (!Camera.main.IsObjectInFrontOfCamera(_drone.gameObject))
        {
            yield return null;
        }


        HUD_Manager.Instance.EnableDroneIndicatorIcon(false);

        _cameraLookAction.action.Disable();
        _playerLookedAtDrone = true;
    }


}

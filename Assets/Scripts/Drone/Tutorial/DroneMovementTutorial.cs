using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

public class DroneMovementTutorial : MonoBehaviour
{
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _movement;
    [SerializeField]
    private bool _skipTutorial;
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
    private FSMParameter _finishedTutorialParameterInActionFSM;
    [SerializeField]
    private FSMParameter _finishedTutorialParameterInMovementFSM;

    [Header("Finite State Machine Events")]
    [SerializeField]
    private GameEventScriptableObject _tutorialEnter;
    [SerializeField]
    private GameEventScriptableObject _tutorialUpdate;
    [SerializeField]
    private GameEventScriptableObject _tutorialExit;

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

    private FiniteStateMachineInstance _actionStateMachineInstance;
    private FiniteStateMachineInstance _movementStateMachineInstance;

    private Transform _player;

    private bool _movedCamera = false;
    private bool _playerPressedWASD = false;
    private bool _medicalPromptShown = false;
    private float _droneTutorialTimer;
    private GameEventResponses _gameEventResponses = new();


    private void Awake()
    {
        _actionStateMachineInstance = _drone.GetActionStateMachineInstance();
        _movementStateMachineInstance = _drone.GetMovementStateMachineInstance();
        _player = Player.Motion.PlayerMovement.Instance.transform;
        _actionStateMachineInstance.SetBool(_finishedTutorialParameterInActionFSM, _skipTutorial);
        _movementStateMachineInstance.SetBool(_finishedTutorialParameterInMovementFSM, _skipTutorial);

        _gameEventResponses.SetResponses(
            (_playerLookControlsEvent, PlayerMovedCamera)
            , (_playerMovementWEvent, PlayerMoved)
            , (_playerMovementSEvent, PlayerMoved)
            , (_playerMovementAEvent, PlayerMoved)
            , (_playerMovementDEvent, PlayerMoved)
            , (_tutorialEnter, EnterTutorial)
            , (_tutorialUpdate, UpdateTutorial)
            , (_tutorialExit, ExitTutorial)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

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
        //_movement.RotateTowardsTarget(_player); // still might want to make the pictogram directly face the camera

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
                        _actionStateMachineInstance.SetBool(_finishedTutorialParameterInActionFSM, true);
                        _movementStateMachineInstance.SetBool(_finishedTutorialParameterInMovementFSM, true);
                    }
                }
            }
        }
    }


   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

public class Drone : MonoBehaviour
{
    [SerializeField]
    private bool _logMovementStateMachineTransitions;
    [SerializeField]
    private bool _logActionStateMachineTransitions;
    [SerializeField]
    private FSMDefinition _movementStateMachineDefinition;
    [SerializeField]
    private FSMDefinition _actionStateMachineDefinition;
    [SerializeField]
    private DroneMovement _movement;
    [SerializeField]
    private DroneFollowPlayer _followPlayer;
    [SerializeField]
    private DroneScanning _scanning;

    [Header("Communication Between the Action FSM and Movement FSM")]
    [SerializeField]
    private BoolTrueWhileInCertainFSMStates _tellMovementFSMWhetherToDoNothing;

    // Use two separate state machines so the drone can perform actions while it's moving,
    // without needing them to be fully synchronized, to help it feel more alive.
    // During actions which should not occur during movement, the action state machine sets a
    // parameter in the movement state machine true to tell it to do nothing.
    private FiniteStateMachineInstance _movementStateMachineInstance;
    private FiniteStateMachineInstance _actionStateMachineInstance;

    private void Awake()
    {
        CheckConstructMovementStateMachineInstance();
        CheckConstructActionStateMachineInstance();

        _tellMovementFSMWhetherToDoNothing.Initialize(_movementStateMachineInstance, null);
    }

    private void OnEnable()
    {
        _tellMovementFSMWhetherToDoNothing.Register();
    }

    private void OnDisable()
    {
        _tellMovementFSMWhetherToDoNothing.Unregister();
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0)
        {
            // this shouldn't happen, I think.
            Debug.LogError("I guess fixed update can run when timescale is 0. Weird.");
            return;
        }

        _followPlayer.OnPreStateMachineInstanceUpdate();
        _scanning.OnPreStateMachineInstanceUpdate();
        _movementStateMachineInstance.Update();
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        // These MonoBehaviours don't have their own Update() methods because this way, they always set parameters
        // immediately before _stateMachineInstance.Update() runs, for consistency. Less bug-prone and avoids 1 frame delays.
        _scanning.OnPreStateMachineInstanceUpdate();

        _actionStateMachineInstance.Update();
    }

    private void CheckConstructActionStateMachineInstance()
    {
        if (_actionStateMachineInstance == null)
        {
            _actionStateMachineInstance = new FiniteStateMachineInstance(_actionStateMachineDefinition
                , this, _logActionStateMachineTransitions);
        }
    }

    private void CheckConstructMovementStateMachineInstance()
    {
        if (_movementStateMachineInstance == null)
        {
            _movementStateMachineInstance = new FiniteStateMachineInstance(_movementStateMachineDefinition
                , this, _logMovementStateMachineTransitions);
        }
    }

    public FiniteStateMachineInstance GetActionStateMachineInstance()
    {
        CheckConstructActionStateMachineInstance();
        return _actionStateMachineInstance;
    }

    public FiniteStateMachineInstance GetMovementStateMachineInstance()
    {
        CheckConstructMovementStateMachineInstance();
        return _movementStateMachineInstance;
    }
}

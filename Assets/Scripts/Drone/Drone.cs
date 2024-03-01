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

    private FiniteStateMachineInstance _movementStateMachineInstance;
    private FiniteStateMachineInstance _actionStateMachineInstance;

    private void Awake()
    {
        CheckConstructMovementStateMachineInstance();
        CheckConstructActionStateMachineInstance();
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

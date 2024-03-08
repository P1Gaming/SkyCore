using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

public class Drone : MonoBehaviour
{
    [SerializeField]
    private bool _logStateMachineTransitions;
    [SerializeField]
    private FSMDefinition _stateMachineDefinition;
    [SerializeField]
    private DroneMovement _movement;
    [SerializeField]
    private DroneFollowPlayer _followPlayer;
    [SerializeField]
    private DroneScanning _scanning;

    private FiniteStateMachineInstance _stateMachineInstance;


    private void Awake()
    {
        CheckConstructStateMachineInstance();
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
        _followPlayer.OnPreStateMachineInstanceUpdate();

        _stateMachineInstance.Update();
    }

    private void CheckConstructStateMachineInstance()
    {
        if (_stateMachineInstance == null)
        {
            _stateMachineInstance = new FiniteStateMachineInstance(_stateMachineDefinition, this, _logStateMachineTransitions);
        }
    }

    public FiniteStateMachineInstance GetStateMachineInstance()
    {
        CheckConstructStateMachineInstance();
        return _stateMachineInstance;
    }


    public DroneMovement DroneMovement { get { return _movement; } }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

public class JellyStateMachine : MonoBehaviour
{
    [SerializeField]
    private FSMDefinition _stateMachineDefinition;
    [SerializeField]
    private FSMParameter _distanceToPlayerParameter;
    [SerializeField]
    private bool _logStateMachineTransitions;

    private FiniteStateMachineInstance _fsmInstance;
    private Transform _player;

    private void Awake()
    {
        _fsmInstance = new FiniteStateMachineInstance(_stateMachineDefinition, this, _logStateMachineTransitions);
        _player = Player.Motion.PlayerMovement.Instance.transform;
    }

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        float distanceToPlayer = (transform.position - _player.position).magnitude;
        _fsmInstance.SetFloat(_distanceToPlayerParameter, distanceToPlayer);
        _fsmInstance.Update();
    }
}

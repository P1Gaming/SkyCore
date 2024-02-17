using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Watching : MonoBehaviour
{
    [SerializeField]
    private NavMeshAgent _agent;
    [SerializeField]
    private JellyStateMachine _stateMachine;
    [SerializeField]
    private GameEventScriptableObject _updateWatching;
    [SerializeField]
    private GameEventScriptableObject _exitWatching;
    [SerializeField, Range(0, 10), Tooltip("The distance the jelly watches the subject from")]
    private int _watchDistance;

    private Transform _player;
    private GameEventResponses _gameEventResponses = new();

    private void Awake()
    {
        _player = Player.Motion.PlayerMovement.Instance.transform;

        _gameEventResponses.SetSelectiveResponses(_stateMachine
            , (_updateWatching, UpdateWatching)
            , (_exitWatching, ExitWatching)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    private void UpdateWatching()
    {
        _agent.stoppingDistance = _watchDistance;
        _agent.SetDestination(_player.position);
    }

    private void ExitWatching()
    {
        _agent.ResetPath();
    }
}

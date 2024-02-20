using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

public class DroneFollowPlayer : MonoBehaviour
{
    [SerializeField]
    private DroneStateMachine _drone;
    [SerializeField]
    private DroneMovement _movement;

    [SerializeField]
    private FSMParameter _distanceFromHoverbyPlayerParameter;

    [SerializeField]
    private GameEventScriptableObject _idleUpdate;
    [SerializeField]
    private GameEventScriptableObject _followPlayerUpdate;

    private FiniteStateMachineInstance _stateMachineInstance;
    private Transform _player;
    private GameEventResponses _gameEventResponses = new();


    private void Awake()
    {
        _stateMachineInstance = _drone.GetStateMachineInstance();
        _player = Player.Motion.PlayerMovement.Instance.transform;

        // There's only 1 drone, so don't need to use SetSelectiveResponses.
        _gameEventResponses.SetResponses(
            (_idleUpdate, UpdateIdle)
            , (_followPlayerUpdate, UpdateMoveTowardsPlayer)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    public void OnPreStateMachineInstanceUpdate()
    {
        float distance = _movement.FromDroneToNear(_player).magnitude;
        _stateMachineInstance.SetFloat(_distanceFromHoverbyPlayerParameter, distance);
    }

    private void UpdateIdle()
    {
        _movement.IdleMovement();
    }

    private void UpdateMoveTowardsPlayer()
    {
        _movement.RotateTowardsTarget(_player);
        _movement.MoveDrone(_movement.FromDroneToNear(_player));
    }
}

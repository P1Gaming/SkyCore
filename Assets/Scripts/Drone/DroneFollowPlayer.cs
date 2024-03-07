using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

public class DroneFollowPlayer : MonoBehaviour
{
    [SerializeField]
    private Drone _drone;
    [SerializeField]
    private DroneMovement _movement;

    [SerializeField]
    private FSMParameter _distanceFromHoverbyPlayerParameter;

    [SerializeField]
    private GameEventScriptableObject _nearPlayerUpdate;
    [SerializeField]
    private GameEventScriptableObject _followPlayerUpdate;
    [SerializeField]
    private GameEventScriptableObject _followPlayerExit;

    private FiniteStateMachineInstance _movementStateMachineInstance;
    private Transform _player;
    private GameEventResponses _gameEventResponses = new();


    private void Awake()
    {
        _movementStateMachineInstance = _drone.GetMovementStateMachineInstance();
        _player = Player.Motion.PlayerMovement.Instance.transform;

        // There's only 1 drone, so don't need to use SetSelectiveResponses.
        _gameEventResponses.SetResponses(
            (_nearPlayerUpdate, UpdateNearPlayer)
            , (_followPlayerExit, ExitFollowPlayer)
            , (_followPlayerUpdate, UpdateFollowPlayer)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    public void OnPreStateMachineInstanceUpdate()
    {
        float distance = _movement.FromDroneToNear(_player).magnitude;
        _movementStateMachineInstance.SetFloat(_distanceFromHoverbyPlayerParameter, distance);
    }

    private void UpdateNearPlayer()
    {
        _movement.IdleMovement();
    }

    private void UpdateFollowPlayer()
    {
        _movement.RotateTowardsTarget(_player);
        _movement.MoveDrone(_movement.FromDroneToNear(_player));
    }

    private void ExitFollowPlayer()
    {
        _movement.StopVelocity();
    }
}

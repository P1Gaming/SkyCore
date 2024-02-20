using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

public class DroneStateMachine : MonoBehaviour
{
    [SerializeField]
    private bool _logStateMachineTransitions;
    [SerializeField]
    private DroneBehaviourSettings _settings;
    [SerializeField]
    private FSMDefinition _stateMachineDefinition;

    [Header("Finite State Machine Parameters")]
    [SerializeField]
    private FSMParameter _distanceFromHoverbyPlayerParameter;

    [Header("Finite State Machine Events")]
    [SerializeField]
    private GameEventScriptableObject _idleUpdate;
    [SerializeField]
    private GameEventScriptableObject _followPlayerUpdate;

    [SerializeField]
    private DroneScanning _scanning;

    private FiniteStateMachineInstance _stateMachineInstance;
    private Transform _player;
    private GameEventResponses _gameEventResponses = new();

    


    private void Awake()
    {
        if (_stateMachineInstance == null)
        {
            _stateMachineInstance = new FiniteStateMachineInstance(_stateMachineDefinition, this, _logStateMachineTransitions);
        }
        _player = Player.Motion.PlayerMovement.Instance.transform;

        // There's only 1 drone, so don't really need to use selective responses for the drone, but might as well.
        _gameEventResponses.SetSelectiveResponses(this
            , (_idleUpdate, UpdateIdle)
            , (_followPlayerUpdate, UpdateMoveTowardsPlayer)
            );
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    private void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        _scanning.OnPreStateMachineInstanceUpdate();

        _stateMachineInstance.SetFloat(_distanceFromHoverbyPlayerParameter, FromDroneToNear(_player).magnitude);
        _stateMachineInstance.Update();

    }

    #region Drone Movement
    //---------------------------------------------------------------

    /// <summary>
    /// Moves the drone along a vector, but not past the end of that vector.
    /// </summary>
    public void MoveDrone(Vector3 toTarget)
    {
        float maxMovementDistance = Time.deltaTime * _settings.MovementSpeed;
        transform.position += Vector3.MoveTowards(Vector3.zero, toTarget, maxMovementDistance);
    }

    /// <summary>
    /// Returns a vector from the drone to a point near the target.
    /// </summary>
    public Vector3 FromDroneToNear(Transform toHoverNear, bool backAwayIfTooClose = true)
    {
        Vector3 targetPosition = MoveUpAndTowardsDrone(toHoverNear.position, transform.position, _settings.HoverHeight
            , _settings.MoveToHorizontalDistanceFromTarget);
        Vector3 toTargetPosition = targetPosition - transform.position;

        if (!backAwayIfTooClose)
        {
            // If it's already closer horizontally than the target position, use its current horizontal position
            Vector3 toSamePosition = toHoverNear.position - transform.position;
            float horizontalDistanceToSamePosition = (new Vector2(toSamePosition.x, toSamePosition.z)).magnitude;
            if (horizontalDistanceToSamePosition < _settings.MoveToHorizontalDistanceFromTarget)
            {
                toTargetPosition.x = 0;
                toTargetPosition.z = 0;
            }
        }

        return toTargetPosition;
    }

    /// <summary>
    /// Moves a vector up and towards the drone (but not vertically towards the drone, only 
    /// horizontally.) The point is to get a vector near an object, for the drone to hover 
    /// next to.
    /// </summary>
    public static Vector3 MoveUpAndTowardsDrone(Vector3 toMove, Vector3 dronePosition
        , float upDistance, float horizontalDistance)
    {
        Vector3 horizontalDirection = toMove - dronePosition;
        horizontalDirection.y = 0;
        horizontalDirection.Normalize();

        return toMove + (upDistance * Vector3.up) - (horizontalDistance * horizontalDirection);
    }

    /// <summary>
    /// Rotates the drone towards the provided transform.
    /// </summary>
    public void RotateTowardsTarget(Transform target)
    {
        Quaternion priorRotation = transform.rotation;

        // is there a way to get this rotation w/o changing the transform's rotation?
        transform.LookAt(target);
        Quaternion rotateTowards = transform.rotation;

        transform.rotation = Quaternion.RotateTowards(priorRotation, rotateTowards, _settings.RotationSpeed * Time.deltaTime);
    }
    #endregion


    #region State Machine Event Responses
    //---------------------------------------------------------------

    private void UpdateIdle()
    {
        float rotateDegrees = _settings.IdleRotationSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0, rotateDegrees, 0));
    }

    private void UpdateMoveTowardsPlayer()
    {
        RotateTowardsTarget(_player);
        MoveDrone(FromDroneToNear(_player));
    }
    #endregion

    public FiniteStateMachineInstance GetStateMachineInstance()
    {
        if (_stateMachineInstance == null)
        {
            // this script's Awake() hasn't run yet.
            _stateMachineInstance = new FiniteStateMachineInstance(_stateMachineDefinition, this, _logStateMachineTransitions);
        }
        return _stateMachineInstance;
    }
}

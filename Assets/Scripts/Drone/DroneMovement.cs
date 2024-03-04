using Codice.CM.Common.Merge;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

// This is placeholder movement. The drone will pass through blocks.
// We can either make it looks like the drone is super sci-fi and can pass thru blocks
// or do something like navmesh so it flies around obstacles.
// We might need to do our own navigation implementation if Unity's navmesh doesn't
// support 3d. Or look into other options.

public class DroneMovement : MonoBehaviour
{
    [SerializeField, Tooltip("How fast the drone moves.")]
    private float _movementSpeed = 3f;
    [SerializeField, Tooltip("How fast the drone rotates towards something, in degrees/sec")]
    private float _turnSpeed = 300f;
    [SerializeField, Tooltip("How fast the drone spins while idling next to the player, in degrees/sec")]
    private float _idleSpinSpeed = 30f;
    [SerializeField, Tooltip("How far away the drone tries to hover from a target, horizontally")]
    private float _moveToHorizontalDistanceFromTarget = 3f;
    [SerializeField, Tooltip("How high the drone tries to hover relative to its target")]
    private float _hoverHeight = 2f;


    private Transform _player;


    // This specifies how close the drone must be to the target point to consider itself to have arrived.
    private const float _hasArrivedDistanceThreshold = 0.1f;



    private void Awake()
    {
        _player = Player.Motion.PlayerMovement.Instance.transform;
    }

    public void IdleMovement()
    {
        float rotateDegrees = _idleSpinSpeed * Time.deltaTime;
        transform.Rotate(new Vector3(0, rotateDegrees, 0));
    }

    /// <summary>
    /// Moves the drone along a vector, but not past the end of that vector.
    /// Should be called from an update method until the drone reaches his destination.
    /// </summary>
    /// <returns>True if the drone has arrived at its destination, or false otherwise.</returns>
    public bool MoveDrone(Vector3 toTarget)
    {
        float maxMovementDistance = Time.deltaTime * _movementSpeed;

        float distance = Vector3.Distance(transform.position, transform.position + toTarget);
        if (distance < _hasArrivedDistanceThreshold)
        {
            return true;
        }

        transform.position += Vector3.MoveTowards(Vector3.zero, toTarget, maxMovementDistance);

        return false;
    }

    /// <summary>
    /// Returns a vector from the drone to a point near the target.
    /// </summary>
    public Vector3 FromDroneToNear(Transform toHoverNear, bool backAwayIfTooClose = true)
    {
        Vector3 targetPosition = MoveUpAndTowardsDrone(toHoverNear.position, transform.position, _hoverHeight
            , _moveToHorizontalDistanceFromTarget);
        Vector3 toTargetPosition = targetPosition - transform.position;

        if (!backAwayIfTooClose)
        {
            // If it's already closer horizontally than the target position, use its current horizontal position
            Vector3 toSamePosition = toHoverNear.position - transform.position;
            float horizontalDistanceToSamePosition = (new Vector2(toSamePosition.x, toSamePosition.z)).magnitude;
            if (horizontalDistanceToSamePosition < _moveToHorizontalDistanceFromTarget)
            {
                toTargetPosition.x = 0;
                toTargetPosition.z = 0;
            }
        }

        return toTargetPosition;
    }

    /// <summary>
    /// This function moves the drone to a point that is a specified distance directly in front of the player.
    /// It should be called in an Update() method until the drone has arrived.
    /// </summary>
    /// <param name="distanceFromPlayer">How far in front of the player the drone will stop.</param>
    /// <param name="keepFacingPlayer">If set to true, the drone will constantly keep rotating to face the player as it moves toward him.</param>
    /// <param name="height">(Optional) The drone will try to stay at this height above the player. This parameter's value is defaulted so that the drone is a bit above the player, making it look down at you at an angle.</param>
    /// <returns>True if the drone has arrived at its destination, or false otherwise.</returns>
    public bool MoveDroneInFrontOfPlayer(float distanceFromPlayer, bool keepFacingPlayer, float height = 1.5f)
    {
        Vector3 targetPosition = _player.transform.position + (_player.transform.forward * distanceFromPlayer);
        targetPosition.y = _player.transform.position.y + height;

        // Calculate vector from drone to target point in front of the player.
        Vector3 direction = targetPosition - transform.position;

        //Debug.DrawLine(transform.position, transform.position + direction, Color.green, 1f);

        if (direction.magnitude > -_hasArrivedDistanceThreshold &&
            direction.magnitude < _hasArrivedDistanceThreshold)
        {
            return true;
        }

        MoveDrone(direction);

        if (keepFacingPlayer) 
        {
            RotateTowardsTarget(_player.transform.position);
        }

        return false;
    }

    /// <summary>
    /// Moves a vector up and towards the drone (but not vertically towards the drone, only 
    /// horizontally.) The point is to get a vector near an object, for the drone to hover 
    /// next to.
    /// </summary>
    private static Vector3 MoveUpAndTowardsDrone(Vector3 toMove, Vector3 dronePosition
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
    /// <param name="target">The transform to look at.</param>
    public void RotateTowardsTarget(Transform target)
    {
        RotateTowardsTarget(target.position);
    }

    /// <summary>
    /// Rotates the drone towards the specified point in world space.
    /// </summary>
    /// <param name="target">The point to look at.</param>
    public void RotateTowardsTarget(Vector3 target)
    {
        Quaternion priorRotation = transform.rotation;

        // is there a way to get this rotation w/o changing the transform's rotation?
        // Answer: We could calculate the vector from drone to target, and then calculate its horizontal angle. That's not hard, but I'm not sure if it's worth it, though.
        transform.LookAt(target);
        Quaternion rotateTowards = transform.rotation;

        transform.rotation = Quaternion.RotateTowards(priorRotation, rotateTowards, _turnSpeed * Time.deltaTime);
    }

}

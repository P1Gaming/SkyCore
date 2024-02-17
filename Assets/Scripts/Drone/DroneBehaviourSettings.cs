using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Settings for drone behaviour.
/// </summary>
[System.Serializable] // to show in inspector
public class DroneBehaviourSettings
{
    [field: Header("Drone Movement")]
    [field: SerializeField, Tooltip("How fast the drone moves.")]
    public float MovementSpeed { get; private set; } = 3f;
    [field: SerializeField, Tooltip("How fast the drone rotates towards something, in degrees/sec")]
    public float RotationSpeed { get; private set; } = 300f;
    [field: SerializeField, Tooltip("How fast the drone spins while idling next to the player, in degrees/sec")]
    public float IdleRotationSpeed { get; private set; } = 30f;
    [field: SerializeField, Tooltip("How far away the drone tries to hover from a target, horizontally")]
    public float MoveToHorizontalDistanceFromTarget { get; private set; } = 3f;
    [field: SerializeField, Tooltip("How high the drone tries to hover relative to its target")]
    public float HoverHeight { get; private set; } = 2f;

    [field: Header("Drone Movement Starting and Stopping")]
    [field: SerializeField, Tooltip("The drone's position might never precisely equal its target position," +
        " so this gives it a little tolerance to stop trying to move there.")]
    public float MaxDistanceFromTargetPositionToStop { get; private set; } = .01f;
    [field: SerializeField, Tooltip("Once the drone is at least this distance from its target position," +
        " it resumes moving towards that position. Must be greater than MaxDistanceFromTargetPositionToStop.")]
    public float MinDistanceFromTargetPositionToResumeMoving = .1f;
    [field: SerializeField, Tooltip("Same as above, but during the tutorial, the drone uses this.")]
    public float MinDistanceFromTargetPositionToResumeMovingDuringTutorial = 3f;

    [field: Header("Scanning")]
    [field: SerializeField, Tooltip("Max distance from the drone to scan things")]
    public float DetectionRange { get; private set; } = 10f;
    [field: SerializeField, Tooltip("Max distance from the player things can be and still will be scanned")]
    public float MaxDistanceFromPlayerToScan { get; private set; } = 10f;
    [field: SerializeField, Tooltip("How long it takes the drone to scan an item or jelly after the player gets close enough")]
    public float ScanSucceedDuration { get; private set; } = 1f;
    [field: SerializeField, Tooltip("How long the drone tries to scan something, waiting for player to get close, before it gives up")]
    public float ScanFailAfterDurationParameter { get; private set; } = 5f;
    [field: SerializeField, Tooltip("How close the drone must be (from where it wants to hover next to the player)" +
        " in order to start moving towards something to scan it.")]
    public float MaxDistanceFromDroneToHoverbyPlayerToGoScan { get; private set; } = .5f;
    [field: SerializeField, Tooltip("How close the player must go to whatever the drone is trying to scan before" +
        " the drone will successfully scan it.")]
    public float MaxDistanceBetweenPlayerAndUnscannedToSuccessfullyScan { get; private set; } = 3f;
    [field: SerializeField, Tooltip("With button press scanning, how far the player must be from the unscanned thing" +
        " in order for the button to deactivate and scanning to fail.")]
    public float ButtonDeactivationRadius { get; private set; } = 5f;

    [field: Header("Misc")]
    [field: SerializeField]
    public bool SkipTutorial { get; private set; }
}

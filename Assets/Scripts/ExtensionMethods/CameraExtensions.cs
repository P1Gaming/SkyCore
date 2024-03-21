using System.Collections;
using System.Collections.Generic;

using UnityEngine;


/// <summary>
/// This class contains extension methods for type Camera.
/// </summary>
public static class CameraExtensions
{
    /// <summary>
    /// This function will tell you if the specified object is in front of the camera or not based on the thresholds (angles in degrees) you pass in.
    /// In otherwords, the object's position cannot be farther from the center of the screen than horizThreshold degrees horizontally, and vertThreshold
    /// degrees vertically.
    /// </summary>
    /// <param name="camera">The camera to check if the object is in front of.</param>
    /// <param name="target">The object we want to know whether it is in front of the camera or not.</param>
    /// <param name="horizThreshold">The horizontal threshold. The object must be within this many degrees of the camera's forward vector.</param>
    /// <param name="vertThreshold">The vertical threshold. The object must be within this many degrees of the camera's forward vector.</param>
    /// <returns>True if the object is within the specified thresholds of the camera's view, or false otherwise.</returns>
    public static bool IsObjectInFrontOfCamera(this Camera camera, GameObject target, float horizThreshold = 10f, float vertThreshold = 10f)
    {
        Vector3 targetPosition = target.transform.position;
        Vector3 camPosition = camera.transform.position;
        Vector3 camForward = camera.transform.forward;

        Vector3 direction = targetPosition - camPosition;

        Vector3 directionHorizOnly = new Vector3(direction.x, 0, direction.z);
        Vector3 camForwardHorizOnly = new Vector3(camForward.x, 0, camForward.z);


        // Calculate the flat distance between the two points as if they are at the same altitude.
        float horizDist = Vector3.Distance(camForwardHorizOnly, directionHorizOnly);
        // Calculate the height difference between the two points.
        float vertDist = targetPosition.y - camPosition.y;


        // Calculate difference between the camera angle and the vector going from the camera to the drone.
        float horizontalAngleDif = Vector3.Angle(camForwardHorizOnly, directionHorizOnly);

        // Then do the same thing for the difference in the vertical angle of the two vectors.
        // This first line is just using the line equation: y = mx + b to find the y value of the camera's forward vector at a distance of horizDist.
        float y = camForward.y / camForwardHorizOnly.magnitude * horizDist + 0;
        float verticalAngleDif = Vector3.Angle(new Vector3(0, y, horizDist), new Vector3(0, direction.y, horizDist));



        // Check if the target object is within horizThreshold degrees of the center of the screen and vertThreshold degrees of the center of the screen.
        return Mathf.Abs(horizontalAngleDif) <= horizThreshold && Mathf.Abs(verticalAngleDif) <= vertThreshold;
    }

}

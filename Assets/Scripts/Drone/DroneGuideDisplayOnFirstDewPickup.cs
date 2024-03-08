using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGuideDisplayOnFirstDewPickup : MonoBehaviour
{
    public static DroneGuideDisplayOnFirstDewPickup instance { get; private set; }

    /// <summary>
    /// Makes the drone rotate in a around the y axis and display the island heart when jellyDew is picked up the first time
    /// </summary>

    public bool guidewasdisplayed { get; private set; } = false;
    [SerializeField] Sprite islandHeartImage;
    [SerializeField] float rotationSpeed;
    bool currentlyRotating = false;
    float counter = 0;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (currentlyRotating)
        {
            //TODO: Use the drone's state machine as the source of this movement.
            transform.Rotate(0, -rotationSpeed * 0.5f * Time.deltaTime, 0);
            counter += Time.deltaTime * rotationSpeed;
            Debug.LogWarning(counter);
            if (counter > 2)
            {
                currentlyRotating = false;
                ShowNewlyScannedThing.ShowImage(islandHeartImage);
                guidewasdisplayed = true;
            }
        }
    }

    public void Activate()
    {
        if (!currentlyRotating)
        {
            currentlyRotating = true;
            transform.rotation = Quaternion.identity;
            counter = 0;
        }
    }
}

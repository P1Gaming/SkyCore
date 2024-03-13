using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Earlier in script execution order.
public class InitializeFixedDeltaTime : MonoBehaviour
{
    private void Awake()
    {
        // If the game is running at the monitor's framerate (with vsync), do 2 physics ticks per frame.
        // Non-integer number per frame would cause a slight jitter because the number of ticks would change
        // each frame.

        // This requires vsync enabled to avoid jitter. I'm not sure there's an advantage to not using vsync. Maybe it could
        // feel slightly less delayed because of the frame buffer, but then we could just give settings for
        // the frame buffer count rather than a setting for vsync. I'm not sure. We should also consider disabling
        // vsync being better at low framerates, maybe.

        // refreshRate isn't exact. Would need to update to use refreshRateRatio.
        double frameRate = Screen.currentResolution.refreshRate;
        double physicsRate = frameRate * (frameRate < 120 ? 2 : 1);
        Time.fixedDeltaTime = (float)(1 / physicsRate);
    }
}
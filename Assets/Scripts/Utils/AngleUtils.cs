using System.Collections;
using System.Collections.Generic;

using UnityEngine;


public static class AngleUtils
{
    /// <summary>
    /// This function converts an angle from the normal 0-360 range to -180 to 180.
    /// </summary>
    /// <param name="angle">The angle to convert.</param>
    /// <returns>The converted angle.</returns>
    public static float ConvertAngleToPlusMinus180(float angle)
    {
        if (angle < 180)
        {
            return angle;
        }
        else
        {
            // The angle is above 180 degrees. So for example, 360 degrees converted to the range -180 to 180 is -1.
            // This is because normally if you rotate the camera left past 0 degrees, it will change to 359 and count down as you keep going left.
            // Therefore, that's what 359 needs to convert int into.
            return (180f - (angle - 180f)) * -1f;
        }
    }

    /// <summary>
    /// This function takes an angle in the range -180 to 180 and converts it back to the normal 0-360 range.
    /// </summary>
    /// <param name="angle">The angle to convert.</param>
    /// <returns>The passed in angle converted back to the 0-360 range.</returns>
    public static float ConvertAngleBackTo0To360(float angle)
    {
        if (angle >= 0)
        {
            return angle;
        }
        else
        {
            // The angle is below 0 degrees, so to need to do the opposite of what we did in ConvertAngleToPlusMinus180().
            return (180f - (angle * -1f)) + 180f;
        }
    }
}

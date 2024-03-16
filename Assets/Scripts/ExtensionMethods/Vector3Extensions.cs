using System.Collections;
using System.Collections.Generic;

using UnityEngine;


/// <summary>
/// This class contains extension methods for type Vector3.
/// </summary>
public static class Vector3Extensions
{
    /// <summary>
    /// This function removes the vertical portion of the vector by setting it to 0f.
    /// This is useful when you only care about the horizontal angle of a vector.
    /// </summary>
    /// <param name="vector">The vector to convert.</param>
    /// <returns>The passed in vector with it's vertical component cleared.</returns>
    public static Vector3 ToHorizontalOnly(this Vector3 vector)
    {
        return new Vector3(vector.x, 0f, vector.z);
    }
}

/// <summary>
/// Floating point number that has errors on either side. Example: 1 ± 0.1f
/// </summary>
public struct FloatWithError
{
    // Fields
    /// <summary>
    /// The expected value in an ideal case
    /// </summary>
    public float DirectValue { get; private set; }
    /// <summary>
    /// The amount of deviation from the expected value
    /// </summary>
    public float Tolerance { get; private set; }

    public FloatWithError(float value, float error = 0)
    {
        DirectValue = value;
        Tolerance = error;
    }

    /// <summary>
    /// Special Case: 0 ± 0
    /// </summary>
    public static FloatWithError Zero => new FloatWithError(0, 0);

    public override readonly string ToString()
    {
        return $"{DirectValue} ± {Tolerance}";
    }

    /// <summary>
    /// Lowest (minimum) Threshold
    /// </summary>
    public readonly float MinValue => DirectValue - Tolerance;
    /// <summary>
    /// Highest (maximum) Threshold
    /// </summary>
    public readonly float MaxValue => DirectValue + Tolerance;
}
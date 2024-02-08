using UnityEngine.Animations;

namespace PlayerMovementTest
{
    /// <summary>
    /// Information related for player movement test
    /// </summary>
    /// <typeparam name="T">The specific test information</typeparam>
    public class PlayerMovementTestInfo<T>
    {
        /// <summary>
        /// Name of the test, used only for <c>ToString</c> method.
        /// </summary>
        private string _testName = null;

        /// <summary>
        /// The main test value information.
        /// </summary>
        public T TestValue { get; private set; }
        /// <summary>
        /// The (single) axis of movement for the test.
        /// </summary>
        public Axis Axis { get; private set; }
        /// <summary>
        /// Whether testing in positive direction of axis.
        /// </summary>
        public bool IsPositive { get; private set; }

        /// <summary>
        /// Construct a test info
        /// </summary>
        /// <param name="testValue">Value assigned to <c>TestValue</c></param>
        /// <param name="axis">Value assigned to <c>Axis</c></param>
        /// <param name="isPositive">Value assigned to <c>IsPositive</c></param>
        /// <param name="nameOverride">Optional name override</param>
        public PlayerMovementTestInfo(T testValue, Axis axis, bool isPositive, string nameOverride = "")
        {
            TestValue = testValue;
            Axis = axis;
            IsPositive = isPositive;
            _testName = nameOverride;
        }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(_testName))
            {
                return $"{_testName}";
            }
            return TestValue.ToString();
        }

        /// <summary>
        /// What the expected change should be based on the test axis and direction
        /// </summary>
        /// <param name="givenAxis">Which axis to check</param>
        /// <param name="value">The possible value if the axis is chosen</param>
        /// <param name="error">The amount of error that is allowed</param>
        /// <returns></returns>
        public FloatWithError ExpectedAxisChange(Axis givenAxis, float value, float error)
        {
            float sign = IsPositive ? +1 : -1;
            return givenAxis == Axis ? new FloatWithError(sign * value, error) : FloatWithError.Zero;
        }
    }
}
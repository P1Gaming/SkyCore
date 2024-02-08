using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerMovementTest
{
    /// <summary>
    /// Container of constants used for player movement tests
    /// </summary>
    public static class PlayerMovementTestConstants
    {
        /// <summary>
        /// Player movement test scene
        /// </summary>
        public const string MOVEMENT_TEST_SCENE = "Assets/Tests/Scenes/PlayerMovement_TestEnvironment.unity";
        /// <summary>
        /// Wait time in seconds
        /// </summary>
        public const float WAIT_TIME = 1;
        /// <summary>
        /// Shorter wait time in seconds
        /// </summary>
        public const float SHORT_WAIT_TIME = 0.1f;
        /// <summary>
        /// Speed of the rectilinear walk
        /// </summary>
        public const float WALK_SPEED = 1;
        /// <summary>
        /// Speed of the rectilinear sprint 
        /// </summary>
        public const float SPRINT_SPEED = 2;
        /// <summary>
        /// Error tolerance to allow slightly off distances for assertions
        /// </summary>
        public const float ERROR_TOLERANCE = 0.1f;
    }
}

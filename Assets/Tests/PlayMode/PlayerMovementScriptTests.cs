using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Player;
using Player.Motion;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

namespace PlayerMovementTest
{
    public class PlayerMovementScriptTests
    {
        // Player and Components
        private GameObject _playerGameObject;
        //private RectilinearMovement _rectilinearMovement;
        private PlayerMovement _playerMovement;

        /// <summary>
        /// Primary test info for this class
        /// </summary>
        private static readonly PlayerMovementTestInfo<Vector2>[] _testValues = new PlayerMovementTestInfo<Vector2>[] {
            new PlayerMovementTestInfo<Vector2>(Vector2.right, Axis.X, true, "Right"),
            new PlayerMovementTestInfo<Vector2>(Vector2.left, Axis.X, false, "Left"),
            new PlayerMovementTestInfo<Vector2>(Vector2.up, Axis.Z, true, "Up"),
            new PlayerMovementTestInfo<Vector2>(Vector2.down, Axis.Z, false, "Down"),
        };

        /// <summary>
        /// Test Environment Setup
        /// </summary>
        [UnitySetUp]
        public IEnumerator TestEnvSetup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(PlayerMovementTestConstants.MOVEMENT_TEST_SCENE, new LoadSceneParameters());
            yield return new WaitUntil(() => GameObject.FindGameObjectWithTag("Player") != null);
            // Lock Target frame rate to be independent of user device for testing
            Application.targetFrameRate = 60;
            _playerGameObject = GameObject.FindGameObjectWithTag("Player");
            _playerGameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            _playerGameObject.GetComponent<PlayerInput>().DeactivateInput();
            //_rectilinearMovement = _playerGameObject.GetComponent<RectilinearMovement>();
            _playerMovement = PlayerMovement.Instance;
        }

        /// <summary>
        /// Test Player walking in a specific direction by directly calling `Go` and `Stop` from <c>RectilinearMovement</c>.
        /// </summary>
        /// <param name="testInfo">Direction and assertion values</param>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestPlayerWalkDirectlyCalled([ValueSource(nameof(_testValues))] PlayerMovementTestInfo<Vector2> testInfo)
        {
            var playerStartPosition = _playerGameObject.transform.position;
            var startTime = Time.time;

            // Have player walk in the test direction
            //_playerMovement.SetLocalHorizontalDirection(testInfo.TestValue);
            //_rectilinearMovement.Go(testInfo.TestValue);
            yield return new WaitForSeconds(PlayerMovementTestConstants.WAIT_TIME);
            //_rectilinearMovement.Stop();
            //_playerMovement.SetLocalHorizontalDirection(Vector2.zero);
            var elapsedTime = Time.time - startTime;

            // Wait a bit for movement to settle
            yield return new WaitForSeconds(PlayerMovementTestConstants.SHORT_WAIT_TIME);

            // Get expected values
            float approximateDistanceTravelled = PlayerMovementTestConstants.WALK_SPEED * elapsedTime;
            var expectedXChange = testInfo.ExpectedAxisChange(Axis.X, approximateDistanceTravelled, PlayerMovementTestConstants.ERROR_TOLERANCE);
            var expectedZChange = testInfo.ExpectedAxisChange(Axis.Z, approximateDistanceTravelled, PlayerMovementTestConstants.ERROR_TOLERANCE);

            // Check values
            var positionChange = _playerGameObject.transform.position - playerStartPosition;
            //Assert.That(positionChange.x, Is.InRange(expectedXChange.MinValue, expectedXChange.MaxValue));
            //Assert.That(positionChange.z, Is.InRange(expectedZChange.MinValue, expectedZChange.MaxValue));
            Assert.AreEqual(Mathf.Sign(positionChange.x), Mathf.Sign(expectedXChange.DirectValue));
            Assert.AreEqual(Mathf.Sign(positionChange.z), Mathf.Sign(expectedZChange.DirectValue));
        }

        ///// <summary>
        ///// Test Player sprinting in a specific direction by directly calling `Go` and `Stop` from <c>RectilinearMovement</c>.
        ///// </summary>
        ///// <param name="testInfo">Direction and assertion values</param>
        ///// <returns></returns>
        //[UnityTest]
        //public IEnumerator TestPlayerSprintDirectlyCalled([ValueSource(nameof(_testValues))] PlayerMovementTestInfo<Vector2> testInfo)
        //{
        //    var playerStartPosition = _playerGameObject.transform.position;
        //    var startTime = Time.time;

        //    // Have player sprint in the test direction
        //    _rectilinearMovement.ToggleSprint();
        //    _rectilinearMovement.Go(testInfo.TestValue);
        //    yield return new WaitForSeconds(PlayerMovementTestConstants.WAIT_TIME);
        //    _rectilinearMovement.Stop();
        //    var elapsedTime = Time.time - startTime;

        //    // Settle after sprint
        //    yield return new WaitForSeconds(PlayerMovementTestConstants.SHORT_WAIT_TIME);

        //    // Get expected values
        //    float approximateDistanceTravelled = PlayerMovementTestConstants.SPRINT_SPEED * elapsedTime;
        //    var expectedXChange = testInfo.ExpectedAxisChange(Axis.X, approximateDistanceTravelled, PlayerMovementTestConstants.ERROR_TOLERANCE);
        //    var expectedZChange = testInfo.ExpectedAxisChange(Axis.Z, approximateDistanceTravelled, PlayerMovementTestConstants.ERROR_TOLERANCE);

        //    // Check values
        //    var positionChange = _playerGameObject.transform.position - playerStartPosition;
        //    Assert.That(positionChange.x, Is.InRange(expectedXChange.MinValue, expectedXChange.MaxValue));
        //    Assert.That(positionChange.z, Is.InRange(expectedZChange.MinValue, expectedZChange.MaxValue));
        //}

        ///// <summary>
        ///// Test player sprint is faster than walk by directly calling `Go`, `Stop`, and `ToggleSprint` from <c>RectilinearMovement</c>.
        ///// </summary>
        //[UnityTest]
        //public IEnumerator TestPlayerWalkSlowerThanSprint()
        //{
        //    var playerStartPosition = _playerGameObject.transform.position;
        //    Vector2 Forward = Vector2.up;

        //    // Walk forward for a second
        //    _rectilinearMovement.Go(Forward);
        //    yield return new WaitForSeconds(PlayerMovementTestConstants.WAIT_TIME);
        //    _rectilinearMovement.Stop();
        //    Vector3 playerRoughWalkSpeed1 = _playerGameObject.transform.position - playerStartPosition;
        //    playerStartPosition = _playerGameObject.transform.position;

        //    // Switch to sprinting
        //    _rectilinearMovement.ToggleSprint();

        //    // Sprint forward for a second
        //    _rectilinearMovement.Go(Forward);
        //    yield return new WaitForSeconds(PlayerMovementTestConstants.WAIT_TIME);
        //    _rectilinearMovement.Stop();
        //    Vector3 playerRoughSprintSpeed = _playerGameObject.transform.position - playerStartPosition;
        //    playerStartPosition = _playerGameObject.transform.position;

        //    // Switch back to walking
        //    _rectilinearMovement.ToggleSprint();

        //    // Walk forward again for a second
        //    _rectilinearMovement.Go(Forward);
        //    yield return new WaitForSeconds(PlayerMovementTestConstants.WAIT_TIME);
        //    _rectilinearMovement.Stop();
        //    Vector3 playerRoughWalkSpeed2 = _playerGameObject.transform.position - playerStartPosition;
        //    playerStartPosition = _playerGameObject.transform.position;

        //    // Check values
        //    Assert.Less(playerRoughWalkSpeed1.z, playerRoughSprintSpeed.z);
        //    Assert.Greater(playerRoughSprintSpeed.z, playerRoughWalkSpeed2.z);
        //}
    }
}
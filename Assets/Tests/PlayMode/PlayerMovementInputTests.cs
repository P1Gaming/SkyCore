using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem.Users;
using UnityEngine.Animations;

namespace PlayerMovementTest
{
    public class PlayerMovementInputTests : InputTestFixture
    {
        // Player and Components
        private GameObject _playerGameObject;
        private PlayerInput _playerInput;
        private InputAction _moveAction;
        private Keyboard _keyboard;

        //public const Key SPRINT_KEY = Key.LeftShift;

        /// <summary>
        /// Primary test info for this class
        /// </summary>
        private static readonly PlayerMovementTestInfo<Key>[] _testValues = new PlayerMovementTestInfo<Key>[] {
        new PlayerMovementTestInfo<Key>(Key.UpArrow, Axis.Z, true),
        new PlayerMovementTestInfo<Key>(Key.W, Axis.Z, true),
        new PlayerMovementTestInfo<Key>(Key.LeftArrow, Axis.X, false),
        new PlayerMovementTestInfo<Key>(Key.A, Axis.X, false),
        new PlayerMovementTestInfo<Key>(Key.DownArrow, Axis.Z, false),
        new PlayerMovementTestInfo<Key>(Key.S, Axis.Z, false),
        new PlayerMovementTestInfo<Key>(Key.RightArrow, Axis.X, true),
        new PlayerMovementTestInfo<Key>(Key.D, Axis.X, true),
    };

        /// <summary>
        /// Run setup for input component and devices. 
        /// Function NOT directly marked as Setup/UnitySetup, due to some issues with the setup lifecycle.
        /// </summary>
        /// <returns></returns>
        public IEnumerator InputTestSetup()
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(PlayerMovementTestConstants.MOVEMENT_TEST_SCENE, new LoadSceneParameters());
            yield return null;
            // Lock Target frame rate to be independent of user device for testing
            Application.targetFrameRate = 60;
            _playerGameObject = GameObject.FindGameObjectWithTag("Player");
            _playerGameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            _playerInput = _playerGameObject.GetComponent<PlayerInput>();
            _keyboard = InputSystem.AddDevice<Keyboard>();
            InputUser.PerformPairingWithDevice(_keyboard, _playerInput.user.valid ? _playerInput.user : default);
            _moveAction = _playerInput.actions.FindAction("Move", true);
            yield return null;
        }

        /// <summary>
        /// Tear Down testing player input and devices
        /// </summary>
        public override void TearDown()
        {
            // Deactivate Player Input -> Unpair the InputUser
            if (_playerInput != null && _playerInput.user.valid)
            {
                // Deactivate Player Input
                _playerInput.enabled = false;
            }

            // Remove the device
            if (_keyboard != null)
            {
                InputSystem.RemoveDevice(_keyboard);
            }
            base.TearDown();
        }

        /// <summary>
        /// Check if Test Devices are setup properly (required for later tests)
        /// </summary>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestDeviceProperlySetup()
        {
            yield return InputTestSetup();
            Assert.NotZero(_playerInput.devices.Count);
            Assert.NotZero(_moveAction.controls.Count);
        }

        /// <summary>
        /// Test if player can walk in a specific direction when pressing a specific key.
        /// </summary>
        /// <param name="testInfo">Keyboard input and assertion value</param>
        /// <returns></returns>
        [UnityTest]
        public IEnumerator TestPlayerWalkWithKey([ValueSource(nameof(_testValues))] PlayerMovementTestInfo<Key> testInfo)
        {
            // Setup
            yield return InputTestSetup();
            var startPos = _playerGameObject.transform.position;
            var startTime = Time.time;

            // Press Test Key
            Press(_keyboard[testInfo.TestValue]);
            yield return null;
            Assert.IsTrue(_keyboard[testInfo.TestValue].IsPressed());

            // Wait a second for movement
            yield return new WaitForSeconds(PlayerMovementTestConstants.WAIT_TIME);

            // Release Test Key
            Release(_keyboard[testInfo.TestValue]);
            yield return null;
            Assert.IsTrue(!_keyboard[testInfo.TestValue].IsPressed());
            var elapsedTime = Time.time - startTime;

            // Wait a bit for movement to settle
            yield return new WaitForSeconds(PlayerMovementTestConstants.SHORT_WAIT_TIME);

            // Check Value
            float approximateDistanceTravelled = PlayerMovementTestConstants.WALK_SPEED * elapsedTime;
            var expectedXChange = testInfo.ExpectedAxisChange(Axis.X, approximateDistanceTravelled, PlayerMovementTestConstants.ERROR_TOLERANCE);
            var expectedZChange = testInfo.ExpectedAxisChange(Axis.Z, approximateDistanceTravelled, PlayerMovementTestConstants.ERROR_TOLERANCE);

            var actualChange = _playerGameObject.transform.position - startPos;
            //Assert.That(actualChange.x, Is.InRange(expectedXChange.MinValue, expectedXChange.MaxValue));
            //Assert.That(actualChange.z, Is.InRange(expectedZChange.MinValue, expectedZChange.MaxValue));

            Assert.AreEqual(Mathf.Sign(actualChange.x), Mathf.Sign(expectedXChange.DirectValue));
            Assert.AreEqual(Mathf.Sign(actualChange.z), Mathf.Sign(expectedZChange.DirectValue));
        }

        ///// <summary>
        ///// Test if player can sprint in a specific direction when pressing a specific key.
        ///// </summary>
        ///// <param name="testInfo">Keyboard input and assertion value</param>
        ///// <returns></returns>
        //[UnityTest]
        //public IEnumerator TestPlayerSprintWithKey([ValueSource(nameof(_testValues))] PlayerMovementTestInfo<Key> testInfo)
        //{
        //    // Setup
        //    yield return InputTestSetup();
        //    var startPos = _playerGameObject.transform.position;

        //    // Press Sprint modifier (but don't move yet)
        //    Press(_keyboard[SPRINT_KEY]);
        //    yield return null;
        //    Assert.IsTrue(_keyboard[SPRINT_KEY].IsPressed());

        //    // Sprint key without moving does nothing
        //    yield return new WaitForSeconds(PlayerMovementTestConstants.SHORT_WAIT_TIME);
        //    Assert.AreEqual(startPos, _playerGameObject.transform.position);

        //    // Press Test Key
        //    Press(_keyboard[testInfo.TestValue]);
        //    yield return null;
        //    Assert.IsTrue(_keyboard[testInfo.TestValue].IsPressed());
        //    var startTime = Time.time;

        //    // Wait a second for movement
        //    yield return new WaitForSeconds(PlayerMovementTestConstants.WAIT_TIME);

        //    // Release Test Keys
        //    // HACK: Release does not allow releasing multiple keys 
        //    //       (input queue state appears to get overridden)
        //    //       Need to work around this by waiting a frame, 
        //    //       which should be fine since position does not
        //    //       need to be absolutely spot on.
        //    Release(_keyboard[SPRINT_KEY]);
        //    yield return null;
        //    Release(_keyboard[testInfo.TestValue]);
        //    yield return null;
        //    Assert.IsTrue(!_keyboard[SPRINT_KEY].IsPressed());
        //    Assert.IsTrue(!_keyboard[testInfo.TestValue].IsPressed());
        //    var elapsedTime = Time.time - startTime;

        //    // Wait a bit for movement to settle
        //    yield return new WaitForSeconds(PlayerMovementTestConstants.SHORT_WAIT_TIME);

        //    // Check Values
        //    float approximateDistanceTravelled = PlayerMovementTestConstants.SPRINT_SPEED * elapsedTime;
        //    var expectedXChange = testInfo.ExpectedAxisChange(Axis.X, approximateDistanceTravelled, PlayerMovementTestConstants.ERROR_TOLERANCE);
        //    var expectedZChange = testInfo.ExpectedAxisChange(Axis.Z, approximateDistanceTravelled, PlayerMovementTestConstants.ERROR_TOLERANCE);

        //    var actualChange = _playerGameObject.transform.position - startPos;
        //    Assert.That(actualChange.x, Is.InRange(expectedXChange.MinValue, expectedXChange.MaxValue));
        //    Assert.That(actualChange.z, Is.InRange(expectedZChange.MinValue, expectedZChange.MaxValue));
        //}
    }
}
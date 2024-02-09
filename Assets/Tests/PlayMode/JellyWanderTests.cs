using Jellies;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Jellies.Behaviors;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class JellyWanderTests
{
    public const string JELLY_MOVEMENT_TEST_SCENE = "Assets/Tests/Scenes/JellyWander_TestEnvironment.unity";

    public const float AGENT_MOVE_SPEED = 0.5f;

    private GameObject _jellyGameObject;

    private Vector3 _destination;

    /// <summary>
    /// Sets up scene and jelly scripts on test game object.
    /// </summary>
    [UnitySetUp]
    public IEnumerator TestEnvSetup()
    {
        yield return EditorSceneManager.LoadSceneAsyncInPlayMode(JELLY_MOVEMENT_TEST_SCENE, new LoadSceneParameters());
        yield return new WaitUntil(() => GameObject.FindObjectOfType(typeof(Wandering)) != null);
        // Lock Target frame rate to be independent of user device for testing
        Application.targetFrameRate = 60;
        _jellyGameObject = GameObject.FindObjectOfType<Wandering>().gameObject;
        _jellyGameObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// Test Jelly Wandering script by comparing the NavMeshAgent's destination to the Jelly's transform after giving it
    /// enough time to walk to the destination.
    /// </summary>
    [UnityTest]
    public IEnumerator TestJellyWander()
    {
        // Setup
        yield return TestEnvSetup();
        Parameters parameters = _jellyGameObject.AddComponent<Parameters>();
        Wandering wandering = _jellyGameObject.AddComponent<Wandering>();
        UnityEngine.AI.NavMeshAgent _agent = wandering.GetComponent<UnityEngine.AI.NavMeshAgent>();
        _destination = _agent.destination;

        // Wander test starts here.
        // Check distance from destination
        Vector3 distance = new Vector3(_destination.x - _jellyGameObject.transform.position.x, 0, _destination.z - _jellyGameObject.transform.position.z);
        yield return new WaitForSeconds(distance.magnitude / AGENT_MOVE_SPEED);

        // Check distance from destination after given sufficient time to move
        distance = new Vector3(_destination.x - _jellyGameObject.transform.position.x, 0, _destination.z - _jellyGameObject.transform.position.z);
        Assert.IsTrue(distance.magnitude <= 0.1f);

        yield return null;
    }
}

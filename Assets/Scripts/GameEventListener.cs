using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A simple GameEventListener that can listen for an event with no parameters.
/// </summary>

[System.Serializable]
public class CustomGameEvent : UnityEvent<object> { }
/// <summary>
/// A simple GameEventListener that can listen for an event with no parameters.
/// </summary>
public class GameEventListener : MonoBehaviour
{
    /// <summary>
    /// The GameEvent this listener script will listen changes for.
    /// </summary>
    [Tooltip("Event to register with.")]
    public GameEventScriptableObject Event;

    /// <summary>
    /// The response to invoke when Event is raised.
    /// </summary>
    [Tooltip("Response to invoke when Event is raised.")]
    public UnityEvent Response;

    /// <summary>
    /// The response to invoke when Event is raised that needs to pass data.
    /// </summary>
    [Tooltip("Response to invoke when Event is raised that passes data.")]
    public CustomGameEvent DataResponse;
    private void OnEnable()
    {
        Event.RegisterListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this);
    }

    /// <summary>
    /// The function that will be called when Event occurs.
    /// </summary>
    public void OnEventRaised()
    {
        Response.Invoke();
    }

    /// <summary>
    /// This method is a method similar to OnEventRaised to allow for data to be passed through the event from the drone to the listener for the pictogram
    /// </summary>
    public void OnDataEventRaised(object data)
    {
        DataResponse.Invoke(data);
    }
}

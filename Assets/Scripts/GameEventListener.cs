using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class CustomGameEvent : UnityEvent<object> { }

/// <summary>
/// A simple GameEventListener that can listen for an event with no parameters.
/// </summary>
public class GameEventListener : MonoBehaviour
{
    [SerializeField, Tooltip("Event to register with.")]
    private GameEventScriptableObject Event;

    [SerializeField, Tooltip("Response to invoke when Event is raised.")]
    private UnityEvent Response;

    [SerializeField, Tooltip("Response to invoke when Event is raised that passes data.")]
    private CustomGameEvent DataResponse;

    [SerializeField, Tooltip("Only hear the event if it is raised by this script, " +
        "or something associated with it (e.g. a particular jelly). Leave it null to not be selective.")]
    private MonoBehaviour _selectivelyListenTo = null;

    private void OnEnable()
    {
        Event.RegisterListener(this, _selectivelyListenTo);
    }

    private void OnDisable()
    {
        Event.UnregisterListener(this, _selectivelyListenTo);
    }

    /// <summary>
    /// The function that will be called when Event occurs.
    /// </summary>
    public void OnEventRaised()
    {
        Response.Invoke();
    }

    /// <summary>
    /// This method is a method similar to OnEventRaised to allow for data to be passed through the event from 
    /// the drone to the listener for the pictogram.
    /// </summary>
    public void OnDataEventRaised(object data)
    {
        DataResponse.Invoke(data);
    }
}

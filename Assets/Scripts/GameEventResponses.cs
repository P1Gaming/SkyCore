using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Need this complexity because GameEventScriptableObject uses lists of System.Action
// (in order to iterate backwards so a response to an event can be to unregister.)
// Using C# events would be simpler. Whereas with lists of System.Action, need to cache
// a reference to the delegate, because typing the name of a method creates a new delegate
// instance. E.g. if a method is named Eat, list.Add(Eat) then list.Remove(Eat) wouldn't
// actually remove it from the list (I think).

// Also, can't do C# events in dictionaries, so they wouldn't work anyway, because of selective listeners.

// It's also more concise to use this than to doing a bunch of "event += MethodName" to register and a bunch
// of "event -= MethodName" to unregister.

/// <summary>
/// A way for scripts to register and unregister all of their responses to game event scriptable objects.
/// </summary>
public class GameEventResponses
{
    private bool _registered;
    private (GameEventScriptableObject, System.Action)[] _eventsAndResponses = null;
    private (GameEventScriptableObject, System.Action<object>)[] _dataEventsAndResponses = null;
    private (MonoBehaviour, GameEventScriptableObject, System.Action)[] _selectiveEventsAndResponses = null;
    private (MonoBehaviour, GameEventScriptableObject, System.Action<object>)[] _selectiveDataEventsAndResponses = null;

    /// <summary>
    /// Stores an array of responses to game events which have no parameter. Does nothing until Register() is called.
    /// </summary>
    public void SetResponses(params (GameEventScriptableObject, System.Action)[] eventsAndResponses)
    {
        _eventsAndResponses = eventsAndResponses;
    }

    /// <summary>
    /// Stores an array of responses to game events which have a parameter. Does nothing until Register() is called.
    /// </summary>
    public void SetDataResponses(params (GameEventScriptableObject, System.Action<object>)[] dataEventsAndResponses)
    {
        _dataEventsAndResponses = dataEventsAndResponses;
    }

    /// <summary>
    /// Stores an array of responses to game events which have no parameter, and only occur if the game event is raised
    /// by something in particular (e.g. a particular jelly). Does nothing until Register() is called.
    /// </summary>
    public void SetSelectiveResponses(MonoBehaviour selectiveFor
        , params (GameEventScriptableObject, System.Action)[] selectiveEventsAndResponses)
    {
        _selectiveEventsAndResponses = new (MonoBehaviour, GameEventScriptableObject, System.Action)[selectiveEventsAndResponses.Length];
        for (int i = 0; i < selectiveEventsAndResponses.Length; i++)
        {
            _selectiveEventsAndResponses[i] = (selectiveFor, selectiveEventsAndResponses[i].Item1, selectiveEventsAndResponses[i].Item2);
        }
    }

    /// <summary>
    /// Stores an array of responses to game events which have a parameter, and only occur if the game event is raised
    /// by something in particular (e.g. a particular jelly). Does nothing until Register() is called.
    /// </summary>
    public void SetSelectiveDataResponses(MonoBehaviour selectiveFor
        , params (GameEventScriptableObject, System.Action<object>)[] 
        selectiveDataEventsAndResponses)
    {
        _selectiveDataEventsAndResponses 
            = new (MonoBehaviour, GameEventScriptableObject, System.Action<object>)[selectiveDataEventsAndResponses.Length];

        for (int i = 0; i < selectiveDataEventsAndResponses.Length; i++)
        {
            _selectiveDataEventsAndResponses[i] 
                = (selectiveFor, selectiveDataEventsAndResponses[i].Item1, selectiveDataEventsAndResponses[i].Item2);
        }
    }

    /// <summary>
    /// If haven't registered yet or unregistered, then register the responses to the game events.
    /// </summary>
    public void Register()
    {
        if (_registered)
        {
            return;
        }
        _registered = true;
        GameEventScriptableObject.RegisterAll(_eventsAndResponses, _dataEventsAndResponses
            , _selectiveEventsAndResponses, _selectiveDataEventsAndResponses);
    }

    /// <summary>
    /// If have registered and not yet unregistered, then unregister the responses to the game events.
    /// </summary>
    public void Unregister()
    {
        if (!_registered)
        {
            return;
        }
        _registered = false;
        GameEventScriptableObject.UnregisterAll(_eventsAndResponses, _dataEventsAndResponses
            , _selectiveEventsAndResponses, _selectiveDataEventsAndResponses);
    }
}

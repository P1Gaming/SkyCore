using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=raQ3iHhE_Kk at ~33 minutes

[CreateAssetMenu]
public class GameEventScriptableObject : ScriptableObject
{
    private List<GameEventListener> _listeners = new();
    private Dictionary<MonoBehaviour, List<GameEventListener>> _selectiveListeners = new();

    private List<System.Action> _onRaise = new();
    private List<System.Action<object>> _onRaiseWithData = new();
    private Dictionary<MonoBehaviour, List<System.Action>> _selectiveOnRaise = new();
    private Dictionary<MonoBehaviour, List<System.Action<object>>> _selectiveOnRaiseWithData = new();

    /// <summary>
    /// Raises the game event.
    /// </summary>
    /// <param name="sourceForSelectiveListeners">The thing calling this method, e.g. a particular jelly.</param>
    public void Raise(MonoBehaviour sourceForSelectiveListeners = null)
    {
        // Like in the video, iterate backwards in case a listener responds to an event by unregistering itself.
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnEventRaised();
        }
        for (int i = _onRaise.Count - 1; i >= 0; i--)
        {
            _onRaise[i]();
        }

        if (sourceForSelectiveListeners == null)
        {
            return;
        }

        if (_selectiveListeners.TryGetValue(sourceForSelectiveListeners, out List<GameEventListener> listenersForSource)) 
        {
            for (int i = listenersForSource.Count - 1; i >= 0; i--)
            {
                listenersForSource[i].OnEventRaised();
            }
        }
        if (_selectiveOnRaise.TryGetValue(sourceForSelectiveListeners, out List<System.Action> selectiveOnRaiseForSource))
        {
            for (int i = selectiveOnRaiseForSource.Count - 1; i >= 0; i--)
            {
                selectiveOnRaiseForSource[i]();
            }
        }
    }

    /// <summary>
    /// Raises the game event and sends data.
    /// </summary>
    /// <param name="data">Data which can be cast and used.</param>
    /// <param name="sourceForSelectiveListeners">The thing calling this method, e.g. a particular jelly.</param>
    public void Raise(object data, MonoBehaviour sourceForSelectiveListeners = null)
    {
        for (int i = _listeners.Count - 1; i >= 0; i--)
        {
            _listeners[i].OnDataEventRaised(data);
        }
        for (int i = _onRaiseWithData.Count - 1; i >= 0; i--)
        {
            _onRaiseWithData[i](data);
        }

        if (sourceForSelectiveListeners == null)
        {
            return;
        }
        
        if (_selectiveListeners.TryGetValue(sourceForSelectiveListeners, out List<GameEventListener> listenersForSource))
        {
            for (int i = listenersForSource.Count - 1; i >= 0; i--)
            {
                listenersForSource[i].OnDataEventRaised(data);
            }
        }
        if (_selectiveOnRaiseWithData.TryGetValue(sourceForSelectiveListeners, out List<System.Action<object>> selectiveOnRaiseForSource))
        {
            for (int i = selectiveOnRaiseForSource.Count - 1; i >= 0; i--)
            {
                selectiveOnRaiseForSource[i](data);
            }
        }
    }

    /// <summary>
    /// Registers a listener component.
    /// </summary>
    /// <param name="listener">The listener component.</param>
    /// <param name="sourceIfSelectiveListener">The thing which must raise this game event in order for the listener
    /// to respond, e.g. a particular jelly. If null, it always responds.</param>
    public void RegisterListener(GameEventListener listener, MonoBehaviour sourceIfSelectiveListener)
    {
        if (sourceIfSelectiveListener == null)
        {
            if (!_listeners.Contains(listener))
            {
                _listeners.Add(listener);
            }
        }
        else
        {
            if (!_selectiveListeners.TryGetValue(sourceIfSelectiveListener, out List<GameEventListener> listenersForSource))
            {
                listenersForSource = new List<GameEventListener>();
                _selectiveListeners.Add(sourceIfSelectiveListener, listenersForSource);
            }
            if (!listenersForSource.Contains(listener))
            {
                listenersForSource.Add(listener);
            }
        }
    }

    /// <summary>
    /// Unregisters a listener component.
    /// </summary>
    /// <param name="listener">The listener component.</param>
    /// <param name="sourceIfSelectiveListener">The thing which must raise this game event in order for the listener
    /// to respond, e.g. a particular jelly. If null, it always responds.</param>
    public void UnregisterListener(GameEventListener listener, MonoBehaviour sourceIfSelectiveListener)
    {
        if (sourceIfSelectiveListener == null)
        {
            _listeners.Remove(listener);
        }
        else
        {
            if (_selectiveListeners.TryGetValue(sourceIfSelectiveListener, out List<GameEventListener> listenersForSource))
            {
                listenersForSource.Remove(listener);
            }
        }
    }

    #region For ManyGameEventsAndResponses

    // Use GameEventsAndResponses, rather than directly using this.

    public static void RegisterAll((GameEventScriptableObject, System.Action)[] eventsAndResponses
        , (GameEventScriptableObject, System.Action<object>)[] dataEventsAndResponse
        , (MonoBehaviour, GameEventScriptableObject, System.Action)[] selectiveEventsAndResponses
        , (MonoBehaviour, GameEventScriptableObject, System.Action<object>)[] selectiveDataEventsAndResponses)
    {
        if (eventsAndResponses != null)
        {
            for (int i = 0; i < eventsAndResponses.Length; i++)
            {
                (GameEventScriptableObject gameEvent, System.Action action) = eventsAndResponses[i];
                gameEvent.AddOnRaise(action);
            }
        }
        if (dataEventsAndResponse != null)
        {
            for (int i = 0; i < dataEventsAndResponse.Length; i++)
            {
                (GameEventScriptableObject gameEvent, System.Action<object> action) = dataEventsAndResponse[i];
                gameEvent.AddOnRaise(action);
            }
        }
        if (selectiveEventsAndResponses != null)
        {
            for (int i = 0; i < selectiveEventsAndResponses.Length; i++)
            {
                (MonoBehaviour selectiveFor, GameEventScriptableObject gameEvent, System.Action action) 
                    = selectiveEventsAndResponses[i];
                gameEvent.AddOnRaise(action, selectiveFor);
            }
        }
        if (selectiveDataEventsAndResponses != null)
        {
            for (int i = 0; i < selectiveDataEventsAndResponses.Length; i++)
            {
                (MonoBehaviour selectiveFor, GameEventScriptableObject gameEvent, System.Action<object> action) 
                    = selectiveDataEventsAndResponses[i];
                gameEvent.AddOnRaise(action, selectiveFor);
            }
        }
    }

    public static void UnregisterAll((GameEventScriptableObject, System.Action)[] eventsAndResponses
        , (GameEventScriptableObject, System.Action<object>)[] dataEventsAndResponse
        , (MonoBehaviour, GameEventScriptableObject, System.Action)[] selectiveEventsAndResponses
        , (MonoBehaviour, GameEventScriptableObject, System.Action<object>)[] selectiveDataEventsAndResponses)
    {
        if (eventsAndResponses != null)
        {
            for (int i = 0; i < eventsAndResponses.Length; i++)
            {
                (GameEventScriptableObject gameEvent, System.Action action) = eventsAndResponses[i];
                gameEvent.RemoveOnRaise(action);
            }
        }
        if (dataEventsAndResponse != null)
        {
            for (int i = 0; i < dataEventsAndResponse.Length; i++)
            {
                (GameEventScriptableObject gameEvent, System.Action<object> action) = dataEventsAndResponse[i];
                gameEvent.RemoveOnRaise(action);
            }
        }
        if (selectiveEventsAndResponses != null)
        {
            for (int i = 0; i < selectiveEventsAndResponses.Length; i++)
            {
                (MonoBehaviour selectiveFor, GameEventScriptableObject gameEvent, System.Action action)
                    = selectiveEventsAndResponses[i];
                gameEvent.RemoveOnRaise(action, selectiveFor);
            }
        }
        if (selectiveDataEventsAndResponses != null)
        {
            for (int i = 0; i < selectiveDataEventsAndResponses.Length; i++)
            {
                (MonoBehaviour selectiveFor, GameEventScriptableObject gameEvent, System.Action<object> action)
                    = selectiveDataEventsAndResponses[i];
                gameEvent.RemoveOnRaise(action, selectiveFor);
            }
        }
    }

    private void AddOnRaise(System.Action onRaise) => _onRaise.Add(onRaise);
    private void AddOnRaise(System.Action<object> onRaise) => _onRaiseWithData.Add(onRaise);
    private void AddOnRaise(System.Action onRaise, MonoBehaviour selectiveFor)
    {
        if (selectiveFor == null)
        {
            throw new System.ArgumentNullException("selectiveFor");
        }
        if (!_selectiveOnRaise.TryGetValue(selectiveFor, out List<System.Action> onRaiseForSource))
        {
            onRaiseForSource = new List<System.Action>();
            _selectiveOnRaise.Add(selectiveFor, onRaiseForSource);
        }
        onRaiseForSource.Add(onRaise);
    }
    private void AddOnRaise(System.Action<object> onRaise, MonoBehaviour selectiveFor)
    {
        if (selectiveFor == null)
        {
            throw new System.ArgumentNullException("selectiveFor");
        }
        if (!_selectiveOnRaiseWithData.TryGetValue(selectiveFor, out List<System.Action<object>> onRaiseForSource))
        {
            onRaiseForSource = new List<System.Action<object>>();
            _selectiveOnRaiseWithData.Add(selectiveFor, onRaiseForSource);
        }
        onRaiseForSource.Add(onRaise);
    }
    private void RemoveOnRaise(System.Action onRaise) => _onRaise.Remove(onRaise);
    private void RemoveOnRaise(System.Action<object> onRaise) => _onRaiseWithData.Remove(onRaise);
    private void RemoveOnRaise(System.Action onRaise, MonoBehaviour selectiveFor)
    {
        if (selectiveFor == null)
        {
            throw new System.ArgumentNullException("selectiveFor");
        }
        if (_selectiveOnRaise.TryGetValue(selectiveFor, out List<System.Action> onRaiseForSource))
        {
            onRaiseForSource.Remove(onRaise);
        }
    }
    private void RemoveOnRaise(System.Action<object> onRaise, MonoBehaviour selectiveFor)
    {
        if (selectiveFor == null)
        {
            throw new System.ArgumentNullException("selectiveFor");
        }
        if (_selectiveOnRaiseWithData.TryGetValue(selectiveFor, out List<System.Action<object>> onRaiseForSource))
        {
            onRaiseForSource.Remove(onRaise);
        }
    }
    #endregion
}
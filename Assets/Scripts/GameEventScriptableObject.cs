using System.Collections.Generic;
using UnityEngine;

// https://www.youtube.com/watch?v=raQ3iHhE_Kk at ~33 minutes

/// <summary>
/// 
/// </summary>
[CreateAssetMenu]
public class GameEventScriptableObject : ScriptableObject
{
    /// <summary>
    /// The list of listeners that this event will notify if it is raised.
    /// </summary>
    private readonly List<GameEventListener> _eventListeners = 
        new List<GameEventListener>();

    // Events to directly subscribe to, if not using GameEventListener.
    // E.g. if an event is raised every frame, avoid using GameEventListener,
    // because it uses UnityEvent, which generates garbage.
    public event System.Action OnRaise;
    public event System.Action<object> OnRaiseWithData;

    public void Raise()
    {
        OnRaise?.Invoke();
        for(int i = _eventListeners.Count -1; i >= 0; i--)
        {
            _eventListeners[i].OnEventRaised();
        }
    }

    public virtual void Raise(object data)
    {
        OnRaiseWithData?.Invoke(data);
        for (int i = _eventListeners.Count - 1; i >= 0; i--)
        {
            _eventListeners[i].OnDataEventRaised(data);
        }
    }
    public void RegisterListener(GameEventListener listener)
    {
        if (_eventListeners.Contains(listener))
        {
            return;
        }

        _eventListeners.Add(listener);
    }

    public void UnregisterListener(GameEventListener listener)
    {
        if (!_eventListeners.Contains(listener))
        {
            return;
        }

        _eventListeners.Remove(listener);
    }
}
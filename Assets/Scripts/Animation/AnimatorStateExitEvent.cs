using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A way to make it easier to do things when an animator exits a state.
/// </summary>
public class AnimatorStateExitEvent : StateMachineBehaviour
{
    [field: SerializeField]
    public string Identifier { get; private set; }

    public event System.Action OnExit;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        OnExit();
    }

    /// <summary>
    /// Gets an exit event from an animator.
    /// </summary>
    /// <param name="identifier">A string to identify the event, in case multiple states in 
    /// the animator have an AnimatorStateExitEvent.</param>
    public static AnimatorStateExitEvent GetExitEvent(Animator animator, string identifier = null)
    {
        AnimatorStateExitEvent[] exitEvents = animator.GetBehaviours<AnimatorStateExitEvent>();
        foreach (AnimatorStateExitEvent exitEvent in exitEvents)
        {
            if (identifier == null || identifier == exitEvent.Identifier)
            {
                return exitEvent;
            }
        }
        return null;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A way to make it easier to do things when an animator enters a state.
/// </summary>
public class AnimatorStateEnterEvent : StateMachineBehaviour
{
    [field: SerializeField]
    public string Identifier { get; private set; }

    public event System.Action OnEnter;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        OnEnter();
    }

    /// <summary>
    /// Gets an enter event from an animator.
    /// </summary>
    /// <param name="identifier">A string to identify the event, in case multiple states in 
    /// the animator have an AnimatorStateEnterEvent.</param>
    public static AnimatorStateEnterEvent GetEnterEvent(Animator animator, string identifier = null)
    {
        AnimatorStateEnterEvent[] enterEvents = animator.GetBehaviours<AnimatorStateEnterEvent>();
        foreach (AnimatorStateEnterEvent enterEvent in enterEvents)
        {
            if (identifier == null || identifier == enterEvent.Identifier)
            {
                return enterEvent;
            }
        }
        return null;
    }
}

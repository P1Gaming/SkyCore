using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implement this class when making a ui element that 
/// work with the MenuControllerStack.
/// </summary>
public abstract class Page : MonoBehaviour
{
    [SerializeField]
    public bool ExitOnNewPagePush { get; internal set; } = false;

    /// <summary>
    /// Called on entry to this page
    /// </summary>
    /// <param name="playAudio">Should this Play audio?</param>
    public void Enter(bool playAudio)
    {
        EnterAnimation(playAudio);
    }

    /// <summary>
    /// Called on exit to this page
    /// </summary>
    /// <param name="playAudio">Should this Play audio?</param>
    public void Exit(bool playAudio)
    {
        ExitAnimation(playAudio);
    }

    internal abstract void EnterAnimation(bool playAudio);
    internal abstract void ExitAnimation(bool playAudio);

}

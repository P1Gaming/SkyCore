using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Helps with Page entryh/exit animations. Meant to be called by the Page class or any class that may have similar functionalty.
/// </summary>
public class AnimationHelper : MonoBehaviour
{
    /// <summary>
    /// Fade animation handler.
    /// </summary>
    /// <param name="canvasGroup">The canvas group to fade in/out.</param>
    /// <param name="speed">The speed of the transition.</param>
    /// <param name="OnFinish">Called when the animation is completed.</param>
    /// <returns></returns>
#nullable enable
    public static IEnumerator FadeIn(CanvasGroup canvasGroup, float speed, UnityEvent? OnFinish)
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;

        float time = 0;
        while (time < 1)
        {
            //Changes whether we are fading in or out
            canvasGroup.alpha = Mathf.Lerp(0, 1, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        canvasGroup.alpha = 1;
        OnFinish?.Invoke();
    }
#nullable disable
    /// <summary>
    /// Fade animation handler.
    /// </summary>
    /// <param name="canvasGroup">The canvas group to fade in/out.</param>
    /// <param name="speed">The speed of the transition.</param>
    /// <param name="OnFinish">Called when the animation is completed.</param>
    /// <returns></returns>
#nullable enable
    public static IEnumerator FadeOut(CanvasGroup canvasGroup, float speed, UnityEvent? OnFinish)
    {
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;

        float time = 0;
        while (time < 1)
        {
            //Changes whether we are fading in or out
            canvasGroup.alpha = Mathf.Lerp(1, 0, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

        canvasGroup.alpha = 0;
        OnFinish?.Invoke();
    }
#nullable disable
}

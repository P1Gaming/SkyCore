using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to manipulate the canvas group to create an animation for entry/exit of that canvas group. 
/// Meant to be used with the MenuController
/// </summary> 
[RequireComponent(typeof(CanvasGroup))]
[DisallowMultipleComponent]
public class PageFade : Page
{
    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;

    [SerializeField]
    private float _animationSpeed = 1f;

    private Coroutine _animationCoroutine;

    private void Awake()
    {
        _rectTransform = this.GetComponent<RectTransform>();
        _canvasGroup = this.GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Play the fade animation
    /// </summary>
    /// <param name="playAudio">Should this Play audio?</param>
    override internal void EnterAnimation(bool playAudio)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(AnimationHelper.FadeIn(_canvasGroup, _animationSpeed, null));
    }

    /// <summary>
    /// Play the fade animation
    /// </summary>
    /// <param name="playAudio">Should this Play audio?</param>
    override internal void ExitAnimation(bool playAudio)
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
        }

        _animationCoroutine = StartCoroutine(AnimationHelper.FadeOut(_canvasGroup, _animationSpeed, null));
    }
}

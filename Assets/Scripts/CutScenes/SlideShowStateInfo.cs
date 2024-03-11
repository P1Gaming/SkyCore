using System;

using UnityEngine;
using UnityEngine.Playables;


public class SlideShowStateInfo
{
    public PlayState State = PlayState.Paused;
    public float ElapsedTime = 0f;
    public float CurrentSlideElapsedTime = 0f;
    public int CurrentSlideIndex = 0;
    public Slide CurrentSlide;


    // Holds a reference to the slide show this object is holding playback state data for.
    private SlideShow _slideShow;
    private int _nextSlideIndex;



    public SlideShowStateInfo(SlideShow slideShow)
    {
        if (slideShow == null)
        {
            throw new ArgumentNullException("The passed in slide show is null!");
        }


        _slideShow = slideShow;
    }

    public void Clear()
    {
        State = PlayState.Paused;
        ElapsedTime = 0f;
        CurrentSlideElapsedTime = 0f;

        CurrentSlideIndex = 0;
        CurrentSlide = _slideShow.GetSlide(0);

        _nextSlideIndex = _slideShow.SlideCount > 1 ? 1 : -1;
    }

    public float GetCurrentSlideDisplayTime()
    {
        return _slideShow.GetSlideDisplayTime(CurrentSlideIndex);
    }

    public float GetCurrentSlideFadeOutTime()
    {
        return _slideShow.GetSlideFadeOutTime(CurrentSlideIndex);
    }

    public float GetCurrentSlideFadeInTime()
    {
        return _slideShow.GetSlideFadeInTime(CurrentSlideIndex);
    }

    /// <summary>
    /// This function returns the amount of time (in seconds) that will elapse between this slide starting, and when it begins to transition out.
    /// </summary>
    /// <returns>The amount of time (in seconds) that will elapse before this slide starts to transition out.</returns>
    public float GetCurrentSlideTransitionTime()
    {
        return _slideShow.GetSlideDisplayTime(CurrentSlideIndex) - _slideShow.GetSlideFadeOutTime(CurrentSlideIndex);
    }

    /// <summary>
    /// This function gets the total length of the transition from the current slide to the next one.
    /// </summary>
    /// <returns><Total length of the transition between the current slide and the next one./returns>
    public float GetCurrentSlideTransitionDuration()
    {
        return _slideShow.GetSlideTransitionDuration(CurrentSlideIndex);
    }

    /// <summary>
    /// Gets the fade in time of the previous slide.
    /// </summary>
    /// <returns>The fade in time of the previous slide, or 0 if there isn't one.</returns>
    public float GetPrevSlideFadeInTime()
    {        
        int prevSlideIndex = CurrentSlideIndex - 1;
        return prevSlideIndex >= 0 ? _slideShow.GetSlideFadeInTime(prevSlideIndex) : 0f;
    }

    /// <summary>
    /// Gets the fade in time of the previous slide.
    /// </summary>
    /// <returns>The fade out time of the previous slide, or 0 if there isn't one.</returns>
    public float GetPrevSlideFadeOutTime()
    {
        int prevSlideIndex = CurrentSlideIndex - 1;
        return prevSlideIndex >= 0 ? _slideShow.GetSlideFadeOutTime(prevSlideIndex) : 0f;
    }

    /// <summary>
    /// Gets the fade in time of the next slide.
    /// </summary>
    /// <returns>The fade in time of the next slide, or 0 if there isn't one.</returns>
    public float GetNextSlideFadeInTime()
    {
        return _nextSlideIndex >= 0 ? _slideShow.GetSlideFadeInTime(_nextSlideIndex) : 0f;
    }

    /// <summary>
    /// Gets the fade out time of the next slide.
    /// </summary>
    /// <returns>The fade out time of the next slide, or 0 if there isn't one.</returns>
    public float GetNextSlideFadeOutTime()
    {
        return _nextSlideIndex >= 0 ? _slideShow.GetSlideFadeOutTime(_nextSlideIndex) : 0f;
    }

    public void IncrementTimers(float deltaTime)
    {
        ElapsedTime += deltaTime;
        CurrentSlideElapsedTime += deltaTime;
    }

    public void MoveToNextSlide()
    {
        if (CurrentSlideIndex >= _slideShow.SlideCount - 1)
        {
            return;
        }


        CurrentSlideIndex++;
        CurrentSlide = _slideShow.GetSlide(CurrentSlideIndex);

        _nextSlideIndex = CurrentSlideIndex < _slideShow.SlideCount - 1 ? CurrentSlideIndex + 1 
                                                                        : -1;        

        CurrentSlideElapsedTime = 0f;
    }



    public Slide NextSlide
    {
        get
        {
            int nextIndex = CurrentSlideIndex++;

            return nextIndex <= _slideShow.SlideCount - 1 ? _slideShow.GetSlide(nextIndex)
                                                          : null;
        }
    }

    public int NextSlideIndex { get { return _nextSlideIndex; } }
    public SlideShow SlideShow { get { return _slideShow; } }
}


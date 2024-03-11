using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public class SlideShowPlayerEventArgs : EventArgs
{
    public SlideShow SlideShow;
}


public partial class SlideShowPlayer : MonoBehaviour
{
    public static SlideShowPlayer Instance;



    public event EventHandler<SlideShowPlayerEventArgs> OnSlideShowStarted;
    public event EventHandler<SlideShowPlayerEventArgs> OnSlideShowPaused;
    public event EventHandler<SlideShowPlayerEventArgs> OnSlideShowStopped;



    [Tooltip("The UI used for displaying the slide show.")]
    [SerializeField] private SlideShowUI _slideShowUI;


    [Space(10)]
    [Tooltip("This field is optional. It lets you specify a slide show to be played immediately on scene load.")]
    [SerializeField] private SlideShow _slideShowToPlayOnStart;

    [Tooltip("This list lets you specify all of the slide shows in this scene.")]
    [SerializeField] private List<SlideShow> _slideShows = new List<SlideShow>();



    // This holds a reference to the currently playing slide show. It will be null if there isn't one playing.
    private SlideShow _currentlyPlayingSlideShow;
    private SlideShowStateInfo _currentSlideShowStateInfo;

    private Dictionary<SlideShow, SlideShowStateInfo> _slideShowStateLookUp = new Dictionary<SlideShow, SlideShowStateInfo>();

    private bool _isPlaying;
    private StopReasons _stopReason = StopReasons.None; // This is used to tell the PresentSlideShow() method to stop if Pause() or Stop() are called, and the appropriate events then get fired.

    private enum StopReasons
    {
        None,
        Paused,
        Stopped,
    }


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one SlideShowPlayer in this scene! Self destructing.");
            Destroy(gameObject);

            return;
        }


        Instance = this;

        InitLookupTable();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_slideShowToPlayOnStart != null)
            PlaySlideShow(_slideShowToPlayOnStart);
    }


    private void InitLookupTable()
    {
        foreach (SlideShow slideShow in _slideShows)
        {
            _slideShowStateLookUp.Add(slideShow, new SlideShowStateInfo(slideShow));
        }

        // If there is a slide show specified to play on start, and it is not already in our list and lookup table, then add it to the lookup table.
        if (_slideShowToPlayOnStart != null && !_slideShows.Contains(_slideShowToPlayOnStart))
        {
            _slideShowStateLookUp.Add(_slideShowToPlayOnStart, new SlideShowStateInfo(_slideShowToPlayOnStart));
        }
    }

    /// <summary>
    /// This gets the slide show at the specified index in the list.
    /// </summary>
    /// <param name="index">The index of the slide show to retreive from the list.</param>
    public SlideShow GetSlideShow(int index)
    {
        return _slideShows[index];
    }

    /// <summary>
    /// This gets the slide show with the specified name.
    /// </summary>
    /// <remarks>
    /// NOTE: If for some reason there are two slide shows with the same name, this function will return the first one it encounters in the list.
    /// </remarks>
    /// <param name="name">The name of the slide show to get.</param>
    /// <returns>The slide show that has the specified name, or null if none is found.</returns>
    public SlideShow GetSlideShow(string name)
    {
        for (int i = 0; i < _slideShows.Count; i++)
        {
            if (_slideShows[i].name == name)
            {
                return _slideShows[i];
            }
        }


        return null;
    }

    /// <summary>
    /// Returns the currently playing slide show if there is one.
    /// </summary>
    /// <returns>The currently playing slide show if there is one, or null otherwise.</returns>
    public SlideShow GetCurrentlyPlayingSlideShow()
    {
        return _currentlyPlayingSlideShow;
    }

    public bool IsPlaying()
    {
        return _currentlyPlayingSlideShow != null &&
               _slideShowStateLookUp[_currentlyPlayingSlideShow].State == PlayState.Playing;
    }

    public bool IsPaused()
    {
        return _currentlyPlayingSlideShow != null &&
               _slideShowStateLookUp[_currentlyPlayingSlideShow].State == PlayState.Paused;
    }

    public bool PlaySlideShow(int index)
    {
        SlideShow slideShow = _slideShows[index];

        return PlaySlideShow(slideShow);
    }

    public bool PlaySlideShow(string name)
    {
        SlideShow slideShow = GetSlideShow(name);
        if (slideShow == null)
        {
            return false;
        }
        else
        {
            PlaySlideShow(slideShow);
            return true;
        }
    }

    private bool PlaySlideShow(SlideShow slideShow)
    {
        SlideShowStateInfo stateInfo = _slideShowStateLookUp[slideShow];

        if (!(stateInfo.State == PlayState.Playing))
        {
            if (_isPlaying)
            {
                Debug.LogError($"Could not play the slide show, because another slide show ({slideShow.name}) is already playing!");
                return false;
            }
            else if (slideShow.SlideCount == 0)
            {
                Debug.LogError($"Cannot play the slide show \"{slideShow.name}\" because it contains no slides!");
                return false;
            }


            _currentlyPlayingSlideShow = slideShow;
            _currentSlideShowStateInfo = stateInfo;
            
            stateInfo.Clear();
            stateInfo.State = PlayState.Playing;


            StartCoroutine(PresentSlideShow());

            return true;
        }
        else
        {
            Debug.LogError($"Could not play the slide show \"{slideShow.name}\", as it is already playing!");
            return false;
        }
    }

    /// <summary>
    /// Pauses playback of the currently playing slide show if there is one. Call Resume() to continue playback.
    /// </summary>
    public void Pause()
    {
        if (_currentlyPlayingSlideShow != null)
        {
            if (!(_currentSlideShowStateInfo.State == PlayState.Playing))
            {
                _currentSlideShowStateInfo.State = PlayState.Paused;
                FireSlideShowPausedEvent();
            }
            else
            {
                Debug.LogWarning("SlideShowPlayer.Pause() was called, but no slide show is playing.");
            }
        }
        else
        {
            Debug.LogWarning("SlideShowPlayer().Pause() was called while no SlideShow is playing.");
        }
    }

    /// <summary>
    /// Resumes playback of the currently playing slide show if there is one. Call this to continue playback after Pause() has been called.
    /// </summary>
    public bool Resume()
    {
        if (_currentlyPlayingSlideShow != null)
        {
            if (_currentlyPlayingSlideShow.SlideCount == 0)
            {
                Debug.LogError($"SlideShowPlayer.Resume() was called on slide show \"{_currentlyPlayingSlideShow.name}\", but it contains no slides!");
                return false;
            }

            if (!(_currentSlideShowStateInfo.State != PlayState.Playing))
            {
                _currentSlideShowStateInfo.State = PlayState.Playing;
                FireSlideShowStartedEvent();

                return true;
            }
            else
            {
                Debug.LogWarning("SlideShowPlayer.Resume() was called, but the slide show is already playing.");
                return false;
            }
        }
        else
        {
            Debug.LogWarning("SlideShowPlayer().Resume() was called while no SlideShow is playing.");
            return false;
        }
    }

    /// <summary>
    /// Stops the currently playing slide show. Use Pause() instead if you need to resume playback where it left off.
    /// </summary>
    public void Stop()
    {
        if (_currentlyPlayingSlideShow != null)
        {
            _currentSlideShowStateInfo.Clear();
            FireSlideShowStoppedEvent();
        }
        else
        {
            Debug.LogWarning("SlideShowPlayer().Stop() was called while no cutscene is playing.");
        }
    }

    /// <summary>
    /// Called when a slide show starts playing.
    /// </summary>
    private void FireSlideShowStartedEvent()
    {
        OnSlideShowStarted?.Invoke(this, new SlideShowPlayerEventArgs() { SlideShow = _currentlyPlayingSlideShow });
    }

    /// <summary>
    /// Called when a slide show gets paused.
    /// </summary>
    private void FireSlideShowPausedEvent()
    {
        OnSlideShowPaused?.Invoke(this, new SlideShowPlayerEventArgs() { SlideShow = _currentlyPlayingSlideShow });
    }

    /// <summary>
    /// Called when a slide show is stopped via Stop() or playback finishes.
    /// </summary>
    private void FireSlideShowStoppedEvent()
    {
        OnSlideShowStopped?.Invoke(this, new SlideShowPlayerEventArgs() { SlideShow = _currentlyPlayingSlideShow });
    }

    private IEnumerator PresentSlideShow()
    {
        _isPlaying = true;
        _stopReason = StopReasons.None; // This value gets changed if Pause() or Stop() are called, telling this method to exit, and the appropriate events get fired.

        // Fire the slide show started event.
        FireSlideShowStartedEvent();

        yield return StartCoroutine(FadeInSlideShow());
        
        float slideDuration = _currentSlideShowStateInfo.GetCurrentSlideDisplayTime();


        while (_currentSlideShowStateInfo.ElapsedTime <= _currentlyPlayingSlideShow.SlideShowLength)
        {
            // Stop playing if the Pause() or Stop() method got called.
            if (_stopReason != StopReasons.None)
            {
                FireAppropriateStopEvent();
                break;
            }

            // Check if it is time to switch to the next slide.
            if (_currentSlideShowStateInfo.CurrentSlideElapsedTime >= slideDuration)
            {
                _currentSlideShowStateInfo.MoveToNextSlide();
                slideDuration = _currentSlideShowStateInfo.GetCurrentSlideDisplayTime();

                _slideShowUI.UpdateSlideDisplay(_currentlyPlayingSlideShow.GetSlide(_currentSlideShowStateInfo.CurrentSlideIndex));
            }


            // If the slide show is paused, or a transition is in progress, then simply wait until the next frame and then start a new iteration of this loop.
            if (_currentSlideShowStateInfo.State == PlayState.Paused || _slideShowUI.IsTransitioning)
            {
                // If we are not paused, but in a transition, then we need to keep incrementing the timers in the state info.
                if (_slideShowUI.IsTransitioning)
                {
                    _currentSlideShowStateInfo.IncrementTimers(Time.deltaTime);
                }

                yield return null; 
                continue;
            }

            _currentSlideShowStateInfo.IncrementTimers(Time.deltaTime);


            //Debug.Log($"{_currentSlideShowStateInfo.ElapsedTime} / {_currentlyPlayingSlideShow.Duration}   {_currentSlideShowStateInfo.CurrentSlideElapsedTime}");



            // Check if we should start a slide transition.
            if (!_slideShowUI.IsTransitioning &&
                _currentSlideShowStateInfo.CurrentSlideElapsedTime >= _currentSlideShowStateInfo.GetCurrentSlideTransitionTime())
            {
                if (_currentSlideShowStateInfo.CurrentSlideIndex >= _currentlyPlayingSlideShow.SlideCount - 1)
                {
                    // We are on the last frame, so break out of the loop to fade out the slide show.
                    break;
                }

                _slideShowUI.StartSlideTransition(_currentSlideShowStateInfo);
            }


            //Debug.Log($"{_currentSlideShowStateInfo.State}    {_currentSlideShowStateInfo.CurrentSlideIndex}    {_currentSlideShowStateInfo.CurrentSlideElapsedTime}    {_currentSlideShowStateInfo.ElapsedTime}");

            //EnableSlideDisplay(true);

            yield return null;

        } // end while


        yield return StartCoroutine(FadeOutSlideShow());


        // Fire the slide show stopped event.
        FireSlideShowStoppedEvent();
        
        _isPlaying = false;
    }

    private void FireAppropriateStopEvent()
    {
        if (_stopReason == StopReasons.Paused)
            FireSlideShowPausedEvent();

        if (_stopReason == StopReasons.Stopped)
            FireSlideShowStoppedEvent();
    }

    private IEnumerator FadeInSlideShow()
    {
        _slideShowUI.ResetSlideDisplay(_currentlyPlayingSlideShow.FadeColor);

        // We need the slide invisible until the initial fade in has finished.
        _slideShowUI.EnableSlideDisplay(false);


        // First, fade out the game view.
        yield return StartCoroutine(_slideShowUI.DoFade(Slide.FadeType.FadeOut, 
                                                        _currentlyPlayingSlideShow.DefaultFadeOutTime));


        _slideShowUI.EnableSlideDisplay(true);
        _slideShowUI.UpdateSlideDisplay(_currentlyPlayingSlideShow.GetSlide(_currentSlideShowStateInfo.CurrentSlideIndex));

        // Fade in the first slide.
        float fadeInTime = _currentSlideShowStateInfo.GetCurrentSlideFadeInTime();
        yield return StartCoroutine(_slideShowUI.DoFade(Slide.FadeType.FadeIn, 
                                                        fadeInTime));

        _currentSlideShowStateInfo.IncrementTimers(fadeInTime);        
    }

    private IEnumerator FadeOutSlideShow()
    {
        // Fade out the last slide.
        yield return StartCoroutine(_slideShowUI.DoFade(Slide.FadeType.FadeOut, 
                                                        _currentSlideShowStateInfo.GetCurrentSlideFadeOutTime()));

        // Disable the slide display image components in the UI so when we fade out, the game view will be visible now instead of the slide display UI.
        _slideShowUI.EnableSlideDisplay(false);

        // Finally, fade back to the game view.
        yield return StartCoroutine(_slideShowUI.DoFade(Slide.FadeType.FadeIn, 
                                                        _currentlyPlayingSlideShow.DefaultFadeInTime));

        _slideShowUI.DisableSlideShowDisplay();

        _currentSlideShowStateInfo.Clear();
    }



    /// <summary>
    /// Returns the number of slide shows in the list.
    /// </summary>
    public int SlideShowCount { get { return _slideShows.Count; } }


} // end class SlideShowPlayer

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;



public class CutScenePlayerEventArgs : EventArgs
{
    public PlayableDirector CutScene;
}


/// <summary>
/// This class allows us to easily play cutscenes that are defined via Cinemachine's timeline feature.
/// It uses Cinemachine's timeline feature, which is very powerful and easy to use once you
/// do the tutorial linked below and get the hang of it.
/// </summary>
/// <remarks>
/// To create a new cut scene, create an empty game object. No right click on it in the hierarchy.
/// Now open the Window menu and select Window->Sequency->Timeline. This window is very similar
/// to the Unity animation window, except that it uses tracks. In this way, it is like video
/// editing software. Your new timeline object can be added into the _CutScenes list of this class.
/// On the timeline GameObject, there is a PlayableDirector component that was automatically added.
/// Make sure its PlayOnAwake setting is off if you don't want it to play immediately on scene load.
/// 
/// To trigger an animator from the timeline, add the GameObject with the animation to the timeline.
/// Just drag it into the left pane, and it will give you a menu to select the type of track to
/// create. Choose animation track.
/// 
/// To make it work, you can place signal emitters in the timeline editor's markers bar (just below the
/// frame numbers bar) if you toggle on the display of signal emitters. You then need to create an emitter
/// in the inspector to the right. You can also place these on any track to keep things more organized.
/// Now, go to the object with the animation and add a SignalReceiver component. In this component,
/// select the signal asset you just created. Set the reaction to call the Animator's SetTrigger
/// method and type in the name of the trigger to fire. In the animator, create a trigger with
/// this name, and have a transition to the animation in question. On that transition, set
/// its condition to the trigger you just created. Now it should work.
/// 
/// You can also create a path for the camera by right-clicking in the hierarchy, and going
/// to Cinemachine->Dolly Track with Cart. It will make two separate game objects.
/// One is the cart, and the other the track it will follow. Select the Dolly Track object,
/// and create the points that will define your track. You can also increase the resolution
/// so the generated path between your track points will be smoother.
/// 
/// If you want the camera movement to be relative to an object like the player, then just drag
/// the dolly track onto that object to make it a child. This way you could, for example, make the
/// camera circle around the player even if he is moving since the track will now move with the
/// player.
/// 
/// Each cutscene should have its own Cinemachine Virtual Camera game object. Set the virtual
/// camera's lookat and follow properties to the Dolly Cart game object. The camera will now move
/// around the dolly track when the cutscene is activated.
/// 
/// NOTE: Dolly carts and objects moving via animators will not pause when you pause the cutscene.
///       This can easily be worked around by putting signal transmitters at the desired spots, 
///       in the timmeline and then making a handler that responds by pausing those objects.
///       I think this is because the PlayableDirector just has no way to know how to properly
///       pause each activation or animation track. This would be a problem if
///       the camera is being controlled by an activation track, for example, as it'd just turn off.
///  
/// You can get up to speed with the basics of creating cut scenes with cinemachine in an hour or
/// so with this Unity tutorial:
/// https://learn.unity.com/project/cutscenes-and-trailers-with-timeline-and-cinemachine?uv=2019.3
/// </remarks>
public class CutScenePlayer : MonoBehaviour
{
    public static CutScenePlayer Instance;



    public event EventHandler<CutScenePlayerEventArgs> OnCutSceneStarted;
    public event EventHandler<CutScenePlayerEventArgs> OnCutScenePaused;
    public event EventHandler<CutScenePlayerEventArgs> OnCutSceneStopped;


    [Tooltip("This list lets you specify all of the cutscenes in this scene. See the comments in CutScenePlayer.cs for info on creating cutscenes.")]
    [SerializeField] private List<PlayableDirector> _CutScenes = new List<PlayableDirector>();

    [Tooltip("This field is optional. It lets you specify a cutscene to be played immediately on scene load. See the comments in CutScenePlayer.cs for info on creating cutscenes.")]
    [SerializeField] private PlayableDirector _cutSceneToPlayOnStart;


    // This holds a reference to the currently playing cut scene. It will be null if there isn't one playing.
    private PlayableDirector _currentlyPlayingCutScene;



    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one CutScenePlayer in this scene! Self destructing.");
            Destroy(gameObject);

            return;
        }


        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_cutSceneToPlayOnStart != null)
            PlayCutScene(_cutSceneToPlayOnStart);
    }


    /// <summary>
    /// This gets the cutscene at the specified index in the cut scenes list.
    /// You can use this to access other features of the PlayableDirector that don't have
    /// shortcut functions in this class.
    /// </summary>
    /// <param name="index">The index of the cut scene to retreive from the cut scenes list.</param>
    public PlayableDirector GetCutScene(int index)
    {
        return _CutScenes[index];
    }

    /// <summary>
    /// This gets the cutscene whose GameObject has the specified name.
    /// You can use this to access other features of the PlayableDirector that don't have
    /// shortcut functions in this class.
    /// </summary>
    /// <param name="name">The name of the cut scene to get.</param>
    /// <returns>The cut scene whose GameObject has the specified name, or null if none is found.</returns>
    public PlayableDirector GetCutScene(string name)
    {
        for (int i = 0; i < _CutScenes.Count; i++)
        {
            if (_CutScenes[i].name == name)
                return _CutScenes[i];
        }


        return null;
    }

    /// <summary>
    /// Returns the currently playing cutscene if there is one.
    /// This function gives you accces to it so you can check its state, the current time playback is at, etc.
    /// </summary>
    /// <returns>The currently playing cutscene if there is one, or null otherwise.</returns>
    public PlayableDirector GetCurrentlyPlayingCutScene()
    {
        return _currentlyPlayingCutScene;
    }

    public bool IsPlaying()
    {
        return _currentlyPlayingCutScene != null && _currentlyPlayingCutScene.state == PlayState.Playing;
    }

    public bool IsPaused()
    {
        return _currentlyPlayingCutScene != null && _currentlyPlayingCutScene.state == PlayState.Paused;
    }

    public bool PlayCutScene(int index)
    {
        PlayableDirector cutScene = _CutScenes[index];

        return PlayCutScene(cutScene);
    }

    public bool PlayCutScene(string name)
    {
        PlayableDirector cutScene = GetCutScene(name);
        if (cutScene == null)
        {
            return false;
        }
        else
        {
            return PlayCutScene(cutScene);
        }
    }

    private bool PlayCutScene(PlayableDirector cutScene)
    {
        if (!(cutScene.state == PlayState.Playing))
        {
            ConnectCutSceneEvents(cutScene);

            cutScene.Play();
            _currentlyPlayingCutScene = cutScene;

            return true;
        }
        else
        {
            Debug.LogError($"Could not play the cut scene \"cutScene.Name\", as it is already playing!");
            return false;
        }
    }

    /// <summary>
    /// Pauses playback of the currently playing cutscene if there is one. Call Resume() to continue playback.
    /// </summary>
    public void Pause()
    {
        if (_currentlyPlayingCutScene != null)
        {
            _currentlyPlayingCutScene.Pause();
        }
        else
        {
            Debug.LogWarning("CutScenePlayer().Pause() was called while no cutscene is playing.");
        }
    }

    /// <summary>
    /// Resumes playback of the currently playing cutscene if there is one. Call this to continue playback after Pause() has been called.
    /// </summary>
    public void Resume()
    {
        if (_currentlyPlayingCutScene != null)
        {
            _currentlyPlayingCutScene.Resume();
        }
        else
        {
            Debug.LogWarning("CutScenePlayer().Resume() was called while no cutscene is playing.");
        }
    }

    /// <summary>
    /// Stops the currently playing cutscene. Use Pause() instead if you need to resume playback where it left off.
    /// </summary>
    public void Stop()
    {
        if (_currentlyPlayingCutScene != null)
        {
            _currentlyPlayingCutScene.Stop();
        }
        else
        {
            Debug.LogWarning("CutScenePlayer().Stop() was called while no cutscene is playing.");
        }
    }

    /// <summary>
    /// Called when a cut scene starts playing.
    /// </summary>
    /// <param name="cutScene">The cutscene that started playing.</param>
    private void CutSceneStarted(PlayableDirector cutScene)
    {
        OnCutSceneStarted?.Invoke(this, new CutScenePlayerEventArgs() { CutScene = cutScene });
    }

    /// <summary>
    /// Called when a cut scene gets paused.
    /// </summary>
    /// <param name="cutScene">The cutscene that was paused.</param>
    private void CutScenePaused(PlayableDirector cutScene)
    {
        OnCutScenePaused?.Invoke(this, new CutScenePlayerEventArgs() { CutScene = cutScene });
    }

    /// <summary>
    /// Called when a cut scene is stopped via Stop() or playback finishes.
    /// </summary>
    /// <param name="cutScene">The cutscene that stopped playing.</param>
    private void CutSceneStopped(PlayableDirector cutScene)
    {
        DisconnectCutSceneEvents(cutScene);

        OnCutSceneStopped?.Invoke(this, new CutScenePlayerEventArgs() { CutScene = cutScene });
    }


    private void ConnectCutSceneEvents(PlayableDirector cutScene)
    {
        cutScene.played += CutSceneStarted;
        cutScene.paused += CutScenePaused;
        cutScene.stopped += CutSceneStopped;
    }

    private void DisconnectCutSceneEvents(PlayableDirector cutScene)
    {
        cutScene.played -= CutSceneStarted;
        cutScene.paused -= CutScenePaused;
        cutScene.stopped -= CutSceneStopped;
    }



    /// <summary>
    /// Returns the number of cut scenes in the cut scenes list.
    /// </summary>
    public int CutSceneCount { get { return _CutScenes.Count; } }
}

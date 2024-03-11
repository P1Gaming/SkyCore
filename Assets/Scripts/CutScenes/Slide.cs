using System;
using UnityEngine;


/// <summary>
/// This class holds the settings for a single slide show slide.
/// </summary>
/// <remarks>
/// This class implements the ISerializationCallbackReceiver interface to allow it to initialize its fields
/// after serialization. Otherwise objects in a list have all their fields cleared by Unity after the constructor
/// has run, which is why I don't initialize them there.
/// </remarks>
[Serializable]
public class Slide : ISerializationCallbackReceiver
{
    [Tooltip("This is how long (in seconds) that this slide will be displayed for.")]
    [SerializeField]
    private float _displayTime;

    [Tooltip("The background color of this slide.")]
    [SerializeField]
    private Color _backgroundColor;


    [Header("Image Options")]
    [Tooltip("The image displayed by this slide.")]
    [SerializeField]
    private Sprite _image;

    [Tooltip("This sets how the image is aligned on the screen.")]
    [SerializeField]
    TextAnchor _ImageScreenAlignment;

    [Tooltip("This sets how the image is displayed.")]
    [SerializeField]
    private ImageDisplayModes _imageDisplayMode;

    [Tooltip("Whether or not to preserve the aspect ratio of the image. This is useful when ImageDisplayMode is set to modes like Stretch Full Screen or Custom Size")]
    [SerializeField]
    private bool _preserveAspectRatio;

    [Tooltip("This is the image size used when ImageDisplayMode is set to one of the custom size options.")]
    [SerializeField]
    private Vector2 _customImageDisplaySize;


    [Header("Transition Options")]
    [Tooltip("Specifies the type of transition between this slide and the next one.")]
    [SerializeField]
    private TransitionTypes _transitionType;

    [Tooltip("This allows you to override the TransitionDuration setting of the parent slide show. A negative value means this slide will use the slide show's default transition duration. NOTE: This value is not used by the CustomFadeInThenFadeOut transition type, as it uses the FadeIn/FadeOut time properties instead.")]
    [SerializeField]    
    private float _transitionDurationOverride;

    [Tooltip("This allows you to override the FadeInTime setting of the parent slide show. A negative value means this slide will use the slide show's default fade in time.")]
    [SerializeField]
    private float _fadeInTimeOverride;

    [Tooltip("This allows you to override the FadeOutTime setting of the parent slide show. A negative value means this slide will use the slide show's default fade out time.")]
    [SerializeField]
    private float _fadeOutTimeOverride;



    public void OnBeforeSerialize()
    {
        if (_backgroundColor == new Color(0,0,0,0))
        {
            _displayTime = 5f;
            _backgroundColor = Color.black;

            _imageDisplayMode = ImageDisplayModes.StretchFullScreen;
            _ImageScreenAlignment = TextAnchor.MiddleCenter;
            _preserveAspectRatio = true;
            _customImageDisplaySize = new Vector2(100, 100);

            _transitionType = TransitionTypes.SimpleFadeOutThenFadeIn;
            _transitionDurationOverride = -1f;

            _fadeInTimeOverride = -1f;
            _fadeOutTimeOverride = -1f;
        }
    }

    public void OnAfterDeserialize()
    {
    }



    public Color BackgroundColor { get { return _backgroundColor; } }
    public float DisplayTime { get { return _displayTime; } }
    
    public Sprite Image { get { return _image; } }
    public ImageDisplayModes ImageDisplayMode { get { return _imageDisplayMode; } }
    public TextAnchor ImageScreenAlignment { get { return _ImageScreenAlignment; } }
    public bool PreserveAspectRatio { get { return _preserveAspectRatio; } }
    public Vector2 CustomImageSize { get { return _customImageDisplaySize; } }

    public TransitionTypes TransitionType {  get { return _transitionType; } }
    public float TransitionDurationOverride { get { return _transitionDurationOverride; } }
    public float FadeInTimeOverride { get {  return _fadeInTimeOverride; } }
    public float FadeOutTimeOverride { get { return _fadeOutTimeOverride; } }

    public bool IsCustomSizeImage
    {
        get
        {
            return _imageDisplayMode == Slide.ImageDisplayModes.CustomSize;
        }
    }

    public bool IsFullscreenImage
    {
        get
        {
            return _imageDisplayMode == Slide.ImageDisplayModes.StretchFullScreen;
        }
    }



    public enum FadeType
    {
        FadeIn,
        FadeOut,
    }

    public enum ImageDisplayModes
    {
        NativeSize,
        StretchFullScreen,
        CustomSize,
    }

    public enum TransitionTypes
    {
        None,
        SimpleFadeOutThenFadeIn,
        CustomFadeOutThenFadeIn,
    }
}

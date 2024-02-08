using Sound;
using System.Linq;
//using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace UI.AudioSettings
{
    public class AudioSettingsUI : MonoBehaviour
    {
        public bool IsUIVisible {  get; private set; }
        
        [SerializeField]
        private UIDocument _uiDocument;
        [Space(15)]
        [Header("Audio options for when opening game for first time.")]
        [SerializeField]
        private bool _isOpeningScreen;
        [SerializeField]
        private int _sceneIndexToLoad = 1;
        [SerializeField]
        private float _defaultSoundVolume = 75f;
        [SerializeField] 
        private int _minSoundVolume = 0;
        [SerializeField] 
        private int _maxSoundVolume = 100;
        //[SerializeField]
        //private WwEvent _musicSoundEvent;
        //[SerializeField]
        //private WwEvent _sfxSoundEvent;
        //[SerializeField]
        //private AK.Wwise.CallbackFlags _endOfSoundEvent;
        

        private bool _isAudioPlaying = false;
        private bool _isDoneInitalizing = false;

        private bool _firstLoadFinished = false;

        private VisualElement _root;

        // UI Element references
        private VisualElement _settingsBackground;
        private Slider _soundSlider;
        private Slider _musicSlider;
        private Button _closeButton;
        private TextField _musicTextField;
        private TextField _soundTextField;

        // UI Element names
        private static readonly string _settingsBackgroundName = "settings-background";
        private static readonly string _musicSliderName = "music-slider";
        private static readonly string _soundSliderName = "sound-slider";
        private static readonly string _closeButtonName = "close-button";
        private static readonly string _musicTextFieldName = "music-field";
        private static readonly string _soundTextFieldName = "sound-field";

        private void Start()
        {
            if (_uiDocument == null)
            {
                _uiDocument = GetComponent<UIDocument>();
            }

            InitUIElements();

            if (_isOpeningScreen)
            {
                return;
            }
            ToggleUIVisibility(false);
        }

        /// <summary>
        /// Initialize UI Elements and bind functions.
        /// </summary>
        private void InitUIElements()
        {
            _root = _uiDocument.rootVisualElement;

            _musicSlider = _root.Q<Slider>(_musicSliderName);
            _soundSlider = _root.Q<Slider>(_soundSliderName);
            _settingsBackground = _root.Q<VisualElement>(_settingsBackgroundName);
            _closeButton = _root.Q<Button>(_closeButtonName);
            _musicTextField = _root.Q<TextField>(_musicTextFieldName);
            _soundTextField = _root.Q<TextField>(_soundTextFieldName);

            //_musicSlider.RegisterValueChangedCallback(OnMusicSliderValueChange);
            //_soundSlider.RegisterValueChangedCallback(OnSoundSliderValueChange);
            _settingsBackground.RegisterCallback<MouseEnterEvent>(OnMouseEnterUI);
            _settingsBackground.RegisterCallback<MouseLeaveEvent>(OnMouseLeaveUI);
            _closeButton.RegisterCallback<ClickEvent>(OnAudioSettingsClose);
            //_musicTextField.RegisterCallback<ChangeEvent<string>>(OnMusicTextFieldChange);
           //_soundTextField.RegisterCallback<ChangeEvent<string>>(OnSoundTextFieldChange);

            if(!_firstLoadFinished)
            {
                SetDefaultSoundValues();
            }
            else
            {
                SetCurrentValues();
            }
        }

        /// <summary>
        /// Sets default sound values at 75.
        /// </summary>
        private void SetDefaultSoundValues()
        {
            _musicSlider.value = _defaultSoundVolume;
            _soundSlider.value = _defaultSoundVolume;
            _isDoneInitalizing = true;
            _firstLoadFinished = true;
        }
        /// <summary>
        /// Sets current sound values from wwise.
        /// </summary>
        private void SetCurrentValues()
        {
            _musicSlider.value = SoundManager.Instance.GetMusicVolume();
            _soundSlider.value = SoundManager.Instance.GetSFXVolume();
        }


        /// <summary>
        /// Close audio settings.
        /// </summary>
        /// <param name="evt"></param>
        private void OnAudioSettingsClose(ClickEvent evt)
        {
            PauseManagement.Instance.DeterminePause();
        }

        /// <summary>
        /// Sets the music volume and plays an example sound 
        /// </summary>
        /// <param name="musicValue"></param>
        //private void SetMusicVolume(float musicValue)
        //{
        //    SoundManager.Instance.SetMusicVolume(musicValue);
        //    PlayExampleSound(_musicSoundEvent);
        //}

        /// <summary>
        /// Sets the sound volume and plays an example sound 
        /// </summary>
        /// <param name="soundValue"></param>
        //private void SetSoundVolume(float soundValue)
        //{
        //    SoundManager.Instance.SetMasterVolume(soundValue);
        //    PlayExampleSound(_sfxSoundEvent);
        //}

        /// <summary>
        /// Music volume slider.
        /// </summary>
        /// <param name="evt"></param>
        //private void OnMusicSliderValueChange(ChangeEvent<float> evt)
        //{
        //    _musicTextField.SetValueWithoutNotify(((int)evt.newValue).ToString());
        //    SetMusicVolume(evt.newValue);
        //}

        /// <summary>
        /// Sound volume slider.
        /// </summary>
        /// <param name="evt"></param>
        //private void OnSoundSliderValueChange(ChangeEvent<float> evt)
        //{
        //    _soundTextField.SetValueWithoutNotify(((int)evt.newValue).ToString());
        //    SetSoundVolume(evt.newValue);
        //}

        /// <summary>
        /// Music volume field.
        /// </summary>
        /// <param name="evt"></param>
        //private void OnMusicTextFieldChange(ChangeEvent<string> evt)
        //{
        //    if (int.TryParse(evt.newValue, out int musicVolume))
        //    {
        //        musicVolume = ClampVolumeToRange(musicVolume, _musicTextField);
        //        _musicSlider.SetValueWithoutNotify(musicVolume);
        //        SetMusicVolume(musicVolume);
        //    }
        //    else
        //    {
        //        _musicTextField.value = FilterNonNumericInput(evt.newValue);
        //    }
        //}

        /// <summary>
        /// Sound volume field.
        /// </summary>
        /// <param name="evt"></param>
        //private void OnSoundTextFieldChange(ChangeEvent<string> evt)
        //{
        //    if (int.TryParse(evt.newValue, out int soundVolume))
        //    {
        //        soundVolume = ClampVolumeToRange(soundVolume, _soundTextField);
        //        _soundSlider.SetValueWithoutNotify(soundVolume);
        //        SetSoundVolume(soundVolume);
        //    }
        //    else
        //    {
        //        _soundTextField.value = FilterNonNumericInput(evt.newValue);
        //    }
        //}

        /// <summary>
        /// Clamps the volume value between 0 and 100, updates the text field accordingly, and returns the clamped value.
        /// </summary>
        /// <param name="volume">The initial volume value.</param>
        /// <param name="textField">The text field where the volume value is displayed.</param>
        /// <returns>The clamped volume value.</returns>
        private int ClampVolumeToRange(int volume, TextField textField)
        {
            if (volume < _minSoundVolume)
            {
                volume = _minSoundVolume;
                textField.value = _minSoundVolume.ToString();
            }
            else if (volume > _maxSoundVolume)
            {
                volume = _maxSoundVolume;
                textField.value = _maxSoundVolume.ToString();
            }
            return volume;
        }

        /// <summary>
        /// Filters non numeric input from the inputfields.
        /// </summary>
        /// <param name="textInput"></param>
        private string FilterNonNumericInput(string textInput)
        {
            return new string(textInput.Where(c => char.IsDigit(c)).ToArray());
        }

        /// <summary>
        /// Mouse enters Sound UI.
        /// Prevents mouse from being stuck in middle of screen.
        /// </summary>
        /// <param name="evt"></param>
        private void OnMouseEnterUI(MouseEnterEvent evt)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        /// <summary>
        /// Mouse leaves UI.
        /// Locks mouse to game and hides it.
        /// </summary>
        /// <param name="evt"></param>
        private void OnMouseLeaveUI(MouseLeaveEvent evt)
        {
            LockMouseToWindow();
        }

        //TODO: Move to would make sense for this to live in Input system.
        /// <summary>
        /// Locks mouse to game window.
        /// </summary>
        private void LockMouseToWindow()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        /// <summary>
        /// Toggle visibility of Sound settings.
        /// </summary>
        /// <param name="value">true = visible, false = hidden</param>
        public void ToggleUIVisibility(bool value)
        {
            IsUIVisible = value;
            _root.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }

        /// <summary>
        /// Plays sample sounds when changing volume
        /// </summary>
        /// <param name="eventToPlay"></param>
        //private void PlayExampleSound(WwEvent eventToPlay)
        //{
        //    if (!_isDoneInitalizing)
        //    {
        //        return;
        //    }
        //    if (_isAudioPlaying)
        //    {
        //        return;
        //    }
        //    _isAudioPlaying = true;
        //   // eventToPlay.Post(gameObject, _endOfSoundEvent, OnFinishPlayingAudio);
        //}
        /// <summary>
        /// Prevents playing mutliple sounds at same time while changing sounds
        /// This event will be called once audio finishes playing
        /// </summary>
        /// <param name="in_cookie"></param>
        /// <param name="in_type"></param>
        /// <param name="in_info"></param>
        //private void OnFinishPlayingAudio(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
        //{
        //    if (in_type == AkCallbackType.AK_EndOfEvent)
        //    {
        //        _isAudioPlaying = false;
        //    }
        //}
        private void OnEnable()
        {
            if(_firstLoadFinished)
            {
                InitUIElements();
            }
            
        }
      
        public void VolumeSlider(float volume)
        {
            
            _soundTextField.value = volume.ToString("0.0");
        }
        public void SaveVolumeButton()
        {
            float volumeValue = _soundSlider.value;
            PlayerPrefs.SetFloat("volumeValue", volumeValue);
            LoadValues();
        }
        void LoadValues()
        {
            float volumeValue = PlayerPrefs.GetFloat("volumeValue");
            _soundSlider.value = volumeValue;
            AudioListener.volume = volumeValue;
        }
    }
}

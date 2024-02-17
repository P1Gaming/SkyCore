using Sound;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;
//using WwEvent = AK.Wwise.Event;

namespace UI.AudioSettings
{
    /// <summary>
    /// This class handles the UI of the settings menu's audio page.
    /// </summary>
    public class CanvasAudioSettings : MonoBehaviour
    {
        [Space(15)]
        [Header("Audio options for when opening game for first time.")]
        [SerializeField]
        private Slider _masterSlider;
        [SerializeField]
        private Slider _musicSlider;
        [SerializeField]
        private Slider _sfxSlider;
        [SerializeField]
        private Text _masterText;
        [SerializeField]
        private Text _musicText;
        [SerializeField]
        private Text _sfxText;
        //[SerializeField]
        //private WwEvent _musicSoundEvent;
        //[SerializeField]
        //private WwEvent _sfxSoundEvent;
        //[SerializeField]
        //private AK.Wwise.CallbackFlags _endOfSoundEvent;
        
        private bool _isAudioPlaying = false;
        private bool _isDoneInitalizing = false;

        private void Awake()
        {
            SetMasterVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MasterVolume, 
                                                 DefaultSettingsMenuValues.MasterVolume));
            SetMusicVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume, 
                                                DefaultSettingsMenuValues.MusicVolume));
            SetSFXVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.SfxVolume, 
                                              DefaultSettingsMenuValues.SoundVolume));

            _isDoneInitalizing = true;
        }

        /// <summary>
        /// This is called when the user clicks the Restore Defaults button on the audio settings page.
        /// </summary>
        public void ResetAllAudioSettings()
        {
            SetMasterVolume(DefaultSettingsMenuValues.MasterVolume);
            SetMusicVolume(DefaultSettingsMenuValues.MusicVolume);
            SetSFXVolume(DefaultSettingsMenuValues.SoundVolume);
        }

        /// <summary>
        /// Set the master volume.
        /// </summary>
        /// <param name="value"></param>
        public void SetMasterVolume(float value)
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MasterVolume, value);

            SoundManager.Instance.SetMusicVolume(value);
            _masterSlider.value = value;
            _masterText.text = $"{value}%";
        }
        /// <summary>
        /// Set the music volumes.
        /// </summary>
        /// <param name="value"></param>
        public void SetMusicVolume(float value)
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, value);

            SoundManager.Instance.SetMusicVolume(value);
            _musicSlider.value = value;
            _musicText.text = $"{value}%";
        }
        /// <summary>
        /// Set the sound effects volume.
        /// </summary>
        /// <param name="value"></param>
        public void SetSFXVolume(float value)
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.SfxVolume, value);

            SoundManager.Instance.SetSFXVolume(value);
            _sfxSlider.value = value;
            _sfxText.text = $"{value}%";
        }
        /// <summary>
        /// Handles playing sound events. Good for single use.
        /// </summary>
        /// <param name="value"></param>
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
        //    Debug.Log("Playing: " + eventToPlay.ToString());
        //    _isAudioPlaying = true;
        //    eventToPlay.Post(gameObject, _endOfSoundEvent, OnFinishPlayingAudio);
        //}
        /// <summary>
        /// Sends out post events
        /// </summary>
        /// <param name="value"></param>
        //private void OnFinishPlayingAudio(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
        //{
        //    if (in_type == AkCallbackType.AK_EndOfEvent)
        //    {
        //        _isAudioPlaying = false;
        //    }
        //}
        /// <summary>
        /// Slider Variable for Music
        /// </summary>
        /// <param name="value"></param>
        //public void ChangeMusicVolume(float value)
        //{
        //    SoundManager.Instance.SetMusicVolume(value);
        //    _musicText.text = value.ToString();
        //    PlayExampleSound(_musicSoundEvent);
        //}
        /// <summary>
        /// Slider Variable for Sounde Effects
        /// </summary>
        /// <param name="value"></param>
        //public void ChangeSFXVolume(float value)
        //{
        //    SoundManager.Instance.SetSFXVolume(value);
        //    _sfxText.text = value.ToString();
        //    PlayExampleSound(_sfxSoundEvent);
        //}
    }
}

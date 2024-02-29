using Sound;
using UnityEngine;
using UnityEngine.UI;

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
        [SerializeField]
        private AK.Wwise.Event _exampleSoundEvent;
        [SerializeField]
        private AK.Wwise.CallbackFlags _endOfSoundEvent;

        private bool _alreadyPlayingExampleSound = false;
        private bool _finishedAwake;

        private void Awake()
        {
            SetMasterVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MasterVolume, 
                                                 DefaultSettingsMenuValues.MasterVolume));
            SetMusicVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume, 
                                                DefaultSettingsMenuValues.MusicVolume));
            SetSFXVolume(PlayerPrefs.GetFloat(PlayerPrefsKeys.SfxVolume, 
                                              DefaultSettingsMenuValues.SoundVolume));
            _finishedAwake = true;
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
        public void SetMasterVolume(float value)
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MasterVolume, value);

            SoundManager.Instance.MasterVolume = value;
            _masterSlider.value = value;
            _masterText.text = $"{value}%";
        }
        /// <summary>
        /// Set the music volumes.
        /// </summary>
        public void SetMusicVolume(float value)
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.MusicVolume, value);

            SoundManager.Instance.MusicVolume = value;
            _musicSlider.value = value;
            _musicText.text = $"{value}%";
        }
        /// <summary>
        /// Set the sound effects volume.
        /// </summary>
        public void SetSFXVolume(float value)
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.SfxVolume, value);

            SoundManager.Instance.SFXVolume = value;
            _sfxSlider.value = value;
            _sfxText.text = $"{value}%";

            // I dunno whether this is the right way to do things in terms of the user's experience.
            // This might've just been for debugging originally, since I've never heard an example sound
            // with the old implementation.
            PlayExampleSound();
        }

        /// <summary>
        /// Handles playing sound events. Good for single use.
        /// </summary>
        private void PlayExampleSound()
        {
            if (!_finishedAwake || _alreadyPlayingExampleSound)
            {
                return;
            }
            Debug.Log("play example sound");
            _alreadyPlayingExampleSound = true;
            _exampleSoundEvent.Post(gameObject, _endOfSoundEvent, OnFinishPlayingAudio);
        }

        /// <summary>
        /// Sends out post events
        /// </summary>
        private void OnFinishPlayingAudio(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
        {
            if (in_type == AkCallbackType.AK_EndOfEvent)
            {
                _alreadyPlayingExampleSound = false;
            }
        }
    }
}

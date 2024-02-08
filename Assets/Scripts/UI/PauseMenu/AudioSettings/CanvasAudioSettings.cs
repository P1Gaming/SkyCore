using Sound;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cursor = UnityEngine.Cursor;
//using WwEvent = AK.Wwise.Event;

namespace UI.AudioSettings
{
    public class CanvasAudioSettings : MonoBehaviour
    {
        [Space(15)]
        [Header("Audio options for when opening game for first time.")]
        [SerializeField]
        private Slider _musicSlider;
        [SerializeField]
        private Slider _sfxSlider;
        [SerializeField]
        private Text _musicText;
        [SerializeField]
        private Text _sfxText;
        [SerializeField]
        private float _defaultSoundVolume = 75f;
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
            SetupMusicVolume(_defaultSoundVolume);
            SetupSFXVolume(_defaultSoundVolume);
            _isDoneInitalizing = true;
        }
        /// <summary>
        /// Setup default music volumes.
        /// </summary>
        /// <param name="value"></param>
        private void SetupMusicVolume(float value)
        {
            SoundManager.Instance.SetMusicVolume(value);
            _musicSlider.value = value;
            _musicText.text = value.ToString();
        }
        /// <summary>
        /// Setup default sound effects volumes.
        /// </summary>
        /// <param name="value"></param>
        private void SetupSFXVolume(float value)
        {
            SoundManager.Instance.SetSFXVolume(value);
            _sfxSlider.value = value;
            _sfxText.text = value.ToString();
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

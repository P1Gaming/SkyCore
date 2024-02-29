using UnityEngine;
using AK.Wwise;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField]
        private RTPC _masterVolumeParameter; // rtcp stands for real-time parameter control
        [SerializeField]
        private RTPC _musicVolumeParameter;
        [SerializeField]
        private RTPC _sfxVolumeParameter;

        /// <summary>
        /// Master bus volume. 0 - 100
        /// </summary>
        public float MasterVolume
        {
            get => _masterVolumeParameter.GetGlobalValue();
            set => _masterVolumeParameter.SetGlobalValue(value);
        }

        /// <summary>
        /// Music bus volume. 0 - 100
        /// </summary>
        public float MusicVolume
        {
            get => _musicVolumeParameter.GetGlobalValue();
            set => _musicVolumeParameter.SetGlobalValue(value);
        }

        /// <summary>
        /// SFX bus volume. 0 - 100
        /// </summary>
        public float SFXVolume
        {
            get => _sfxVolumeParameter.GetGlobalValue();
            set => _sfxVolumeParameter.SetGlobalValue(value);
        }





        private static SoundManager _instance;
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    // Need to do this if another script's Awake needs it before this
                    // one's Awake runs.
                    _instance = FindObjectOfType<SoundManager>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                _instance = this;
            }
            DontDestroyOnLoad(gameObject);


            MasterVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.MasterVolume, 0.75f);
            MusicVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.MusicVolume, 0.5f);
            SFXVolume = PlayerPrefs.GetFloat(PlayerPrefsKeys.SfxVolume, 0.75f);
        }
    }

}
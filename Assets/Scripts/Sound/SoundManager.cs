using UnityEngine;

namespace Sound
{
    public class SoundManager : MonoBehaviour
    {

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
        }

        /// <summary>
        /// Set master bus volume
        /// </summary>
        /// <param name="value">0 - 100</param>
        public void SetMasterVolume(float value)
        {
            
        }

        /// <summary>
        /// Set music bus volume
        /// </summary>
        /// <param name="value">0 - 100</param>
        public void SetMusicVolume(float value)
        {
            
        }

        /// <summary>
        /// Get music bus volume
        /// </summary>
        /// <param name="value">0 - 100</param>
        public float GetMusicVolume()
        {
            return 0;
        }

        /// <summary>
        /// Set sfx bus volume
        /// </summary>
        /// <param name="value">0 - 100</param>
        public void SetSFXVolume(float value)
        {
            
        }

        /// <summary>
        /// Get sfx bus volume
        /// </summary>
        /// <param name="value">0 - 100</param>
        public float GetSFXVolume()
        {
            return 0;
        }

        /// <summary>
        /// Set global Wwise game parameter value.
        /// </summary>
        /// <param name="parameter">AK.Wwise.RTPS parameter to change</param>
        /// <param name="value"></param>
        //private void SetGlobalSoundParameter(RTPC parameter, float value)
        //{
        //    parameter.SetGlobalValue(value);
        //}

        /// <summary>
        /// Set global Wwise game parameter value.
        /// </summary>
        /// <param name="parameter">AK.Wwise.RTPS parameter to change</param>
        /// <param name="value"></param>
        //private float GetGlobalSoundParameter(RTPC parameter)
        //{
        //    return parameter.GetValue(this.gameObject);
        //}
    }

}
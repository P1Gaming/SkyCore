using Sound;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Player.View;

namespace UI.ControlSettings
{
    /// <summary>
    /// This class handles the UI of the settings menu's controls page.
    /// </summary>
    public class CanvasControlSettings : MonoBehaviour
    {
        [Space(15)]
        [Header("Control options for when opening game for first time.")]
        [SerializeField]
        private Slider _viewSensitivitySlider;
        [SerializeField]
        private Text _viewSensitivityText;
        [SerializeField] 
        private Toggle _enableShakyCamToggle;


        #pragma warning disable
        private bool _isDoneInitalizing = false;

        private ShakyCamToggle _shakyCamToggle;



        private void Awake()
        {
            _shakyCamToggle = FindObjectOfType<ShakyCamToggle>();

            SetViewSensitivity(PlayerPrefs.GetFloat(PlayerPrefsKeys.FirstPersonViewSensitivity, 
                                                    DefaultSettingsMenuValues.FirstPersonViewSensitivity));            
            SetShakyCamEnabled(PlayerPrefs.GetInt(PlayerPrefsKeys.ShakyCamEnabled,
                                                  DefaultSettingsMenuValues.ShakyCamEnabled ? 1 : 0) > 0); // The "> 0" here just converts from int to bool.

            _isDoneInitalizing = true;
        }

        /// <summary>
        /// This is called when the user clicks the Restore Defaults button on the controls settings page.
        /// </summary>
        public void ResetAllControlsSettings()
        {
            SetViewSensitivity(DefaultSettingsMenuValues.FirstPersonViewSensitivity);
            SetShakyCamEnabled(DefaultSettingsMenuValues.ShakyCamEnabled);
        }

        /// <summary>
        /// Setup view sensitivity value.
        /// </summary>
        /// <param name="value"></param>
        public void SetViewSensitivity(float value)
        {
            PlayerPrefs.SetFloat(PlayerPrefsKeys.FirstPersonViewSensitivity, value);

            SoundManager.Instance.MusicVolume = value;
            _viewSensitivitySlider.value = value;

            // This line has to convert the slider's range into a percentage. So we subtract its minValue from both value and maxValue and then divide.
            _viewSensitivityText.text = $"{(value - _viewSensitivitySlider.minValue) / (_viewSensitivitySlider.maxValue - _viewSensitivitySlider.minValue) * 100:N0}%";

            
            FirstPersonView.Instance.SetSensitivity(value / 100);
            

        }
        public void SetShakyCamEnabled(bool value)
        {
            PlayerPrefs.SetInt(PlayerPrefsKeys.ShakyCamEnabled, 
                               value ? 1 : 0);

            _enableShakyCamToggle.isOn = value;


            if (_shakyCamToggle != null && _shakyCamToggle.IsShakyCamEnabled != value)
            {
                _shakyCamToggle.Toggle(value);
            }
        }

    }
}


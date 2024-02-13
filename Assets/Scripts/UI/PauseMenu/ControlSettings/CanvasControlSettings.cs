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

            SoundManager.Instance.SetMusicVolume(value);
            _viewSensitivitySlider.value = value;
            _viewSensitivityText.text = $"{value}%";

            FirstPersonView fpView = FindObjectOfType<FirstPersonView>();
            if (fpView != null)
                fpView.SetSensitivity(value / 100f);

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


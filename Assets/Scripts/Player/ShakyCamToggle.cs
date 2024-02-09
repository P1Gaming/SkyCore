using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ShakyCamToggle : MonoBehaviour
{
    [SerializeField, Tooltip("The shaky cam toggle")]
    Toggle _shakyCam;
    [SerializeField, Tooltip("The cinemachine camera that controls the shaking")]
    CinemachineVirtualCamera _cinemachine;
    //The key for the playerPref setting
    const string _playerPrefKey = "ShakyCamToggle";
    bool _isDestroyed = false;
    //The current thread
    System.Threading.Thread _thread; 
    // Start is called before the first frame update
    void Start()
    {
        _thread = System.Threading.Thread.CurrentThread;
        
        if (PlayerPrefs.GetInt(_playerPrefKey) == 0)
        {
            _shakyCam.isOn = false;
        }
        else if (PlayerPrefs.GetInt(_playerPrefKey) == 1)
        {
            _shakyCam.isOn = true;
        }
    }
    
    /**
     * This function is for when the player clicks the toggle button.
     * It checks if it is the same thread and if the toggle has not been destroyed
     * If the shaky cam is toggled, the camera shakes.
     * If it is not toggled, then the camera does not shake
     * For the player prefs, 0 means it is deactivated, 1 means that it is activated.
     * **/
    public void Toggle()
    {
        if (_thread == System.Threading.Thread.CurrentThread)
        {
            if (!_isDestroyed)
            {
                if (_shakyCam.isOn)
                {
                    //Cinemachine noise enabled
                    _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = .5f;
                    _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = .3f;
                    PlayerPrefs.SetInt(_playerPrefKey, 1);
                }
                else if (!_shakyCam.isOn)
                {
                    //Cinemachine noise disabled
                    _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                    _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;   
                    PlayerPrefs.SetInt(_playerPrefKey, 0);
                }
            }
        } 
    }
    private void OnDestroy()
    {
        _isDestroyed = true;   
    }

}

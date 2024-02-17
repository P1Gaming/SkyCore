using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ShakyCamToggle : MonoBehaviour
{
    [SerializeField, Tooltip("The cinemachine camera that controls the shaking")]
    CinemachineVirtualCamera _cinemachine;
    //The key for the playerPref setting
    bool _isDestroyed = false;
    //The current thread


    System.Threading.Thread _thread; 



    // Start is called before the first frame update
    void Start()
    {
        _thread = System.Threading.Thread.CurrentThread;        
    }
    
    /**
     * This function is for when the player clicks the toggle button.
     * It checks if it is the same thread and if the toggle has not been destroyed
     * If the shaky cam is toggled, the camera shakes.
     * If it is not toggled, then the camera does not shake
     * For the player prefs, 0 means it is deactivated, 1 means that it is activated.
     * **/
    public void Toggle(bool value)
    {
        if (_thread == System.Threading.Thread.CurrentThread)
        {
            if (!_isDestroyed)
            {
                IsShakyCamEnabled = value;

                if (value)
                {
                    //Cinemachine noise enabled
                    _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = .5f;
                    _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = .3f;
                }
                else if (!value)
                {
                    //Cinemachine noise disabled
                    if (_cinemachine == null)
                        Debug.Log("NULL");

                    _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                    _cinemachine.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = 0;   
                }
            }
        } 
    }
    private void OnDestroy()
    {
        _isDestroyed = true;   
    }


    public bool IsShakyCamEnabled { get; private set; }

}

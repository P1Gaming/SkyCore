using FiniteStateMachine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class HUD_Manager : MonoBehaviour
{
    public static HUD_Manager Instance;


    [Header("Drone Indicator Icon Settings")]
    
    [Tooltip("This is the icon that blinks when the drone wants your attention.")]
    [SerializeField]
    private Image _DroneIndicatorIcon;

    [Tooltip("This is how long (in seconds) it takes for the icon to blink once.")]
    [SerializeField, Min(0.01f)]
    private float _BlinkFrequency = 0.5f;


    private bool _DroneIndicatorIconEnabled;



    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is already another HUD_Manager in this scene. Self destructing...");
            Destroy(gameObject);
        }


        Instance = this;

        _DroneIndicatorIcon.gameObject.SetActive(false);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator DoDroneIndicatorBlinking()
    {
        float blinkTimer = 0f;


        while (_DroneIndicatorIconEnabled)
        {
            blinkTimer += Time.deltaTime;
            if (blinkTimer >= _BlinkFrequency)
            {
                blinkTimer = 0f;
                _DroneIndicatorIcon.gameObject.SetActive(!_DroneIndicatorIcon.gameObject.activeSelf);
            }


            yield return null;
        }


        _DroneIndicatorIcon.gameObject.SetActive(false);
        _DroneIndicatorIconEnabled = false;
    }



    public void EnableDroneIndicatorIcon(bool state)
    {
        _DroneIndicatorIconEnabled = state;

        if (state)
        {
            StartCoroutine(DoDroneIndicatorBlinking());
        }
        else
        {
            StopCoroutine(DoDroneIndicatorBlinking());
            _DroneIndicatorIcon.gameObject.SetActive(false);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using Player.View;

public class SensitivitySlider : MonoBehaviour
{
    [SerializeField]
    private Slider _sensSlider;
    [SerializeField]
    private Text _sensText;

    private FirstPersonViewNew _fPV;

    /// <summary>
    /// Set default value to the Slider.
    /// Can be used to retrieve the saved settings of player in the future.
    /// </summary>
    private void Awake()
    {
        _fPV = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonViewNew>();
        _sensSlider.value = _fPV.GetSensitivity() * 100f;
        _sensText.text = _sensSlider.value.ToString("F0");
    }

    /// <summary>
    /// Used by Unity Slider to dynamically change the value/
    /// </summary>
    /// <param name="value"></param>
    public void SensSlider(float value)
    {
        _sensText.text = value.ToString("F0");
        _fPV.SetSensitivity(value / 100f);
    }

}

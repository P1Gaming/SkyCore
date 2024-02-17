using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GeneratorHandler : MonoBehaviour
{
    private float maxCharge = 100.0f;
    private float currentCharge = 0.0f;
    [SerializeField] private Slider displaySlider;
    [SerializeField] private TMP_Text textDisplay;

    [ContextMenu("DebugCharge")]
    public void DebugCharge()
    {
        SetCharge(50.0f);
    }

    public void SetCharge(float setCharge)
    {
        currentCharge = setCharge;
        SetTextBar();
        SetPercentageBar();
    }

    private void SetTextBar()
    {
        textDisplay.text = GetPercentageText();
    }

    private void SetPercentageBar()
    {
        displaySlider.value = GetPercentageFloat();
    }

    private string GetPercentageText()
    {
        return ((currentCharge / maxCharge) * 100).ToString("F0") + "%";
    }

    private float GetPercentageFloat()
    {
        return currentCharge / maxCharge;
    }
}
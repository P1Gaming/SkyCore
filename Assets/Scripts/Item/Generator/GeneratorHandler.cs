using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class GeneratorHandler : MonoBehaviour
{
    private float maxCharge = 100.0f;
    private float currentCharge = 0.0f;
    private float targetCharge;
    [SerializeField] private Slider displaySlider;
    [SerializeField] private TMP_Text textDisplay;

    //Use these methods to increase the generator charge
    public void AddCharge(float setCharge)
    {
        targetCharge += setCharge;

        if (targetCharge > maxCharge)
        {
            targetCharge = maxCharge;
        }
        else if (targetCharge < 0)
        { 
            targetCharge = 0;
        }
    }

    public void SetCharge(float setCharge)
    {
        targetCharge = setCharge;

        if (targetCharge > maxCharge)
        {
            targetCharge = maxCharge;
        }
        else if (targetCharge < 0)
        {
            targetCharge = 0;
        }
    }

    //Changes the percentage of the text
    private void SetTextBar()
    {
        textDisplay.text = GetPercentageText();
    }
    //Changes the bar percentage
    private void SetPercentageBar()
    {
        displaySlider.value = GetPercentageFloat();
    }
    //Calculates what text to display
    private string GetPercentageText()
    {
        return ((currentCharge / maxCharge) * 100).ToString("F0") + "%";
    }
    //Calculates what percentage to display (ranges between 0 and 1)
    private float GetPercentageFloat()
    {
        return currentCharge / maxCharge;
    }

    //Lerp Bar Animation
    private void FixedUpdate()
    {
        currentCharge = Mathf.Lerp(currentCharge, targetCharge, 0.9f*Time.deltaTime);
        SetTextBar();
        SetPercentageBar();
    }
}
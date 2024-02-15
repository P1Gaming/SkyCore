using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


/// <summary>
/// This class contains click handlers for the main menu buttons that don't just change which settings page is open.
/// </summary>
public class SettingsMenuButtonsHandler : MonoBehaviour
{
    private PauseManagement _pauseManagement;


    private void Awake()
    {
        _pauseManagement = FindObjectOfType<PauseManagement>();
    }

    public void OnSaveClicked()
    {
        throw new NotImplementedException();
    }

    public void OnLoadClicked()
    {
        throw new NotImplementedException();
    }

    public void OnCloseClicked()
    {
        _pauseManagement.DeterminePause();
    }
}

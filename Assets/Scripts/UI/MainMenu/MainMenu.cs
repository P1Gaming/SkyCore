using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [Tooltip("The name of the scene to load when the user clicks on the Play button.")]
    [SerializeField] private string _sceneToLoadOnPlay;


    public void OnPlayClicked()
    {
        if (_sceneToLoadOnPlay != null)
        {
            SceneManager.LoadScene(_sceneToLoadOnPlay);
        }
        else
        {
            Debug.LogError("No scene has been specified to load when the play button is clicked!");
        }
    }

    public void OnSettingsClicked()
    {
        throw new NotImplementedException();
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }
}

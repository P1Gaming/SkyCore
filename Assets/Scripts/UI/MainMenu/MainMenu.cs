using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


/// <summary>
/// This class runs the UI of the main menu.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [Tooltip("The name of the scene to load when the user clicks on the Play button.")]
    [SerializeField] private string _sceneToLoadOnPlayClicked;

    [Tooltip("The object to enable to display the settings window.")]
    [SerializeField] private GameObject _panelToShowOnSettingsClicked;



    private PlayerActions _pauseAction;



    private void Awake()
    {   
        // This code makes it so pressing Esc will close the settings menu.
        _pauseAction = new PlayerActions();
        _pauseAction.UI.Pause.performed += OnPauseKeyPressed;
        _pauseAction.Enable();
    }

    public void OnPlayClicked()
    {
        if (_sceneToLoadOnPlayClicked != null)
        {
            SceneManager.LoadScene(_sceneToLoadOnPlayClicked);
        }
        else
        {
            Debug.LogError("No scene has been specified to load when the play button is clicked!");
        }
    }

    private void OnPauseKeyPressed(InputAction.CallbackContext context)
    {
        ToggleSettingsMenu(false);
    }

    public void OnSettingsClicked()
    {
        ToggleSettingsMenu(true);
    }

    public void OnQuitClicked()
    {
        Application.Quit();
    }

    private void ToggleSettingsMenu(bool state)
    {
        bool isAlreadyOpen = _panelToShowOnSettingsClicked.activeSelf;


        // If we are opening the settings menu, then hide the start menu by setting scale to 0.
        // Hiding it this way lets it stay active.
        transform.localScale = state ? Vector3.zero : Vector3.one;

        // Display or hide the settings menu.
        _panelToShowOnSettingsClicked.SetActive(state);


        // If we are opening the settings menu and it is not already open, then call a coroutine that will display the main menu again when the user closes the settings menu.
        if (state && !isAlreadyOpen)
            StartCoroutine(WaitForSettingsWindowToClose());
    }

    private IEnumerator WaitForSettingsWindowToClose()
    {
        // Wait for the settings menu to close.
        while (_panelToShowOnSettingsClicked.activeSelf)
        {
            yield return null; // Wait one frame.
        }


        // Display the main menu again.
        ToggleSettingsMenu(false);
    }


}

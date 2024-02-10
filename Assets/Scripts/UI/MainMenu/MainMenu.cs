using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [Tooltip("Just drag a scene into this field, and it will be loaded when the user clicks the Play button.")]
    [SerializeField] private SceneAsset _SceneToLoadOnPlay;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayClicked()
    {
        if (_SceneToLoadOnPlay != null)
            SceneManager.LoadScene(_SceneToLoadOnPlay.name);
        else
            Debug.LogError("No scene has been specified to load when the play button is clicked!");
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

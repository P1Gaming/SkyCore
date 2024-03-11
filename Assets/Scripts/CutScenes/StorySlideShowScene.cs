using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;


public class StorySlideShowScene : MonoBehaviour
{
    private SlideShowPlayer _slideShowPlayer;

    private void Awake()
    {
        _slideShowPlayer = FindObjectOfType<SlideShowPlayer>();
    }


    // Start is called before the first frame update
    void Start()
    {
        _slideShowPlayer.OnSlideShowStopped += OnSlideShowStopped;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnSlideShowStopped(object sender, SlideShowPlayerEventArgs e)
    {
        SceneManager.LoadScene("Main");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Cut : MonoBehaviour
{
    float speed = 12f;
    bool _one = true;
    // Update is called once per frame
    void Update()
    {
        speed -= Time.deltaTime;

        if (speed < 6f && _one)
        {
            SceneManager.LoadScene("WolfPistol");

            _one = false;
        }

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor.SearchService;
public class Move : MonoBehaviour
{

    public Canvas canvas;
    int index;
    float speed = 25f;

    float time = 6f;

    // Start is called before the first frame update
    public Image[] ImageA;
    float fiveSec = 25f;
    public Sprite[] S;
    public GameObject Wolf;
    public GameObject Pistol;
    public GameObject Wolf2;
    bool _one = true;
    bool _two = true;
    bool _three = true;
    bool _four = true;
    bool _five =true;
    void Start()
    {
        for (int i = 0; i < S.Length; i++)
        {
            S[i] = Resources.LoadAll<Sprite>("ImageA")[i];

        }

    }

    public void moveNext()
    {
        if (time >= 0 && _four)
        {
            ImageA[2].enabled = false;
            ImageA[0].enabled = false;
            ImageA[1].enabled = false;
            time -= Time.deltaTime;
            _four = false;
        }

    }

    public void MoveNext1()
    {

        if (fiveSec <= 20 && _one && _two && _three)
        {

            ImageA[0].enabled = true;
            _one = false;
        }
        else if (fiveSec <= 15 && _two)
        {
            ImageA[2].enabled = false;
            ImageA[0].enabled = false;
            ImageA[1].enabled = true;
            _two = false;
        }
        else if (fiveSec <= 10 && _three)
        {
            ImageA[0].enabled = false;
            ImageA[1].enabled = false;
            ImageA[2].enabled = true;
            _three = false;
        }
        else if (fiveSec <= 5 && _five)
        {
            ImageA[2].enabled = false;
            ImageA[0].enabled = false;
            ImageA[1].enabled = false;
           SceneManager.LoadScene("Main");
            _five = false;
        }
    
        fiveSec -= Time.deltaTime;
        int i = 0; i += 1;
    }
    // Update is called once per frame
    void Update()
    {
        moveNext();
        MoveNext1();


    }
}


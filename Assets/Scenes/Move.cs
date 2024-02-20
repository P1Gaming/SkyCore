using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Move : MonoBehaviour
{

    public Canvas canvas;
    int index;
    float speed = 12f;
    float speed1 =12f;
    float speed2 =12f;
    float time = 30f;
   
    // Start is called before the first frame update
    public Image[] ImageA;
    public Sprite[] S;
    public GameObject Wolf;
    public GameObject Pistol;
    public GameObject Wolf2;
    bool _one = true;
    bool _two = true;
    bool _three = true;
    bool _four = true;
    void Start()
    {
       
    }
    public void moveNext()
    {
        if (time <= 30 && _four)
        {
            ImageA[0].enabled = false;

            _four = false;

            ImageA[1].enabled = false;
            ImageA[2].enabled = false;  
        }
        time -= Time.deltaTime; 
    }
    public void moveNext1()
    {
        if (time <= 25 && _two)
        {
            ImageA[0].enabled = true;

            _two = false;

            ImageA[1].enabled = false;
            ImageA[2].enabled = false;

            

            //  yield return new WaitForSeconds(3f);
        }
        speed -= Time.deltaTime;
    }
    public void moveNext2()
    {
        if (time <= 20 && _one)
        {

            _one = false;
            ImageA[1].enabled = true;
            ImageA[0].enabled = false;
            ImageA[2].enabled = false;
            
            //  yield return new WaitForSeconds(3f);
        }
        time -= Time.deltaTime;

    }



    public void moveNext3()

    {

        if (time <= 15 && _three)
        {
            ImageA[2].enabled = true;
            ImageA[0].enabled = false;
            ImageA[1].enabled = false;
            // 
        }
        time -= Time.deltaTime;
    }
    public void moveNext4()

    {

        if (time <= 10 && _three)
        {
            ImageA[2].enabled = false;
            SceneManager.LoadScene("Main"); 
        }
        
    }
    // Update is called once per frame
    void Update()
    {   moveNext();
        moveNext1();
        moveNext2();
        moveNext3();
        moveNext4();

    }
       
}
    

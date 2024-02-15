using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Move : MonoBehaviour
{
    float speed = 35.0f;
    // Start is called before the first frame update
    public Texture[] Image;
    public Sprite[] S;
    public GameObject Wolf;
    public GameObject Pistol;
    bool _one = true;
    bool _two = false;
    void Start()
    {
        for (int i = 0; i < S.Length; i++)

            S[i] = Resources.LoadAll<Sprite>("Images")[i];

    }
    // Update is called once per frame
    void Update()
    {speed -=Time.deltaTime;
        if (speed < 15)
        {
           SceneManager.LoadScene("Main");
        }       
        transform.position += Vector3.right * 2f * Time.deltaTime;
        if (transform.position.x > 10)
        {
            transform.position = new Vector3(-1, 0, 0);
        }
        Pistol.transform.position += Vector3.right * 2f * Time.deltaTime;
        if (Pistol.transform.position.x > 10)
        {
            Pistol.transform.position = new Vector3(-1, 0, 0);
        }

    }
}

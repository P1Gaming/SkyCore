using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DewInstantiate : MonoBehaviour
{

    public GameObject Dew;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DewSpawn()
    {
        Instantiate(Dew, transform.position + (transform.right * 1), transform.rotation);
    }
}

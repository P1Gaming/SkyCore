using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    private static InteractionUI _instance;

    public static InteractionUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindWithTag("InteractionUI")?
                    .GetComponent<InteractionUI>();
            }
            return _instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

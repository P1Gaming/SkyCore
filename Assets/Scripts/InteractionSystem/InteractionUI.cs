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
                _instance = GameObject.FindWithTag("InteractionUI").GetComponent<InteractionUI>();
            }
            return _instance;
        }
    }

    private int _numberOfReasonsToBeInactive = 0;
    public int NumberOfReasonsToBeInactive
    {
        get => _numberOfReasonsToBeInactive;
        set
        {
            _numberOfReasonsToBeInactive = value;
            if (_numberOfReasonsToBeInactive < 0)
            {
                throw new System.Exception("In InteractionUI, _numberOfReasonsToBeInactive < 0: " + _numberOfReasonsToBeInactive);
            }
            gameObject.SetActive(_numberOfReasonsToBeInactive == 0);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// I think this code is just for the prompt "E" ui. So maybe this can be controlled by the PlayerInteraction script instead,
// if that's the only thing which uses the InteractionUI prefab.

// Should probably rename this and its prefab to InteractionPrompt or something.
// Probably shouldn't activate/deactivate the canvas gameobject via game event scriptable objects, because the code which needs
// the prompt to be shown shouldn't be decoupled from the prompt.


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
            //Debug.Log("# reasons InteractionUI inactive: " + value);
            if (_numberOfReasonsToBeInactive < 0)
            {
                throw new System.Exception("In InteractionUI, _numberOfReasonsToBeInactive < 0: " + _numberOfReasonsToBeInactive);
            }
            gameObject.SetActive(_numberOfReasonsToBeInactive == 0);
        }
    }
}

using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Currently a stub. Intended functionality:
/// Allows a gameobject to be found by InteractionFinder
/// Sends out an event when InteractionFinder finds and selects it
/// Holds a list of all possible interactions
/// </summary>

public class Interactable : MonoBehaviour
{
    int[] _interactions_;

    public PlayerInteraction LastInteractedWith { get; private set; }

    public void Interact(int index, PlayerInteraction interactingWith)
    {
        LastInteractedWith = interactingWith;

        if(index > -1 && index < _interactions_.Length)
        {
            DoInteractionThing(_interactions_[index]);
        }
        else
        {
            Debug.LogError("some interaction code done goofed majorly");
        }
    }

    private void DoInteractionThing(int index)
    {
        switch (index)
        {
            case 0: //taking berries from a berry bush
                {
                    if (!GetComponent<BerryBush>())//GetComponent<BerryBush>())
                    {
                        Debug.LogError("trying to do the berry bush interaction with no bush component");
                    }
                    else
                    {
                        GetComponent<BerryBush>().Harvest(LastInteractedWith);
                    }
                }
                break;
            case 1: //the next interaction to be implemented should go here
                Debug.LogError("something's trying to reference an interaction that doesn't exist");
                break;
            default:
                Debug.LogError("something's trying to reference an interaction that doesn't exist");
                break;
        }
    }
    private InteractionScriptableObject[] _interactions;
    
    [CanBeNull]
    public InteractionScriptableObject EvaluatePriority()
    {
        if (_interactions == null || _interactions.Length == 0)
        {
            Debug.LogWarning("Interactable object has no interactions registered.");
            return null;
        }

        return _interactions[0];
    }
}

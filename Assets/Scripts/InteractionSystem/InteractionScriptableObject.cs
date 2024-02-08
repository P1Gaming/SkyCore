using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Interaction", menuName = "ScriptableObjects/New Interaction")]
public class InteractionScriptableObject : ScriptableObject
{
    public string Action;
    public string InteractionHintText;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A component to put text on a gameobject to describe things.
/// </summary>
public class Comment : MonoBehaviour
{
    [TextArea(2, 10)]
    public string _comment;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingLevelManager : MonoBehaviour
{

    /// <summary> 
    /// This controls the current level of the crafting system
    /// Only recipes up to a certain level should be added
    /// Hence why we need a level management system
    ///   </summary>
    [SerializeField]
    private int _currentLevel;

    public int CraftingLevel { get { return _currentLevel; } set { _currentLevel = value;  } }

}

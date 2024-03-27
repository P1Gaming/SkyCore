using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Jellies;

///<summary>
/// Script: SlimeExperience.cs
/// Author: Michael Spangenberg (m_spangenberg)
/// Summary: A script that handles the exp gained by jellies and leveling up
/// This script does not take the jelly's loyalty, evolution, or hunger into account
/// It has not been tested on more than one jelly at the same time
/// Link to Trello Card: https://trello.com/c/FTKfqOR4/2513-programming-mspangenberg-jelly-leveling-and-experience-a-more-dynamic-system-between-player-and-jelly
/// TODO: Implement the save system when one is created
/// TODO: Implement other factors that could affect the leveling of a jelly, like evolution
/// </summary>
public class SlimeExperience : MonoBehaviour
{
    private Parameters _parameters;
    [Tooltip("The max level of the jelly")]
    [SerializeField]
    private int _maxLevel;
    [Tooltip("The current level of the jelly")]
    [SerializeField]
    private int _levelNum;
    [Tooltip("How much EXP the jelly has currently")]
    [SerializeField]
    private float _currentEXP;
    [Tooltip("How much EXP in total that the jelly needs to get to the next level")]
    [SerializeField]
    private float _expThreshold;
    [Tooltip("How much EXP the jelly still need to get to the next level")]
    [SerializeField]
    private float _expToNextLevel;
    [Tooltip("The multiplier for the EXP threshold to increase the EXP threshold after a level up")]
    [SerializeField]
    private float _expMultiplier = 1.5f;
    [Tooltip("A variable for testing the EXP system, not meant to be used in production code")]
    [SerializeField]
    private float _testEXPGain = 10f;
    [Tooltip("A variable for that when reached initiates EXP penalty formula")]
    [SerializeField]
    private int _consectuctiveInteractionThreshold = 5;
    [Tooltip("A variable that defines in seconds when to reset _consectuctiveInteractionCounter")]
    [SerializeField]
    private float _consectuctiveInteractionTimer = 30f;
    [Tooltip("A variable that determines how much penalty scales for interactions")]
    [SerializeField]
    private float _penaltyPercentage = 0.4f;

    private float _timeSinceLastInteraction = 0f;

    private int _consectuctiveInteractionCounter = 0;
    
    private string _previousEXPsource = "";

    public int LevelNum
    {
        get => _levelNum;
    }

    private void Awake()
    {
        _parameters = GetComponent<Parameters>();
        
        _expThreshold = _parameters.RequiredExpForNextLevel.Length > 0 ? _parameters.RequiredExpForNextLevel[_levelNum] : _expThreshold;
    }

    ///<summary>
    /// Timer that resets _consectuctiveInteractionCounter after EXP has not been gained in a while
    /// </summary>
    void Update()
    {
        #if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AddEXP(_testEXPGain, "Test Source");
            }
        #endif

        _timeSinceLastInteraction += Time.deltaTime;
        if (_timeSinceLastInteraction >= _consectuctiveInteractionTimer) 
        {
            //Debug.Log("_consectuctiveInteractionCounter reset");
            _timeSinceLastInteraction = 0f;
            _consectuctiveInteractionCounter = 0;
        }
    }
    ///<summary>
    /// Levels up the jelly if the jelly has not reached the max level
    /// </summary>
    private void LevelUp()
    { 
        float _extraEXP = _currentEXP - _expThreshold; //The extra EXP after a level up
        if(_levelNum < _maxLevel)
        {
            _levelNum++;

            // Used to handle if the array is empty or outside of range for the slimes current level
            int index = System.Math.Min(_levelNum - 1, _parameters.RequiredExpForNextLevel.Length - 1);
            _expThreshold = _parameters.RequiredExpForNextLevel[index];
           

            // Placeholder for notifying if the player has reached another level
            //Debug.Log("Your jelly has reached level " + _levelNum + "!");
        }
        
        _currentEXP = _extraEXP;
        _expThreshold *= _expMultiplier; 
        _expToNextLevel = GetEXPtoNextLevel();
    }
    /// <summary>
    /// Calculates the EXP that the jelly needs in order to get to the next level
    /// </summary>
    public float GetEXPtoNextLevel()
    {
        return _expThreshold - _currentEXP;
    }
    /// <summary>
    /// Adds additional EXP to the currentEXP
    /// If the EXP total reaches the EXP threshold, the level up function is called
    /// When consecutive interactions are done more than the _consectuctiveInteractionThreshold allows
    /// for, the jelly will get an EXP penalty.
    /// parameter EXP: The EXP gained from the action
    /// parameter EXPsource: The source the EXP comes from
    /// </summary>
    public void AddEXP(float EXP, string EXPsource)
    {
        _timeSinceLastInteraction = 0f;

        if (_previousEXPsource != EXPsource)
        {
            _consectuctiveInteractionCounter = 0;
        }
        else 
        {
            _consectuctiveInteractionCounter++;
        }

        _previousEXPsource = EXPsource;
        //Debug.Log("Source: " + EXPsource);

        if (_consectuctiveInteractionCounter >= _consectuctiveInteractionThreshold) 
        {
            float penalty = 1 / (1 + (_penaltyPercentage * (_consectuctiveInteractionCounter - _consectuctiveInteractionThreshold)));
            EXP *= penalty;
            //Debug.Log("Consecutive Counter: " + _consectuctiveInteractionCounter + "| EXP Given: " + EXP);
            // In the future, add jelly annoyance reaction here to indicate EXP penalty to player.
        }

        _currentEXP += EXP; 

        if (_currentEXP >= _expThreshold)
        {
            LevelUp();
        }
        _expToNextLevel = GetEXPtoNextLevel();
    }
}

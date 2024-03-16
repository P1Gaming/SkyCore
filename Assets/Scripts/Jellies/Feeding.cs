using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Player;

namespace Jellies
{
    /// <summary>
    /// Currently a stub. Intended functionality:
    /// Handling of Jelly feeding mechanic, checking if the player can feed the Jelly.
    /// If the player feeds the Jelly, then this handles the Jelly's reactions and output objects
    /// </summary>
    public class Feeding : MonoBehaviour
    {
        [SerializeField, Tooltip("Scriptable object representing" +
        " the type of item to feed the jelly")]
        private BerryItemIdentity _berryItemIdentity;

        private Parameters _parameters;

        private DewInstantiate _dew;

        private SlimeExperience _slimeExp;
        private void Awake()
        {
            _parameters = GetComponent<Parameters>();
            _dew = GetComponent<DewInstantiate>();
            _slimeExp = GetComponent<SlimeExperience>();
        }

        /// <summary>
        /// Feeds jelly by increasing it's food saturation.
        /// </summary>
        /// <param name="amountToIncrease">Amount of food to feed by, is same as saturation.</param>
        public void FeedJelly(float amountToIncrease)
        {
            _parameters.IncreaseFoodSaturation(amountToIncrease);
        }

        /// <summary>
        /// Called by the jelly prefab's feed button.
        /// </summary>
        public void OnFeedButton()
        {
            if (Inventory.Instance.TrySubtractItemAmount(_berryItemIdentity, 1))
            {
                FeedJelly(_berryItemIdentity.SaturationValue);

                // Used to handle if the array is empty or outside of range for the slimes current level
                try
                {
                    _dew.DewSpawn(_parameters.NumOfDewSpawnedAtLevel[_slimeExp.LevelNum - 1]);
                } catch (System.IndexOutOfRangeException)
                {
                    // Uses the highest level requirement if the slimes current level is outside of range
                    Debug.LogWarning("Property value \"NumOfDewSpawnedAtLevel\" array is either empty or outside of range for level num." +
                        "Using highest level requirement (" + _parameters.NumOfDewSpawnedAtLevel[_parameters.NumOfDewSpawnedAtLevel.Length - 1] +
                        ")");
                    _dew.DewSpawn(_parameters.NumOfDewSpawnedAtLevel[_parameters.NumOfDewSpawnedAtLevel.Length - 1]);
                }
            }
        }
    }
}



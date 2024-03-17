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


        public bool IsFull => _parameters.FoodSaturation == _parameters.MaxFoodSaturation;

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
        public bool TryFeedJelly(float amountToIncrease)
        {
            if (_parameters.FoodSaturation >= _parameters.MaxFoodSaturation)
            {
                if (_parameters.FoodSaturation > _parameters.MaxFoodSaturation)
                {
                    throw new Exception("In Feeding, _parameters.FoodSaturation >= _parameters.MaxFoodSaturation: "
                        + _parameters.FoodSaturation + " " + _parameters.MaxFoodSaturation);
                }
                return false;
            }

            _parameters.IncreaseFoodSaturation(amountToIncrease);

            int index = Math.Min(_parameters.NumOfDewSpawnedAtLevel.Length - 1, _slimeExp.LevelNum - 1);
            _dew.DewSpawn(_parameters.NumOfDewSpawnedAtLevel[index]);

            return true;
        }

        /// <summary>
        /// Called by the jelly prefab's feed button.
        /// </summary>
        public void OnFeedButton()
        {
            if (Inventory.Instance.TrySubtractItemAmount(_berryItemIdentity, 1))
            {
                TryFeedJelly(_berryItemIdentity.SaturationValue);
            }
        }
    }
}



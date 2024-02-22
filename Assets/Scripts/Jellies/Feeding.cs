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
        private ItemBase _berryItem;

        private Parameters _parameters;

        private DewInstantiate _dew;
        private void Awake()
        {
            _parameters = GetComponent<Parameters>();
            _dew = GetComponent<DewInstantiate>();
        }

        /// <summary>
        /// Feeds jelly by increasing it's food saturation.
        /// </summary>
        /// <param name="amountToIncrease">Amount of food to feed by, is same as saturation.</param>
        public void FeedJelly(float amountToIncrease)
        {
            _parameters.IncreaseFoodSaturation(amountToIncrease);
        }

        public void SpawnDew()
        {
            _dew.DewSpawn();
        }
        /// <summary>
        /// Called by the jelly prefab's feed button.
        /// </summary>
        public void OnFeedButton()
        {
            InventoryBase hotBar = InventoryScene.Instance.HotBar;
            if (hotBar.TrySubtractItemAmount(_berryItem, 1))
            {
                FeedJelly(_berryItem.SaturationValue);
                SpawnDew();
            }
        }
    }
}



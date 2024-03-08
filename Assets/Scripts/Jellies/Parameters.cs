using System;
using System.Collections;
using UnityEngine;

namespace Jellies
{
    /// <summary>
    /// Holds what are the different type of jellies are.
    /// </summary>
    [Tooltip("Holds what are the different type of jellies are.")]
    [Serializable]
    public enum JellyType
    {
        Base, Tumble, MonsterTumbleEvolutionTest
    }

    /// <summary>
    /// Handles updating and storing Jelly information. 
    /// It handles the type of Jelly a Jelly is, ensures Jelly Hunger levels are updated, with the Food Saturation decreasing at regular intervals, 
    /// and updates the Jelly experience system whenever the Jelly is fed.
    /// </summary>
    public class Parameters : MonoBehaviour
    {
        /// <summary>
        /// What type of jelly is it.
        /// </summary>
        [Tooltip("What type of jelly is it.")]
        [field: SerializeField]
        public JellyType jellyType
        {
            get;
            private set;
        }

        /// <summary>
        /// The number of dew spawned per interaction.
        /// </summary>
        [Tooltip("The number of dew spawned per interaction. Index 0 is assumed to be level 1, index 1 level 2 and so on.")]
        [field: SerializeField]
        public int[] NumOfDewSpawnedAtLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// The required experience per level
        /// </summary>
        [Tooltip("The required experience per level. Index 0 is assumed to be level 1, index 1 level 2 and so on.")]
        [field: SerializeField]
        public float[] RequiredExpForNextLevel
        {
            get;
            private set;
        }

        /// <summary>
        /// How well fed is the jelly. Decreases over time.
        /// </summary>
        [Tooltip("How well fed is the jelly. Decreases over time.")]
        public float FoodSaturation
        {
            get;
            private set;
        }

        /// <summary>
        /// How full can the jelly get. 
        /// </summary>
        [Tooltip("How full can the jelly get.")]
        [field: SerializeField]
        public float MaxFoodSaturation
        {
            get;
            private set;
        }

        /// <summary>
        /// How low can the food saturation get before the Jelly is hungry.
        /// </summary>
        [Tooltip("How low can the food saturation get before the Jelly is hungry.")]
        [SerializeField]
        private float _minFoodSaturation;

        /// <summary>
        /// How often to decrease the food saturation.
        /// </summary>
        [Tooltip("How often to decrease the food saturation.")]
        [SerializeField]
        private float _foodSaturationInterval;

        /// <summary>
        /// How much to decrease the Food Saturation by.
        /// </summary>
        [Tooltip("How much to decrease the Food Saturation by.")]
        [SerializeField]
        private float _foodSaturationDecrement;

        /// <summary>
        /// Xp Controller for the slime.
        /// TODO: Replace this with a Event
        /// </summary>
        [Tooltip("Xp Controller for the slime.")]
        [SerializeField]
        private SlimeExperience _slimeXp;

        /// <summary>
        /// Returns the type of Jelly.
        /// </summary>
        public JellyType TypeOfThisJelly()
        {
            return jellyType;
        }

        /// <summary>
        /// Feed the jelly a given amount.
        /// </summary>
        public void IncreaseFoodSaturation(float amount)
        {
            if(FoodSaturation + amount <= MaxFoodSaturation)
            {
                FoodSaturation += amount;
            }
            else
            {
                FoodSaturation = MaxFoodSaturation;
            }
            

            // TODO: Come back and make this more dynamic and replace with a event.
            if (_slimeXp == null && transform.GetComponentInChildren<SlimeExperience>())
            {
                _slimeXp = transform.GetComponentInChildren<SlimeExperience>();
            }
            else if (_slimeXp != null)
            {
                _slimeXp.AddEXP(10, "Red Berries");
            }
        }

        /// <summary>
        /// Jelly loses some food Saturation when going to the bathroom.
        /// </summary>
        public void DecreaseFoodSaturation(float amount)
        {
            FoodSaturation -= amount;
            if (FoodSaturation < 0)
            {
                FoodSaturation = 0;
            }
        }

        /// <summary>
        /// Set how full is the jelly's belly.
        /// Used for Unit Testing and can be removed if necessary
        /// </summary>
        public void SetFoodSaturation(float amount)
        {
            FoodSaturation = amount;
        }

        private void Awake()
        {
            MaxFoodSaturation = 100;
            FoodSaturation = MaxFoodSaturation;
        }

        /// <summary>
        /// Call on GameObject creation
        /// </summary>
        private void Start()
        {
            StartCoroutine(Digest());
        }

        /// <summary>
        /// A function that decreases the food satiation over time.
        /// </summary>
        /// <returns>Next time to run function</returns>
        IEnumerator Digest()
        {
            DecreaseFoodSaturation(_foodSaturationDecrement);
            yield return new WaitForSeconds(_foodSaturationInterval);
            StartCoroutine(Digest());
        }
    }
}
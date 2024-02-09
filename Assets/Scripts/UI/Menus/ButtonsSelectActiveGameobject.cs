using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Allows buttons to select which gameObject is active. It assumes 1 of multiple gameObjects is active
    /// at a time.
    /// </summary>
    public class ButtonsSelectActiveGameobject : MonoBehaviour
    {
        [SerializeField]
        private GameObject _firstActive;

        private GameObject _currentActive;

        private void Awake()
        {
            if (_firstActive != null)
            {
                _firstActive.SetActive(true);
            }
            _currentActive = _firstActive;
        }

        /// <summary>
        /// Sets one of a set of gameObjects active, and deactivates the prior active one. For use by UI buttons.
        /// </summary>
        public void SelectToBeActive(GameObject gameObject)
        {
            if (_currentActive != null)
            {
                _currentActive.SetActive(false);
            }
            gameObject.SetActive(true);
            _currentActive = gameObject;
        }
    }
}
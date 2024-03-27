using Jellies;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

namespace Jellies
{
    public class Petting : MonoBehaviour
    {
        private Parameters _parameter;

        private DewInstantiate _dew;

        private SlimeExperience _slimeExp;

        [SerializeField]
        private GameObject _pettingButton;

        [SerializeField, Tooltip("How often the player can pet the jelly")]
        private float _pettingCooldown;
        // Start is called before the first frame update
        private void Awake()
        {
            _parameter = GetComponent<Parameters>();
            _dew = GetComponent<DewInstantiate>();
            _slimeExp = GetComponent<SlimeExperience>();
            _parameter.CanBePet = true;
            //_pettingButton = GameObject.Find("PetButton").GetComponent<Button>();
        }

        // Update is called once per frame
        void Update()
        {
            
            
        }
        public void PetJelly()
        {
            if (_parameter.CanBePet)
            {
                _dew.DewSpawn(2);
                _parameter.CanBePet = false;
                _slimeExp.AddEXP(10, "Petting");
                StartCoroutine(Delay());
            }
        }
        private IEnumerator Delay()
        {
            //_pettingButton.gameObject.SetActive(false);
            yield return new WaitForSeconds(_pettingCooldown);
            //_pettingButton.gameObject.SetActive(true);
            _parameter.CanBePet = true;
        }
    }
}


using Jellies;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Jellies
{
    public class Petting : MonoBehaviour
    {
        private Parameters _parameter;

        private DewInstantiate _dew;

        private SlimeExperience _slimeExp;
        // Start is called before the first frame update
        private void Awake()
        {
            _parameter = GetComponent<Parameters>();
            _dew = GetComponent<DewInstantiate>();
            _slimeExp = GetComponent<SlimeExperience>();
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void PetJelly()
        {
            if (_parameter.CanBePet)
            {
                
            }
        }
        private IEnumerator Delay()
        {
            yield return null;
        }
    }
}


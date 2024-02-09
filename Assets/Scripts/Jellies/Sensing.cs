using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace Jellies
{
    public class Sensing : MonoBehaviour
    {
        public event Action<EventArgs> Sensed;


        public enum Sensor
        {
            Other,
        }

        public class EventArgs : System.EventArgs
        {
            public Sensor stimulus;
            public GameObject source;
            public bool status;
        }



        private void OnTriggerEnter(Collider other)
        {
            if (IsPlayer(other.gameObject))
            {
                Sensed?.Invoke(new EventArgs()
                {
                    stimulus = Sensor.Other,
                    source = other.gameObject,
                    status = true,
                });
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsPlayer(other.gameObject))
            {
                Sensed?.Invoke(new EventArgs()
                {
                    stimulus = Sensor.Other,
                    source = other.gameObject,
                    status = false,
                });
            }
        }

        private bool IsPlayer(GameObject obj)
        {
            return obj.CompareTag("Player");
        }
    }

}

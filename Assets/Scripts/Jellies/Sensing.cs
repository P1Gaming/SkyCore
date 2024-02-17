using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


namespace Jellies
{
    public class Sensing : MonoBehaviour
    {
        public event Action<EventArgs> Sensed;


        public class EventArgs : System.EventArgs
        {
            public bool closeToPlayer;
        }
        

        private void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other.gameObject))
            {
                return;
            }

            Sensed?.Invoke(new EventArgs()
            {
                closeToPlayer = true,
            });
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsPlayer(other.gameObject))
            {
                return;
            }

            Sensed?.Invoke(new EventArgs()
            {
                closeToPlayer = false,
            });
        }

        private bool IsPlayer(GameObject obj)
        {
            return obj.CompareTag("Player");
        }
    }

}

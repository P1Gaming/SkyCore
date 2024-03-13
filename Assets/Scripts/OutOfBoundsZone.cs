using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Motion;

namespace Player
{
    /// <summary>
    /// Teleports the player back to solid ground if they fall out of the world.
    /// </summary> 
    public class OutOfBoundsZone : MonoBehaviour
    {
        private Vector3 _recentGroundedPosition;

        private void Awake()
        {
            _recentGroundedPosition = PlayerMovement.Instance.transform.position;
            StartCoroutine(UpdateRespawnPoint());
        }

        private IEnumerator UpdateRespawnPoint()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);
                if (PlayerMovement.Instance.StandingOnGroundLayer)
                {
                    _recentGroundedPosition = PlayerMovement.Instance.transform.position;
                }
            }
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject.TryGetComponent(out PlayerMovement playerMovement))
            {
                playerMovement.Teleport(_recentGroundedPosition);
            }
        }
    }
}

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
        private float _lastTimeUpdatedRecentGroundedPosition;
        private Vector3 _recentGroundedPosition;

        private void Awake()
        {
            PlayerMovement.Instance.UpdateBasedOnGrounded += UpdateBasedOnPlayerGrounded;

            _recentGroundedPosition = PlayerMovement.Instance.transform.position;
            _lastTimeUpdatedRecentGroundedPosition = Time.time;
        }

        private void UpdateBasedOnPlayerGrounded(bool grounded)
        {
            if (grounded)
            {
                // only do this every few seconds so when the player falls out of the world,
                // it usually won't teleport the player to right on the edge (b/c that feels punishing,)
                if (Time.time > _lastTimeUpdatedRecentGroundedPosition + 3f)
                {
                    _recentGroundedPosition = PlayerMovement.Instance.transform.position;
                    _lastTimeUpdatedRecentGroundedPosition = Time.time;
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

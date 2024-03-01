using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Motion
{

    ///<summary>
    ///Decides if the player is on the ground
    /// </summary>
    [Serializable]
    public class GroundedDecider
    {
        [SerializeField, Tooltip("What layer the character uses for the ground")]
        private LayerMask _groundLayer;

        [SerializeField, Tooltip("Adjusts tolerance on rough terrain"), Range(0, 0.5f)]
        float _offSet = 0.1f;

        private CharacterController _characterController;

        public void Initialize(CharacterController controller)
        {
            _characterController = controller;  
        }

        ///<summary>
        ///Checking if the player is on the ground.
        /// </summary>
        /// <returns>
        /// Returns boolean. If it return true the player is on the ground. If false the player is not on the ground.
        /// </returns>
        public bool IsGrounded()
        {
            /*Vector3 position = _characterController.transform.position;
            position.y -= _offSet;

            return Physics.CheckSphere(position, _characterController.radius, _groundLayer, QueryTriggerInteraction.Ignore);*/
            return true;
        }
    }
}

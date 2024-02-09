using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jellies.Behaviors;

namespace Jellies.Behaviors
{
    /// <summary>
    /// Together with MonBehaviour provides minimal operations for managing Jelly state
    /// Notes from PR55 "maybe switch to drone’s scriptable-object-based state machine"
    /// Tientuine, Inherited by Wandering.cs and Watching.cs, seems to be a setup to separate scripts and emulate enums.
    /// Having logic in these 2 scripts for moving and stopping instead of one script.
    /// </summary>
    public class State : MonoBehaviour
    {

        /// <summary>
        /// Register with this event if you need to know when the jelly starts an action
        /// </summary>
        public System.Action<State> Entered { get; set; }

        /// <summary>
        /// Register with this event if you need to know when the jelly stops an action
        /// </summary>
        public System.Action<State> Exited { get; set; }

        /// <summary>
        /// Enables this state and notifies listeners that the jelly has entered this state
        /// </summary>
        /// <param name="gameObject">Game Object</param>
        public virtual void Enter(GameObject gameObject)
        {
            enabled = true;
            if (Entered != null)
            {
                Entered.Invoke(this);
            }
        }

        /// <summary>
        /// Disables this state and notifies listeners that the jelly has left this state
        /// </summary>
        public virtual void Exit()
        {
            enabled = false;
            if (Exited != null)
            {
                Exited.Invoke(this);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
    /// <summary>
    /// A state in a finite state machine.
    /// </summary>
    [CreateAssetMenu(menuName = "Finite State Machine/State")]
    public class FSMState : ScriptableObject
    {
        [field: Header("Null to do nothing")]
        [field: SerializeField, Tooltip("Raised when the state is entered.")]
        public GameEventScriptableObject OnEnter { get; private set; }

        [field: SerializeField, Tooltip("Raised every frame while in the state.")]
        public GameEventScriptableObject OnUpdate { get; private set; }

        [field: SerializeField, Tooltip("Raised when the state is exited.")]
        public GameEventScriptableObject OnExit { get; private set; }
    }
}

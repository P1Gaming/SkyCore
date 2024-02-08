using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
    /// <summary>
    /// A transition between states in a finite state machine.
    /// </summary>
    [CreateAssetMenu(menuName = "Finite State Machine/Transition")]
    public class FSMTransition : ScriptableObject
    {
        [SerializeField]
        private bool _disable;

        [Header("From = null means transition from any state.")]
        [SerializeField]
        private FSMState _from;

        [Header("To = null means transition to default state.")]
        [SerializeField]
        private FSMState _to;

        [SerializeField, Tooltip("All conditions must be met for the transition to happen.")]
        private FSMTransitionCondition[] _conditions;

        /// <summary>
        /// Checks whether this transition occurs, and resets trigger parameters if so.
        /// </summary>
        public void Check(FSMState currentState, FSMState defaultState
            , Dictionary<FSMParameter, bool> bools, Dictionary<FSMParameter, bool> triggers
            , Dictionary<FSMParameter, float> floats, ref FSMState newState)
        {
            if (_disable)
            {
                return;
            }

            // If _from is null, transition from any state, except from currentState to currentState.
            bool transitionFromAnyState = _from == null && currentState != _to;

            bool fromCorrectState = currentState == _from || transitionFromAnyState;
            if (!fromCorrectState)
            {
                return;
            }

            for (int i = 0; i < _conditions.Length; i++)
            {
                if (!_conditions[i].MeetsCondition(bools, triggers, floats))
                {
                    return;
                }
            }

            for (int i = 0; i < _conditions.Length; i++)
            {
                _conditions[i].CheckResetTrigger(triggers);
            }

            if (_to != null)
            {
                newState = _to;
            }
            else
            {
                newState = defaultState;
            }
        }
    }
}
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
        [field: SerializeField, Tooltip("Logs why the transition isn't happening, when at the required state (_from or any)")]
        public bool LogFailureReason { get; private set; }

        [Header("From = null means transition from any state.")]
        [SerializeField]
        private FSMState _from;

        [SerializeField]
        private FSMState _to;
        
        [Header("Min Duration")]
        [SerializeField, Tooltip("Must be in the state for this duration before can do this transition.")]
        private float _minDurationInFrom = 0;

        [SerializeField, Tooltip("Must be null or a float parameter. If not null, must be in the state for duration" +
            " of the parameter's value before can do this transition.")]
        private FSMParameter _parameterForMinDurationInFrom;

        [SerializeField, Tooltip("All conditions must be met for the transition to happen.")]
        private FSMTransitionCondition[] _conditions;

#if UNITY_EDITOR
        public bool Disable => _disable;
        public FSMState From => _from;
        public FSMState To => _to;
        public float MinDurationInFrom => _minDurationInFrom;
        public FSMParameter ParameterForMinDurationInFrom => _parameterForMinDurationInFrom;
        public FSMTransitionCondition[] Conditions => _conditions;
#endif

        public void CheckValid(Dictionary<FSMParameter, float> floatParameters)
        {
            if (_parameterForMinDurationInFrom != null)
            {
                if (!floatParameters.ContainsKey(_parameterForMinDurationInFrom))
                {
                    Debug.LogError("The FSMDefinition doesn't contain the parameter _parameterForMinDurationInFrom");
                }
                if (_parameterForMinDurationInFrom.Type != FSMParameter.ParameterType.Float)
                {
                    Debug.LogError("_parameterForMinDurationInFrom must be null or a float parameter", this);
                }
            }
        }

        public bool FromCorrectState(FSMState currentState)
        {
            // If _from is null, transition from any state, except from currentState to currentState.
            bool transitionFromAnyState = _from == null && currentState != _to;
            return currentState == _from || transitionFromAnyState;
        }

        /// <summary>
        /// Checks whether this transition occurs, and resets trigger parameters if so.
        /// </summary>
        public void Check(FSMState currentState, FSMState defaultState, float durationInCurrentState
            , Dictionary<FSMParameter, bool> bools, Dictionary<FSMParameter, bool> triggers
            , Dictionary<FSMParameter, float> floats, ref FSMState newState)
        {
            if (_disable)
            {
                if (LogFailureReason)
                {
                    Debug.Log("Failure reason: disabled", this);
                }
                return;
            }

            if (!FromCorrectState(currentState))
            {
                return;
            }

            float minDurationInFrom = _minDurationInFrom;
            if (_parameterForMinDurationInFrom != null)
            {
                minDurationInFrom = floats[_parameterForMinDurationInFrom];
            }
            if (durationInCurrentState < minDurationInFrom)
            {
                if (minDurationInFrom <= 0)
                {
                    throw new System.Exception("unexpected behaviour: if minDurationInFrom is 0," +
                        " it should have no effect");
                }

                if (LogFailureReason)
                {
                    Debug.Log("Failure reason: haven't been in current state long enough", this);
                }

                return;
            }


            for (int i = 0; i < _conditions.Length; i++)
            {
                if (!_conditions[i].MeetsCondition(bools, triggers, floats))
                {
                    if (LogFailureReason)
                    {
                        Debug.Log("Failure reason: don't meet condition at index " + i, this);
                    }

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
                throw new System.Exception("_to is null");
            }
        }
    }
}
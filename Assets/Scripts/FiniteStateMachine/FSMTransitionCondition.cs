using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
    /// <summary>
    /// A condition required for a transition to occur in a finite state machine.
    /// </summary>
    [System.Serializable] // to show in inspector
    public class FSMTransitionCondition
    {
        [SerializeField]
        private FSMParameter _parameter;

        [SerializeField]
        private bool _equalsForBoolParameter;

        [SerializeField]
        private FloatComparisonType _comparisonTypeForFloatParameter;

        [SerializeField]
        private FSMParameter _otherFloatParameterToCompareTo;

        [SerializeField]
        private float _comparedToForFloatParameter;

#if UNITY_EDITOR
        public FSMParameter Parameter => _parameter;
        public bool EqualsForBoolParameter => _equalsForBoolParameter;
        public FloatComparisonType ComparisonTypeForFloatParameter => _comparisonTypeForFloatParameter;
        public FSMParameter OtherFloatParameterToCompareTo => _otherFloatParameterToCompareTo;
        public float ComparedToForFloatParameter => _comparedToForFloatParameter;
#endif

        public enum FloatComparisonType
        {
            EqualTo, NotEqualTo, GreaterThan, LessThan, GreaterThanOrEqualTo, LessThanOrEqualTo
        }

        private void LogNonExistentParameter()
        {
            Debug.LogError("FSMTransitionCondition's parameter doesn't exist in the FSMDefinition.", _parameter);
        }

        /// <summary>
        /// Returns true if the parameter's provided value equals shouldEqual.
        /// </summary>
        private bool MeetsBoolOrTriggerCondition(Dictionary<FSMParameter, bool> boolsOrTriggers, bool shouldEqual)
        {
            if (!boolsOrTriggers.TryGetValue(_parameter, out bool value))
            {
                LogNonExistentParameter();
                return false;
            }
            return value == shouldEqual;
        }

        /// <summary>
        /// Returns true if the parameter's provided value meets the condition which this object represents.
        /// </summary>
        public bool MeetsCondition(Dictionary<FSMParameter, bool> bools
            , Dictionary<FSMParameter, bool> triggers
            , Dictionary<FSMParameter, float> floats)
        {
            if (_parameter.Type == FSMParameter.ParameterType.Bool)
            {
                return MeetsBoolOrTriggerCondition(bools, _equalsForBoolParameter);
            }

            if (_parameter.Type == FSMParameter.ParameterType.Trigger)
            {
                return MeetsBoolOrTriggerCondition(triggers, true);
            }

            if (_parameter.Type == FSMParameter.ParameterType.Float)
            {
                if (!floats.TryGetValue(_parameter, out float value))
                {
                    LogNonExistentParameter();
                    return false;
                }

                float compareTo = _comparedToForFloatParameter;
                if (_otherFloatParameterToCompareTo != null)
                {
                    compareTo = floats[_otherFloatParameterToCompareTo];
                }

                if (_comparisonTypeForFloatParameter == FloatComparisonType.EqualTo)
                {
                    return value == compareTo;
                }
                else if (_comparisonTypeForFloatParameter == FloatComparisonType.GreaterThan)
                {
                    return value > compareTo;
                }
                else if (_comparisonTypeForFloatParameter == FloatComparisonType.GreaterThanOrEqualTo)
                {
                    return value >= compareTo;
                }
                else if (_comparisonTypeForFloatParameter == FloatComparisonType.LessThan)
                {
                    return value < compareTo;
                }
                else if (_comparisonTypeForFloatParameter == FloatComparisonType.LessThanOrEqualTo)
                {
                    return value <= compareTo;
                }
                else if (_comparisonTypeForFloatParameter == FloatComparisonType.NotEqualTo)
                {
                    return value != compareTo;
                }
            }

            Debug.LogError("Somehow, no condition was checked in FSMTransitionCondition: " + ToString());

            return false;
        }

        /// <summary>
        /// If this condition's parameter is a trigger, resets the trigger for a finite state machine instance.
        /// </summary>
        public void CheckResetTrigger(Dictionary<FSMParameter, bool> triggers)
        {
            if (_parameter.Type == FSMParameter.ParameterType.Trigger)
            {
                triggers[_parameter] = false;
            }
        }
    }
}
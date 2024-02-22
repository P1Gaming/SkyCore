#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FiniteStateMachine;

namespace FiniteStateMachineEditor
{
    public class FSMEditorOneCondition : MonoBehaviour
    {
        [SerializeField]
        private GameObject _boolRequirement;
        [SerializeField]
        private GameObject _floatRequirements;
        [SerializeField]
        private TMP_Dropdown _parameterSelectionDropdown;
        [SerializeField]
        private TMP_Dropdown _boolRequirementDropdown;
        [SerializeField]
        private TMP_Dropdown _floatComparisonDropdown;
        [SerializeField]
        private TMP_InputField _floatComparedToInputField;
        [SerializeField]
        private TMP_Dropdown _comparedToParameterSelectionDropdown;
        [SerializeField]
        private TMP_Dropdown _comparedToParameterOrValueDropdown;
        [SerializeField]
        private Image _backgroundImage;
        [SerializeField]
        private Toggle _toggle;
        [SerializeField]
        private Color _selectedColor;

        private Color _normalColor;
        private FSMDefinition _fsmDefinition;
        private FSMEditor _fsmEditor;

        public FSMTransitionCondition Condition { get; private set; }


        public void Initialize(FSMTransitionCondition condition, FSMDefinition fsmDefinition, ToggleGroup toggleGroup
            , FSMEditor fsmEditor)
        {
            Condition = condition;
            _fsmDefinition = fsmDefinition;
            _fsmEditor = fsmEditor;

            _normalColor = _backgroundImage.color;
            _toggle.group = toggleGroup;

            UpdateParameterDropdownsAndCondition();
        }

        public void UpdateParameterDropdownsAndCondition()
        {
            SetParametersInDropdown(_fsmDefinition, _parameterSelectionDropdown);
            SetParametersInDropdownOnlyIncludingFloats(_fsmDefinition, _comparedToParameterSelectionDropdown);
            UpdateDisplayedCondition();
        }

        private void UpdateDisplayedCondition()
        {
            _parameterSelectionDropdown.SetValueWithoutNotify(GetParameterIndex(_fsmDefinition, Condition.Parameter));

            _boolRequirementDropdown.SetValueWithoutNotify(Condition.EqualsForBoolParameter ? 1 : 0);
            _floatComparisonDropdown.SetValueWithoutNotify((int)Condition.ComparisonTypeForFloatParameter);

            bool compareToOtherParameter = Condition.OtherFloatParameterToCompareTo != null;
            _comparedToParameterOrValueDropdown.SetValueWithoutNotify(compareToOtherParameter ? 1 : 0);
            if (compareToOtherParameter)
            {
                _comparedToParameterSelectionDropdown.SetValueWithoutNotify(
                    GetParameterIndexAmongstFloats(_fsmDefinition, Condition.OtherFloatParameterToCompareTo));
            }

            _boolRequirement.SetActive(Condition.Parameter.Type == FSMParameter.ParameterType.Bool);
            _floatRequirements.SetActive(Condition.Parameter.Type == FSMParameter.ParameterType.Float);
            _comparedToParameterSelectionDropdown.gameObject.SetActive(compareToOtherParameter);
            _floatComparedToInputField.gameObject.SetActive(!compareToOtherParameter);

            _floatComparedToInputField.SetTextWithoutNotify("" + Condition.ComparedToForFloatParameter);
        }

       

        public void OnConditionParameterDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionParameterDropdownChange(this, changeTo);
            UpdateDisplayedCondition();
        }
        public void OnConditionBoolRequirementDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionBoolRequirementDropdownChange(this, changeTo == 1 ? true : false);
            UpdateDisplayedCondition();
        }
        public void OnConditionFloatComparisonTypeDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionFloatComparisonTypeDropdownChange(this, changeTo);
            UpdateDisplayedCondition();
        }

        public void OnConditionInputFieldFloatValueToCompareToChange(string changeTo)
        {
            if (!float.TryParse(changeTo, out float asFloat))
                return;
            _fsmEditor.OnConditionInputFieldFloatValueToCompareToChange(this, asFloat);
            UpdateDisplayedCondition();
        }

        public void OnConditionFloatParameterToCompareToDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionFloatParameterToCompareToDropdownChange(this, changeTo);
            UpdateDisplayedCondition();
        }

        public void OnConditionCompareToValueOrParameterModeDropdownChange(int changeTo)
        {
            _fsmEditor.OnConditionCompareToValueOrParameterModeDropdownChange(this, changeTo, _comparedToParameterOrValueDropdown);
            UpdateDisplayedCondition();
        }




        public void OnSelectedToggle(bool toggleTo)
        {
            _backgroundImage.color = toggleTo ? _selectedColor : _normalColor;
            _fsmEditor.OnToggleSelectedConditionEditor(this, toggleTo);
        }




        // split out these static things

        public static int GetTransitionIndex(FSMDefinition fsmDefinition, FSMTransition transition)
        {
            for (int i = 0; i < fsmDefinition.Transitions.Length; i++)
            {
                if (fsmDefinition.Transitions[i] == transition)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetIndexOfDefaultState(FSMDefinition fsmDefinition)
        {
            for (int i = 0; i < fsmDefinition.EditorInfo.States.Length; i++)
            {
                if (fsmDefinition.EditorInfo.States[i] == fsmDefinition.DefaultState)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetStateIndex(FSMDefinition fsmDefinition, FSMState state)
        {
            for (int i = 0; i < fsmDefinition.EditorInfo.States.Length; i++)
            {
                if (fsmDefinition.EditorInfo.States[i] == state)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetParameterIndex(FSMDefinition fsmDefinition, FSMParameter parameter)
        {
            for (int i = 0; i < fsmDefinition.Parameters.Length; i++)
            {
                if (fsmDefinition.Parameters[i] == parameter)
                {
                    return i;
                }
            }
            return -1;
        }

        public static int GetParameterIndexAmongstFloats(FSMDefinition fsmDefinition, FSMParameter parameter)
        {
            if (parameter.Type != FSMParameter.ParameterType.Float)
            {
                throw new System.InvalidOperationException("must be a float parameter");
            }
            int result = 0;
            for (int i = 0; i < fsmDefinition.Parameters.Length; i++)
            {
                if (fsmDefinition.Parameters[i] == parameter)
                    return result;
                if (fsmDefinition.Parameters[i].Type == FSMParameter.ParameterType.Float)
                    result++;
            }
            throw new System.InvalidOperationException("shouldnt get here, is the parameter not included in the fsmDefinition?");
        }

        public static void SetParametersInDropdown(FSMDefinition fsmDefinition, TMP_Dropdown dropdown)
        {
            FSMParameter[] parameters = fsmDefinition.Parameters;
            dropdown.options.Clear();
            for (int i = 0; i < parameters.Length; i++)
                dropdown.options.Add(new TMP_Dropdown.OptionData(parameters[i].name));
            dropdown.RefreshShownValue();
        }

        public static void SetParametersInDropdownOnlyIncludingFloats(FSMDefinition fsmDefinition, TMP_Dropdown dropdown)
        {
            FSMParameter[] parameters = fsmDefinition.Parameters;
            dropdown.options.Clear();
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].Type == FSMParameter.ParameterType.Float)
                    dropdown.options.Add(new TMP_Dropdown.OptionData(parameters[i].name));
            }
            dropdown.RefreshShownValue();
        }

        public static int ConvertIndexOfParameterAmongstFloatsToAmongstAllParameters(FSMDefinition fsmDefinition, int indexAmongstFloats)
        {
            int nthFloat = 0;
            for (int i = 0; i < fsmDefinition.Parameters.Length; i++)
            {
                if (fsmDefinition.Parameters[i].Type == FSMParameter.ParameterType.Float)
                {
                    if (nthFloat == indexAmongstFloats)
                        return i;
                    nthFloat++;
                }
            }
            throw new System.InvalidOperationException("didnt find the float parameter");
        }

        public static bool HasAnyFloatParameter(FSMDefinition fsmDefinition)
        {
            foreach (FSMParameter parameter in fsmDefinition.Parameters)
            {
                if (parameter.Type == FSMParameter.ParameterType.Float)
                    return true;
            }
            return false;
        }

        public static int GetConditionIndex(FSMTransition transition, FSMTransitionCondition condition)
        {
            for (int i = 0; i < transition.Conditions.Length; i++)
            {
                if (transition.Conditions[i] == condition)
                    return i;
            }
            throw new System.InvalidOperationException("couldn't find the condition in the transition");
        }
    }
}
#endif
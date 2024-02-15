#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using FiniteStateMachine;

namespace FiniteStateMachineEditor
{
    public class FSMEditorOneParameter : MonoBehaviour
    {
        [SerializeField]
        private TMP_InputField _titleInputField;

        [SerializeField]
        private TMP_Dropdown _parameterTypeDropdown;
        [SerializeField]
        private TMP_Dropdown _initialBoolDropdown;
        [SerializeField]
        private TMP_Dropdown _initialTriggerDropdown;
        [SerializeField]
        private TMP_InputField _initialFloatInputField;


        [SerializeField]
        private Image _backgroundImage;
        [SerializeField]
        private Toggle _toggle;
        [SerializeField]
        private Color _selectedColor;

        private Color _normalColor;
        private FSMDefinition _fsmDefinition;
        private FSMEditor _fsmEditor;

        public FSMParameter Parameter { get; private set; }

        public void Initialize(FSMParameter parameter, FSMDefinition fsmDefinition, ToggleGroup toggleGroup
             , FSMEditor fsmEditor)
        {
            Parameter = parameter;
            _fsmDefinition = fsmDefinition;
            _fsmEditor = fsmEditor;

            _normalColor = _backgroundImage.color;
            _toggle.group = toggleGroup;

            _titleInputField.SetTextWithoutNotify(parameter.name);

            UpdateDisplayedInfo();
        }

        private void UpdateDisplayedInfo()
        {
            UpdateActiveGameObjectsBasedOnParameterType();

            _parameterTypeDropdown.SetValueWithoutNotify((int)Parameter.Type);
            _parameterTypeDropdown.RefreshShownValue();

            _initialBoolDropdown.SetValueWithoutNotify(Parameter.InitialBool ? 1 : 0);
            _initialBoolDropdown.RefreshShownValue();

            _initialTriggerDropdown.SetValueWithoutNotify(Parameter.InitialTrigger ? 1 : 0);
            _initialTriggerDropdown.RefreshShownValue();

            _initialFloatInputField.SetTextWithoutNotify("" + Parameter.InitialFloat);

        }

        private void UpdateActiveGameObjectsBasedOnParameterType()
        {
            _initialBoolDropdown.gameObject.SetActive(Parameter.Type == FSMParameter.ParameterType.Bool);
            _initialTriggerDropdown.gameObject.SetActive(Parameter.Type == FSMParameter.ParameterType.Trigger);
            _initialFloatInputField.gameObject.SetActive(Parameter.Type == FSMParameter.ParameterType.Float);
        }

        public void OnSelectedToggle(bool toggleTo)
        {
            _backgroundImage.color = toggleTo ? _selectedColor : _normalColor;
            _fsmEditor.OnToggleSelectedParameterEditor(this, toggleTo);
        }

        public void OnTitleInputFieldChange(string renameTo)
        {
            renameTo = renameTo.Trim();

            if (renameTo.Length == 0 || renameTo == Parameter.name)
            {
                _titleInputField.SetTextWithoutNotify(Parameter.name);
                
                // dunno why this is necessary, but otherwise when you select all text and hit enter, no text is shown
                _titleInputField.ActivateInputField();
                
                return;
            }

            string statePath = AssetDatabase.GetAssetPath(Parameter);

            string pathOfStateFolder = FSMEditor.PathOfFolder(Parameter);
            renameTo = FSMEditor.AdjustTitleToNotAlreadyExist(renameTo, pathOfStateFolder, out _);

            string errorMessage = AssetDatabase.RenameAsset(statePath, renameTo);
            if (errorMessage.Length != 0)
                Debug.LogWarning(errorMessage);

            _titleInputField.SetTextWithoutNotify(Parameter.name);

            _fsmEditor.OnParameterTitleChanged();
        }

        public void OnParameterTypeDropdownChanged(int changeTo)
        {
            FSMEditorSaveDataChanger.SetParameterType(Parameter, changeTo);
            UpdateActiveGameObjectsBasedOnParameterType();
            _fsmEditor.OnChangeParameterType();
        }

        public void OnInitialFloatInputFieldChanged(string changeTo)
        {
            if (!float.TryParse(changeTo, out float asFloat))
            {
                _initialFloatInputField.SetTextWithoutNotify("" + Parameter.InitialFloat);
                return;
            }
            FSMEditorSaveDataChanger.SetParameterInitialFloat(Parameter, asFloat);
        }

        public void OnInitialBoolDropdownChanged(int changeTo)
        {
            FSMEditorSaveDataChanger.SetParameterInitialBool(Parameter, changeTo == 0 ? false : true);
        }

        public void OnInitialTriggerDropdownChanged(int changeTo)
        {
            FSMEditorSaveDataChanger.SetParameterInitialTrigger(Parameter, changeTo == 0 ? false : true);
        }









    }
}
#endif
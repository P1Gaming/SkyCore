#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using TMPro;
using FiniteStateMachine;

// split this into pieces, which'll have their inspector references directly rather than via FSMEditorReferences

namespace FiniteStateMachineEditor
{
    public class FSMEditor : MonoBehaviour
    {
        private const string PATH_OF_FOLDER_FOR_ALL_STATE_MACHINES = "Assets\\Scriptable Objects\\State Machines";

        [SerializeField, Tooltip("The name of the state machine to create or edit. " +
            "It's stored in " + PATH_OF_FOLDER_FOR_ALL_STATE_MACHINES)]
        private string _stateMachineName;

        [SerializeField, Tooltip("The state machine to edit, if the above string is empty.")]
        private FSMDefinition _stateMachineDefinition;

        private FSMEditorReferences _refs;
        private CameraInfo _cameraInfo;
        private FSMEditorCameraControls _cameraControls;

        private List<FSMEditorOneState> _states = new();
        private List<FSMEditorOneTransition> _transitions = new();
        private List<FSMEditorOneCondition> _conditionsOfSelectedTransition = new();
        private List<FSMEditorOneParameter> _parameters = new();

        private FSMEditorOneState _dragging;
        private FSMEditorOneState _selectedState;
        private FSMEditorOneTransition _selectedTransition;
        private FSMEditorOneCondition _selectedCondition; // not mutually exclusive with having a selected state/transition
        private FSMEditorOneParameter _selectedParameter; // also not mutually exclusive with any of the others
        private List<FSMEditorOneState> _clickCandidates = new();
        private Vector2 _priorMouseWorldPos;
        private float _initialConditionsContentAreaHeight;
        private float _initialParametersContentAreaHeight;

        private FSMEditorOneTransition _transitionEditorToShowCreatingTransition;
        private FSMEditorOneState _creatingTransitionFrom;

        

        private void Awake()
        {
            _refs = FindObjectOfType<FSMEditorReferences>();
            _refs.UIToggle.SetActive(false); // to make the game window just black if don't initialize successfully

            if (!AssetDatabase.IsValidFolder(PATH_OF_FOLDER_FOR_ALL_STATE_MACHINES))
                throw new System.InvalidOperationException("The folder path for state machines doesn't exist.");

            if (_stateMachineName.Length != 0)
                _stateMachineDefinition = null;

            if (_stateMachineName.Length == 0 && _stateMachineDefinition == null)
            {
                EditorGUIUtility.PingObject(gameObject);
                Debug.LogError("Enter the state machine's name to create or edit one.");
                return;
            }

            if (_stateMachineDefinition == null)
                CreateOrFindStateMachineDefinition();
            if (_stateMachineDefinition == null)
                return;
            _refs.UIToggle.SetActive(true);

            _cameraInfo = new CameraInfo();
            _cameraControls = new FSMEditorCameraControls(_refs.Workspace
                , _refs.NonZoomedAreaLeftForSeeingWorkspace, _cameraInfo);

            _initialConditionsContentAreaHeight = _refs.ConditionsScrolledContentRect.rect.height;
            _initialParametersContentAreaHeight = _refs.ParametersScrolledContentRect.rect.height;

            AddVisualForState(null); // Anystate
            for (int i = 0; i < _stateMachineDefinition.EditorInfo.States.Length; i++)
                AddVisualForState(_stateMachineDefinition.EditorInfo.States[i]);

            for (int i = 0; i < _stateMachineDefinition.Transitions.Length; i++)
                AddVisualForTransition(_stateMachineDefinition.Transitions[i]);

            for (int i = 0; i < _stateMachineDefinition.Parameters.Length; i++)
                AddVisualForParameter(_stateMachineDefinition.Parameters[i]);
            UpdateParametersScrollContentHeight();

            _transitionEditorToShowCreatingTransition = CreateVisualForTransitionForCreatingNewTransitions();

            int defaultStateIndex = FSMEditorOneCondition.GetIndexOfDefaultState(_stateMachineDefinition);
            _cameraControls.MoveCameraToPutWorldPosInCenterOfScreenSpaceRect(
                _stateMachineDefinition.EditorInfo.StateEditorPositions[defaultStateIndex], _refs.NonZoomedAreaLeftForSeeingWorkspace);

            _priorMouseWorldPos = _cameraInfo.MouseWorldPosition;

            _refs.UIToggle.SetActive(true);
        }


        private void CreateOrFindStateMachineDefinition()
        {
            string pathOfStateMachineFolder = System.IO.Path.Combine(PATH_OF_FOLDER_FOR_ALL_STATE_MACHINES, _stateMachineName);
            string filenameOfFSMDefinition = $"{_stateMachineName}.asset";
            string pathOfFSMDefinition = System.IO.Path.Combine(pathOfStateMachineFolder, filenameOfFSMDefinition);

            if (AssetDatabase.IsValidFolder(pathOfStateMachineFolder))
            {
                _stateMachineDefinition = AssetDatabase.LoadAssetAtPath<FSMDefinition>(pathOfFSMDefinition);

                if (_stateMachineDefinition == null)
                {
                    Debug.LogError($"The state machine definition does not exist. Make sure there's an FSMDefinition scriptable object" +
                        $" in {pathOfStateMachineFolder}, and make sure its filename is {_stateMachineName}");
                }
                return;
            }

            bool createNew = EditorUtility.DisplayDialog("Create new state machine?"
                , $"Create a new state machine named \"{_stateMachineName}\"?", "OK", "Cancel");
            if (!createNew)
                return;

            string guid = AssetDatabase.CreateFolder(PATH_OF_FOLDER_FOR_ALL_STATE_MACHINES, _stateMachineName);
            if (guid.Length == 0)
            {
                Debug.LogError("Couldn't create the state machine's folder. Make sure the state machine's name can be a folder," +
                    " e.g. exclude symbols like >");
                return;
            }

            _stateMachineDefinition = ScriptableObject.CreateInstance<FSMDefinition>();
            AssetDatabase.CreateAsset(_stateMachineDefinition, pathOfFSMDefinition);
            AddState(out _);

            Debug.Log("Created new state machine in " + pathOfStateMachineFolder);
        }








        private void AddVisualForState(FSMState state)
        {
            GameObject instantiated = Instantiate(_refs.StateVisualPrefab, _refs.Workspace);
            FSMEditorOneState stateEditor = instantiated.GetComponent<FSMEditorOneState>();
            stateEditor.Initialize(state, _stateMachineDefinition);
            _states.Add(stateEditor);
        }

        private void AddVisualForTransition(FSMTransition transition)
        {
            GameObject instantiated = Instantiate(_refs.TransitionVisualPrefab, _refs.LinesFolder);
            FSMEditorOneTransition transitionEditor = instantiated.GetComponent<FSMEditorOneTransition>();
            transitionEditor.Initialize(transition, _states, _transitions, _cameraInfo);
            _transitions.Add(transitionEditor);
        }
        private FSMEditorOneTransition CreateVisualForTransitionForCreatingNewTransitions()
        {
            GameObject instantiated = Instantiate(_refs.TransitionVisualPrefab, _refs.LinesFolder);
            instantiated.SetActive(false);
            FSMEditorOneTransition result = instantiated.GetComponent<FSMEditorOneTransition>();
            result.InitializeSpecialOneForCreatingNewTransitions(_cameraInfo);
            return result;
        }

        private void AddVisualForTransitionCondition(FSMTransitionCondition condition)
        {
            GameObject instantiated = Instantiate(_refs.ConditionVisualPrefab, _refs.ConditionsParent);
            FSMEditorOneCondition conditionEditor = instantiated.GetComponent<FSMEditorOneCondition>();
            conditionEditor.Initialize(condition, _stateMachineDefinition, _refs.ConditionsToggleGroupForSelectingOne, this);
            _conditionsOfSelectedTransition.Add(conditionEditor);
        }

        private void AddVisualForParameter(FSMParameter parameter)
        {
            GameObject instantiated = Instantiate(_refs.ParameterVisualPrefab, _refs.ParametersParent);
            FSMEditorOneParameter parameterEditor = instantiated.GetComponent<FSMEditorOneParameter>();
            parameterEditor.Initialize(parameter, _stateMachineDefinition, _refs.ParametersToggleGroupForSelectingOne, this);
            _parameters.Add(parameterEditor);
        }

        private void Update()
        {
            if (_stateMachineDefinition == null)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _creatingTransitionFrom = null;
                _transitionEditorToShowCreatingTransition.gameObject.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                if (_selectedTransition != null)
                {
                    DeleteTransition(_selectedTransition);
                    for (int i = 0; i < _transitions.Count; i++)
                    {
                        _transitions[i].UpdateNthBetweenSameTwoStates();
                        _transitions[i].UpdateLine();
                    }
                }
                if (_selectedState != null && !_selectedState.IsForAnystate && _selectedState != _creatingTransitionFrom)
                {
                    if (_states.Count == 2) // 2 b/c one is for Anystate
                    {
                        Debug.LogWarning("Cannot delete the last state.");
                    }
                    else
                    {
                        DeleteStateAndAssociatedThings(_selectedState);
                    }
                }
            }


            float zoomOutBefore = _cameraInfo.ZoomOut;
            _cameraControls.OnUpdate();
            if (_cameraInfo.ZoomOut != zoomOutBefore)
            {
                foreach (FSMEditorOneTransition transition in _transitions)
                    transition.UpdateLineWidth();
            }

            bool clickingZoomableArea = Input.GetButtonDown("left click")
                && MouseIsInsideZoomableArea();

            CheckDragOneState(clickingZoomableArea);
            CheckChangeSelected(clickingZoomableArea);
            CheckUpdateLineShowingTransitionBeingCreated();

            
        }

        private void DeleteStateAndAssociatedThings(FSMEditorOneState stateEditor)
        {
            // delete it, all attached transitions, and the associated game events
            if (stateEditor == _selectedState)
            {
                _selectedState = null;
                _refs.SelectedStateEditorToggle.SetActive(false);
            }

            // delete the associated transitions
            FSMEditorOneTransition[] transitions = new FSMEditorOneTransition[stateEditor.ConnectedTransitions.Count];
            stateEditor.ConnectedTransitions.CopyTo(transitions);
            for (int i = 0; i < transitions.Length; i++)
                DeleteTransition(transitions[i]);

            // delete the associated game events
            if (stateEditor.State.OnEnter != null)
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(stateEditor.State.OnEnter));
            if (stateEditor.State.OnUpdate != null)
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(stateEditor.State.OnUpdate));
            if (stateEditor.State.OnExit != null)
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(stateEditor.State.OnExit));

            // reassign the default state
            if (stateEditor.State == _stateMachineDefinition.DefaultState)
            {
                FSMEditorOneState newDefaultState = _states[1]; // at index is 0 is the one for Anystate.
                if (newDefaultState == stateEditor)
                    newDefaultState = _states[2];
                FSMEditorSaveDataChanger.SetFSMDefinitionDefaultState(_stateMachineDefinition, newDefaultState.State);
                newDefaultState.UpdateWhetherDefaultState();
            }

            // delete the state
            int index = FSMEditorOneCondition.GetStateIndex(_stateMachineDefinition, stateEditor.State);
            FSMEditorSaveDataChanger.RemoveStateAtIndex(_stateMachineDefinition, index);

            _states.Remove(stateEditor);
            Destroy(stateEditor.gameObject);
        }

        private void DeleteTransition(FSMEditorOneTransition transitionEditor)
        {
            if (_selectedTransition == transitionEditor)
            {
                _selectedTransition = null;
                _refs.SelectedTransitionEditorToggle.SetActive(false);
            }

            int index = FSMEditorOneCondition.GetTransitionIndex(_stateMachineDefinition, transitionEditor.Transition);
            FSMEditorSaveDataChanger.RemoveTransitionAtIndex(_stateMachineDefinition, index);

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(transitionEditor.Transition));

            transitionEditor.From.RemoveDeletedTransition(transitionEditor);
            transitionEditor.To.RemoveDeletedTransition(transitionEditor);
            _transitions.Remove(transitionEditor);
            Destroy(transitionEditor.gameObject);
        }


        public void OnStartCreatingTransitionButton()
        {
            if (_selectedState == null)
                return;
            _creatingTransitionFrom = _selectedState;
            _transitionEditorToShowCreatingTransition.gameObject.SetActive(true);
            CheckUpdateLineShowingTransitionBeingCreated();
        }

        private void CheckUpdateLineShowingTransitionBeingCreated()
        {
            if (_creatingTransitionFrom == null)
                return;
            Vector2 posFrom = _creatingTransitionFrom.transform.position;
            Vector2 posTo = _cameraInfo.MouseWorldPosition;

            if (MouseIsInsideZoomableArea())
            {
                FSMEditorOneState stateBelowMouse = FindStateBelowMouse();
                if (stateBelowMouse == _creatingTransitionFrom)
                    stateBelowMouse = null;
                if (stateBelowMouse != null)
                {
                    int nthBelowMouse = FSMEditorOneTransition.NthBetweenSameTwoStates(_transitions, null
                        , _creatingTransitionFrom, stateBelowMouse);
                    Vector3 offset = FSMEditorOneTransition.AdjustmentToAvoidIdenticalLines(nthBelowMouse);
                    posTo = stateBelowMouse.transform.position + offset;
                    posFrom += (Vector2)offset;
                }
            }

            _transitionEditorToShowCreatingTransition.SetLine(posFrom, posTo);
        }

        public void OnChangeParameterType()
        {
            FSMEditorOneCondition.SetParametersInDropdownOnlyIncludingFloats(_stateMachineDefinition
                , _refs.TransitionMinTimeParameterSelectionDropdown);
            if (_selectedTransition != null)
            {
                foreach (FSMEditorOneCondition condition in _conditionsOfSelectedTransition)
                    condition.UpdateParameterDropdownsAndCondition();
            }
        }



        private bool MouseIsInsideZoomableArea()
        {
            return RectTransformUtility.RectangleContainsScreenPoint(_refs.NonZoomedAreaLeftForSeeingWorkspace, Input.mousePosition);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (_stateMachineDefinition == null)
                return;
            _cameraControls.OnApplicationFocus(focus);
        }

        private void CheckChangeSelected(bool clickingZoomableArea)
        {
            if (!clickingZoomableArea)
                return;
            FSMEditorOneState belowMouse = FindStateBelowMouse();
            if (belowMouse == _creatingTransitionFrom && belowMouse != null)
                return;

            if (_selectedState != null)
                _selectedState.SetSelected(false);
            if (_selectedTransition != null)
                _selectedTransition.SetWhetherSelected(false);

            _selectedState = belowMouse;
            if (_selectedState != null)
            {
                if (_creatingTransitionFrom == null)
                {
                    ShowVisualsForSelectedState();
                }
                else
                {
                    string transitionTitle = $"[{_creatingTransitionFrom.Name}] to [{_selectedState.Name}]";
                    string transitionPath = MakePathForScriptableObject(ref transitionTitle, "Transitions");
                    FSMTransition newTransition = ScriptableObject.CreateInstance<FSMTransition>();
                    AssetDatabase.CreateAsset(newTransition, transitionPath);
                    FSMEditorSaveDataChanger.SetFSMTransitionFromAndTo(newTransition, _creatingTransitionFrom.State, _selectedState.State);
                    FSMEditorSaveDataChanger.AddTransitionToFSMDefinition(_stateMachineDefinition, newTransition);

                    AddVisualForTransition(newTransition);

                    _creatingTransitionFrom = null;
                    _transitionEditorToShowCreatingTransition.gameObject.SetActive(false);

                    _selectedState = null;
                    _selectedTransition = _transitions[^1];

                }
            }
            else
                _selectedTransition = FindClickedTransition();
            

            _refs.SelectedStateEditorToggle.SetActive(_selectedState != null);
            

            if (_selectedTransition != null)
            {
                _selectedTransition.SetWhetherSelected(true);
                _refs.TransitionTitleText.text = $"{_selectedTransition.From.Name} -> {_selectedTransition.To.Name}";
                _refs.TransitionMinTimeIn1stStateInputField.SetTextWithoutNotify("" + _selectedTransition.Transition.MinDurationInFrom);
                _refs.TransitionDisableToggle.SetIsOnWithoutNotify(_selectedTransition.Transition.Disable);
                _refs.TransitionLogFailureReasonToggle.SetIsOnWithoutNotify(_selectedTransition.Transition.LogFailureReason);

                UpdateConditionsInScrollArea();

                FSMEditorOneCondition.SetParametersInDropdownOnlyIncludingFloats(_stateMachineDefinition
                    , _refs.TransitionMinTimeParameterSelectionDropdown);

                UpdateShownMinDuration();

            }
            _refs.SelectedTransitionEditorToggle.SetActive(_selectedTransition != null);

        }

        private void ShowVisualsForSelectedState()
        {
            _selectedState.transform.SetAsLastSibling();
            _selectedState.SetSelected(true);
            _refs.StateTitleInputField.SetTextWithoutNotify(_selectedState.Name);
            _selectedTransition = null;
        }

        

        private void UpdateConditionsInScrollArea()
        {
            Transform conditionsParent = _refs.ConditionsParent;
            for (int i = 0; i < conditionsParent.childCount; i++)
                Destroy(conditionsParent.GetChild(i).gameObject);
            _conditionsOfSelectedTransition.Clear();
            for (int i = 0; i < _selectedTransition.Transition.Conditions.Length; i++)
            {
                FSMTransitionCondition condition = _selectedTransition.Transition.Conditions[i];
                AddVisualForTransitionCondition(condition);
            }
            UpdateConditionsScrollContentHeight();
        }

        private void UpdateConditionsScrollContentHeight()
        {
            UpdateScrollContentHeight(_refs.ConditionsGridLayoutGroup
                , _conditionsOfSelectedTransition.Count, _refs.ConditionsScrolledContentRect
                , _initialConditionsContentAreaHeight);
        }

        private void UpdateParametersScrollContentHeight()
        {
            UpdateScrollContentHeight(_refs.ParametersGridLayoutGroup
                , _parameters.Count, _refs.ParametersScrolledContentRect, _initialParametersContentAreaHeight);
        }

        private void UpdateScrollContentHeight(UnityEngine.UI.GridLayoutGroup grid, int numGriddedItems, RectTransform scrolledContentArea
            , float minHeight)
        {
            float topAndBottomExtra = grid.padding.top + grid.padding.bottom;
            float forSpacing = System.Math.Max(0, numGriddedItems - 1) * grid.spacing.y;
            float forCellSize = numGriddedItems * grid.cellSize.y;

            float height = topAndBottomExtra + forSpacing + forCellSize;
            float newHeight = Mathf.Max(minHeight, height);
            scrolledContentArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        }

        private void UpdateShownMinDuration()
        {
            bool useParameterForMinDuration = _selectedTransition.Transition.ParameterForMinDurationInFrom != null;

            _refs.TransitionMinTimeParameterOrValueDropdown.SetValueWithoutNotify(useParameterForMinDuration ? 1 : 0);

            if (useParameterForMinDuration)
            {
                int index = FSMEditorOneCondition.GetParameterIndexAmongstFloats(_stateMachineDefinition
                    , _selectedTransition.Transition.ParameterForMinDurationInFrom);
                _refs.TransitionMinTimeParameterSelectionDropdown.SetValueWithoutNotify(index);
            }

            _refs.TransitionMinTimeIn1stStateInputField.gameObject.SetActive(!useParameterForMinDuration);
            _refs.TransitionMinTimeParameterSelectionDropdown.gameObject.SetActive(useParameterForMinDuration);

            _refs.TransitionMinTimeIn1stStateInputField.SetTextWithoutNotify("" + _selectedTransition.Transition.MinDurationInFrom);
        }




        public void OnConditionParameterDropdownChange(FSMEditorOneCondition conditionEditor, int changeTo)
        {
            if (_selectedTransition == null)
                return;
            FSMParameter parameter = _stateMachineDefinition.Parameters[changeTo];
            int conditionIndex = FSMEditorOneCondition.GetConditionIndex(_selectedTransition.Transition, conditionEditor.Condition);
            FSMEditorSaveDataChanger.SetConditionParameter(_selectedTransition.Transition, conditionIndex, parameter);
        }

        public void OnConditionBoolRequirementDropdownChange(FSMEditorOneCondition conditionEditor, bool changeTo)
        {
            if (_selectedTransition == null)
                return;
            int conditionIndex = FSMEditorOneCondition.GetConditionIndex(_selectedTransition.Transition, conditionEditor.Condition);
            FSMEditorSaveDataChanger.SetConditionBoolRequirement(_selectedTransition.Transition, conditionIndex, changeTo);
        }

        public void OnConditionFloatComparisonTypeDropdownChange(FSMEditorOneCondition conditionEditor, int changeTo)
        {
            if (_selectedTransition == null)
                return;
            int conditionIndex = FSMEditorOneCondition.GetConditionIndex(_selectedTransition.Transition, conditionEditor.Condition);
            FSMEditorSaveDataChanger.SetConditionFloatComparisonType(_selectedTransition.Transition, conditionIndex, changeTo);
        }

        public void OnConditionInputFieldFloatValueToCompareToChange(FSMEditorOneCondition conditionEditor, float changeTo)
        {
            if (_selectedTransition == null)
                return;
            int conditionIndex = FSMEditorOneCondition.GetConditionIndex(_selectedTransition.Transition, conditionEditor.Condition);
            FSMEditorSaveDataChanger.SetConditionFloatValueToCompareTo(_selectedTransition.Transition, conditionIndex, changeTo);
        }

        public void OnConditionFloatParameterToCompareToDropdownChange(FSMEditorOneCondition conditionEditor, int changeTo)
        {
            if (_selectedTransition == null)
                return;
            int parameterIndex = FSMEditorOneCondition.ConvertIndexOfParameterAmongstFloatsToAmongstAllParameters(
                _stateMachineDefinition, changeTo);
            FSMParameter parameter = _stateMachineDefinition.Parameters[parameterIndex];
            int conditionIndex = FSMEditorOneCondition.GetConditionIndex(_selectedTransition.Transition, conditionEditor.Condition);
            FSMEditorSaveDataChanger.SetConditionOtherFloatParameterToCompareTo(_selectedTransition.Transition, conditionIndex, parameter);
        }

        public void OnConditionCompareToValueOrParameterModeDropdownChange(FSMEditorOneCondition conditionEditor, int changeTo
            , TMP_Dropdown dropdown)
        {
            if (_selectedTransition == null)
                return;

            if (!FSMEditorOneCondition.HasAnyFloatParameter(_stateMachineDefinition))
            {
                Debug.LogWarning("Cannot switch to parameter mode because the state machine has no float parameter.");
                dropdown.SetValueWithoutNotify(0);
                return;
            }

            int conditionIndex = FSMEditorOneCondition.GetConditionIndex(_selectedTransition.Transition, conditionEditor.Condition);

            if (changeTo == 0)
            {
                // value
                FSMEditorSaveDataChanger.SetConditionOtherFloatParameterToCompareTo(_selectedTransition.Transition
                    , conditionIndex, null);
                // don't need to set the float value, just use whatever it already is.
            }
            else if (changeTo == 1)
            {
                // parameter
                int parameterIndex
                    = FSMEditorOneCondition.ConvertIndexOfParameterAmongstFloatsToAmongstAllParameters(_stateMachineDefinition, 0);
                FSMParameter parameter = _stateMachineDefinition.Parameters[parameterIndex];
                FSMEditorSaveDataChanger.SetConditionOtherFloatParameterToCompareTo(_selectedTransition.Transition
                   , conditionIndex, parameter);
            }

            if (_selectedTransition != null)
                UpdateShownMinDuration();
        }

        public void OnDropdownMinDurationModeChange(int changeTo)
        {
            if (_selectedTransition == null)
                return;

            if (!FSMEditorOneCondition.HasAnyFloatParameter(_stateMachineDefinition))
            {
                Debug.LogWarning("Cannot switch to parameter mode because the state machine has no float parameter.");
                _refs.TransitionMinTimeParameterOrValueDropdown.SetValueWithoutNotify(0);
                return;
            }

            if (changeTo == 0)
            {
                // value
                FSMEditorSaveDataChanger.SetFSMTransitionMinDurationParameter(_selectedTransition.Transition, null);
                // don't need to set the float value, just use whatever it already is.
            }
            else if (changeTo == 1)
            {
                // parameter
                int parameterIndex 
                    = FSMEditorOneCondition.ConvertIndexOfParameterAmongstFloatsToAmongstAllParameters(_stateMachineDefinition, 0);
                FSMParameter parameter = _stateMachineDefinition.Parameters[parameterIndex];
                FSMEditorSaveDataChanger.SetFSMTransitionMinDurationParameter(_selectedTransition.Transition, parameter);
            }
            UpdateShownMinDuration();
        }

        public void OnDropdownMinDurationParameterChange(int changeTo)
        {
            if (_selectedTransition == null)
                return;

            int parameterIndex 
                = FSMEditorOneCondition.ConvertIndexOfParameterAmongstFloatsToAmongstAllParameters(_stateMachineDefinition, changeTo);
            FSMParameter parameter = _stateMachineDefinition.Parameters[parameterIndex];
            FSMEditorSaveDataChanger.SetFSMTransitionMinDurationParameter(_selectedTransition.Transition, parameter);
        }

        public void OnInputFieldMinDurationTextChange(string changeTo)
        {
            if (_selectedTransition == null)
                return;
            if (!float.TryParse(changeTo, out float asFloat))
                return;
            FSMEditorSaveDataChanger.SetFSMTransitionMinDurationInFrom(_selectedTransition.Transition, asFloat);
        }

        public void OnToggleSelectedTransitionDisable(bool toggleTo)
        {
            if (_selectedTransition == null)
                return;
            FSMEditorSaveDataChanger.SetFSMTransitionDisable(_selectedTransition.Transition, toggleTo);
        }

        public void OnToggleSelectedTransitionLogFailureReason(bool toggleTo)
        {
            if (_selectedTransition == null)
                return;
            FSMEditorSaveDataChanger.SetFSMTransitionLogFailureReason(_selectedTransition.Transition, toggleTo);
        }

        public void OnToggleSelectedConditionEditor(FSMEditorOneCondition conditionEditor, bool toggleTo)
        {
            if (!toggleTo && conditionEditor != _selectedCondition)
                return;
            if (toggleTo)
                _selectedCondition = conditionEditor;
            else
                _selectedCondition = null;
        }

        public void OnToggleSelectedParameterEditor(FSMEditorOneParameter parameterEditor, bool toggleTo)
        {
            if (!toggleTo && parameterEditor != _selectedParameter)
                return;
            if (toggleTo)
                _selectedParameter = parameterEditor;
            else
                _selectedParameter = null;
        }

        public void OnAddConditionButton()
        {
            if (_selectedTransition == null)
                return;
            if (_stateMachineDefinition.Parameters.Length == 0)
            {
                Debug.LogWarning("The state machine has no parameters, so cannot add a condition.");
                return;
            }
            FSMEditorSaveDataChanger.AddDefaultCondition(_selectedTransition.Transition, _stateMachineDefinition);
            UpdateConditionsInScrollArea();
        }

        public void OnRemoveConditionButton()
        {
            if (_selectedTransition == null)
                return;
            FSMTransitionCondition[] conditions = _selectedTransition.Transition.Conditions;
            if (conditions.Length == 0)
                return;
            int indexOfConditionToRemove = conditions.Length - 1;
            if (_selectedCondition != null)
            {
                for (int i = 0; i < conditions.Length; i++)
                {
                    if (conditions[i] == _selectedCondition.Condition)
                    {
                        indexOfConditionToRemove = i;
                        break;
                    }
                }
            }
            FSMEditorSaveDataChanger.RemoveConditionAtIndex(_selectedTransition.Transition, indexOfConditionToRemove);
            UpdateConditionsInScrollArea();
        }

        public void OnButtonForSetSelectedStateAsDefault()
        {
            if (_selectedState != null && !_selectedState.IsForAnystate)
                SetDefaultState(_selectedState.State);
        }

        private void SetDefaultState(FSMState newDefaultState)
        {
            FSMEditorSaveDataChanger.SetFSMDefinitionDefaultState(_stateMachineDefinition, newDefaultState);
            foreach (FSMEditorOneState state in _states)
                state.UpdateWhetherDefaultState();
        }


        public void OnStateInputFieldTextChange(string renameTo)
        {
            if (_selectedState == null)
                return;

            renameTo = renameTo.Trim();
            
            if (renameTo.Length == 0 || renameTo == _selectedState.Name)
            {
                _refs.StateTitleInputField.SetTextWithoutNotify(_selectedState.Name);
                return;
            }


            string statePath = AssetDatabase.GetAssetPath(_selectedState.State);

            string pathOfStateFolder = PathOfFolder(_selectedState.State);
            renameTo = AdjustTitleToNotAlreadyExist(renameTo, pathOfStateFolder, out _);

            string errorMessage = AssetDatabase.RenameAsset(statePath, renameTo);
            if (errorMessage.Length != 0)
                Debug.LogWarning(errorMessage);
            else
                _selectedState.UpdateText();

            _refs.StateTitleInputField.SetTextWithoutNotify(_selectedState.Name);


            string enterGameEventPath = AssetDatabase.GetAssetPath(_selectedState.State.OnEnter);
            string updateGameEventPath = AssetDatabase.GetAssetPath(_selectedState.State.OnUpdate);
            string exitGameEventPath = AssetDatabase.GetAssetPath(_selectedState.State.OnExit);

            DecideTitlesOfStateGameEvents(_selectedState.Name, out string enterTitle, out string updateTitle, out string exitTitle);

            if (enterGameEventPath.Length != 0)
            {
                errorMessage = AssetDatabase.RenameAsset(enterGameEventPath, enterTitle);
                if (errorMessage.Length != 0)
                    Debug.LogWarning(errorMessage);
            }

            if (updateGameEventPath.Length != 0)
            {
                errorMessage = AssetDatabase.RenameAsset(updateGameEventPath, updateTitle);
                if (errorMessage.Length != 0)
                    Debug.LogWarning(errorMessage);
            }

            if (exitGameEventPath.Length != 0)
            {
                errorMessage = AssetDatabase.RenameAsset(exitGameEventPath, exitTitle);
                if (errorMessage.Length != 0)
                    Debug.LogWarning(errorMessage);
            }
        }

        public void OnParameterTitleChanged()
        {
            if (_selectedTransition == null)
                return;
            foreach (FSMEditorOneCondition condition in _conditionsOfSelectedTransition)
                condition.UpdateParameterDropdownsAndCondition();
            FSMEditorOneCondition.SetParametersInDropdownOnlyIncludingFloats(_stateMachineDefinition
                , _refs.TransitionMinTimeParameterSelectionDropdown);
        }








        public void OnCreateParameterButton()
        {
            string parameterTitle = "New Parameter";
            string parameterPath = MakePathForScriptableObject(ref parameterTitle, "Parameters");
            FSMParameter newParameter = ScriptableObject.CreateInstance<FSMParameter>();
            AssetDatabase.CreateAsset(newParameter, parameterPath);
            FSMEditorSaveDataChanger.AddParameterToFSMDefinition(newParameter, _stateMachineDefinition);

            AddVisualForParameter(newParameter);
            
            if (_selectedTransition != null)
            {
                UpdateShownMinDuration();
                for (int i = 0; i < _conditionsOfSelectedTransition.Count; i++)
                    _conditionsOfSelectedTransition[i].UpdateParameterDropdownsAndCondition();
            }
            UpdateParametersScrollContentHeight();
        }

        public void OnRemoveParameterButton()
        {
            if (_parameters.Count == 0)
                return;
            FSMEditorOneParameter editorToRemove = _parameters[^1];
            if (_selectedParameter != null)
                editorToRemove = _selectedParameter;

            FSMParameter parameterToRemove = editorToRemove.Parameter;

            for (int i = 0; i < _transitions.Count; i++)
            {
                if (_transitions[i].Transition.ParameterForMinDurationInFrom == parameterToRemove)
                {
                    Debug.LogWarning($"Cannot remove the parameter {parameterToRemove.name} because it's used by a transition's " +
                        $"Min Duration in Prior State.");
                    return;
                }

                FSMTransitionCondition[] conditions = _transitions[i].Transition.Conditions;
                for (int j = 0; j < conditions.Length; j++)
                {
                    FSMTransitionCondition condition = conditions[j];
                    bool parameterIsComparedToIt = condition.Parameter.Type == FSMParameter.ParameterType.Float
                        && condition.OtherFloatParameterToCompareTo == parameterToRemove;

                    if (condition.Parameter == parameterToRemove || parameterIsComparedToIt)
                    {
                        Debug.LogWarning($"Cannot remove the parameter {parameterToRemove.name} because it's used by a transition.");
                        return;
                    }
                }
            }

            int parameterIndex = FSMEditorOneCondition.GetParameterIndex(_stateMachineDefinition, parameterToRemove);
            FSMEditorSaveDataChanger.RemoveParameterFromFSMDefinition(_stateMachineDefinition, parameterIndex);

            _parameters.Remove(editorToRemove);
            Destroy(editorToRemove.gameObject);

            if (_selectedTransition != null)
            {
                UpdateShownMinDuration();
                for (int i = 0; i < _conditionsOfSelectedTransition.Count; i++)
                    _conditionsOfSelectedTransition[i].UpdateParameterDropdownsAndCondition();
            }
            UpdateParametersScrollContentHeight();

            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(parameterToRemove));
        }

        private void AddState(out FSMState newState)
        {
            string stateTitle = "New State";
            string statePath = MakePathForScriptableObject(ref stateTitle, "States");

            newState = ScriptableObject.CreateInstance<FSMState>();
            AssetDatabase.CreateAsset(newState, statePath);
            FSMEditorSaveDataChanger.AddStateToFSMDefinition(_stateMachineDefinition, newState);

            if (_stateMachineDefinition.EditorInfo.States.Length == 1)
            {
                // had no states, now have one so make it the default
                SetDefaultState(_stateMachineDefinition.EditorInfo.States[0]);
            }

            DecideTitlesOfStateGameEvents(stateTitle, out string enterTitle, out string updateTitle, out string exitTitle);
            string enterPath = MakePathForScriptableObject(ref enterTitle, "GameEvents");
            string updatePath = MakePathForScriptableObject(ref updateTitle, "GameEvents");
            string exitPath = MakePathForScriptableObject(ref exitTitle, "GameEvents");

            GameEventScriptableObject enterEvent = ScriptableObject.CreateInstance<GameEventScriptableObject>();
            GameEventScriptableObject updateEvent = ScriptableObject.CreateInstance<GameEventScriptableObject>();
            GameEventScriptableObject exitEvent = ScriptableObject.CreateInstance<GameEventScriptableObject>();

            AssetDatabase.CreateAsset(enterEvent, enterPath);
            AssetDatabase.CreateAsset(updateEvent, updatePath);
            AssetDatabase.CreateAsset(exitEvent, exitPath);
            FSMEditorSaveDataChanger.SetGameEventsOfState(newState, enterEvent, updateEvent, exitEvent);
        }

        public void OnCreateStateButton()
        {
            AddState(out FSMState newState);
            AddVisualForState(newState);

            if (_creatingTransitionFrom == null)
            {
                _selectedState = _states[^1];
                ShowVisualsForSelectedState();
                _refs.SelectedStateEditorToggle.SetActive(true);
            }
        }

        private void DecideTitlesOfStateGameEvents(string stateTitle
            , out string enterTitle, out string updateTitle, out string exitTitle)
        {
            enterTitle = stateTitle + " {Enter}";
            updateTitle = stateTitle + " {Update}";
            exitTitle = stateTitle + " {Exit}";
        }

        public static string AdjustTitleToNotAlreadyExist(string title, string pathOfFolder, out int number)
        {
            number = 0;
            string path;
            string newTitle;
            do
            {
                newTitle = number == 0 ? title : $"{title} {number}";
                string fileName = $"{newTitle}.asset";
                path = System.IO.Path.Combine(pathOfFolder, fileName);
                number++;
            } while (AssetDatabase.AssetPathToGUID(path, AssetPathToGUIDOptions.OnlyExistingAssets).Length != 0);
            number--;
            return newTitle;
        }

        public static string PathOfFolder(ScriptableObject scriptableObject)
        {
            string pathOfFSMDefinition = AssetDatabase.GetAssetPath(scriptableObject);
            string pathOfParentFolder = System.IO.Path.GetDirectoryName(pathOfFSMDefinition);
            return pathOfParentFolder;
        }

        private string PathOfParentFolder()
        {
            return PathOfFolder(_stateMachineDefinition);
        }

        private string MakePathForScriptableObject(ref string title, string subfolder = null)
        {
            string pathOfParentFolder = PathOfParentFolder();
            string pathOfFolder = pathOfParentFolder;
            if (subfolder != null && subfolder.Length != 0)
            {
                pathOfFolder = System.IO.Path.Combine(pathOfFolder, subfolder);
                if (!AssetDatabase.IsValidFolder(pathOfFolder))
                    AssetDatabase.CreateFolder(pathOfParentFolder, subfolder);
            }
            if (!AssetDatabase.IsValidFolder(pathOfFolder))
                throw new System.InvalidOperationException("The folder isn't valid somehow.");

            title = AdjustTitleToNotAlreadyExist(title, pathOfFolder, out _);
            return System.IO.Path.Combine(pathOfFolder, $"{title}.asset");
        }







        private void CheckDragOneState(bool clickingZoomableArea)
        {
            if (clickingZoomableArea)
            {
                _dragging = FindStateBelowMouse();
            }

            if (Input.GetButtonUp("left click"))
            {
                if (_dragging != null)
                {
                    FSMEditorSaveDataChanger.SavePositionOfState(_stateMachineDefinition, _dragging);
                    _dragging = null;
                }
            }

            Vector2 newMouseWorldPos = _cameraInfo.MouseWorldPosition;
            if (_dragging != null)
            {
                if (!_cameraControls.ApplicationGainedFocusPriorFrame)
                {
                    // condition is b/c trying to fix a bug where dragging sometimes sends the rectangle far away from the mouse,
                    // not sure if this is correct
                    _dragging.Drag(newMouseWorldPos - _priorMouseWorldPos);
                }
            }
            _priorMouseWorldPos = newMouseWorldPos;
        }

        private FSMEditorOneState FindStateBelowMouse()
        {
            Vector2 mouseWorldPos = _cameraInfo.MouseWorldPosition;
            _clickCandidates.Clear();
            foreach (FSMEditorOneState state in _states)
            {
                if (state.WorldPointIsInsideRect(mouseWorldPos))
                    _clickCandidates.Add(state);
            }

            // Click the state with the highest sibling index, because that's rendered on top of the others
            FSMEditorOneState result = null;
            int highestSiblingIndex = -1;
            for (int i = 0; i < _clickCandidates.Count; i++)
            {
                int siblingIndex = _clickCandidates[i].transform.GetSiblingIndex();
                if (siblingIndex > highestSiblingIndex)
                {
                    highestSiblingIndex = siblingIndex;
                    result = _clickCandidates[i];
                }
            }
            return result;
        }

        private FSMEditorOneTransition FindClickedTransition()
        {
            Vector2 mouseWorldPos = _cameraInfo.MouseWorldPosition;
            float minSqrDistance = float.PositiveInfinity;
            FSMEditorOneTransition closest = null;
            foreach (FSMEditorOneTransition transition in _transitions)
            {
                float sqrDistance = transition.SqrDistanceFromLineSegment(mouseWorldPos);
                if (sqrDistance < minSqrDistance)
                {
                    minSqrDistance = sqrDistance;
                    closest = transition;
                }
            }
            float minDistance = Mathf.Sqrt(minSqrDistance);

            // this is really just the distance scaled by the camera orthographic size (zoomout)
            // but also scaled by a constant which makes it like pixels with 1920x1080 screen size,
            // so on a 4k screen it hopefully won't have reduced click radius.
            float pixelsPerUnitIsh = 1080 / (_cameraInfo.ZoomOut * 2);
            float minDistanceInPixelsIsh = minDistance * pixelsPerUnitIsh;

            const float clickRadiusInPixels = 30;

            if (minDistanceInPixelsIsh < clickRadiusInPixels)
            {
                return closest;
            }
            return null;
        }
    }
}
#endif
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;
using UnityEditor;


namespace FiniteStateMachineEditor
{
    public static class FSMEditorSaveDataChanger
    {
        private static SerializedObject _serializedObject;
        private static SerializedProperty _serializedProperty;

        private static SerializedProperty Property(Object unityObject, string property)
        {
            _serializedObject = new SerializedObject(unityObject);
            _serializedProperty = _serializedObject.FindProperty(property);
            return _serializedProperty;
        }

        private static SerializedProperty SwitchProperty(string property)
        {
            _serializedProperty = _serializedObject.FindProperty(property);
            return _serializedProperty;
        }

        private static SerializedProperty SubProperty(string subProperty)
        {
            _serializedProperty = _serializedProperty.FindPropertyRelative(subProperty);
            return _serializedProperty;
        }

        private static SerializedProperty ArrayElement(int index)
        {
            _serializedProperty = _serializedProperty.GetArrayElementAtIndex(index);
            return _serializedProperty;
        }

        private static SerializedProperty AppendAndGotoNewArrayElement(int priorLength)
        {
            _serializedProperty.InsertArrayElementAtIndex(priorLength);
            return ArrayElement(priorLength);
        }

        private static SerializedProperty GoToPropertyOfCondition(FSMTransition transition
            , int conditionIndex, string property)
        {
            Property(transition, "_conditions");
            ArrayElement(conditionIndex);
            return SubProperty(property);
        }

        private static void ApplyModifiedProperties()
        {
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
            _serializedObject = null; // to make it less bug-prone
            _serializedProperty = null;
        }



        public static void RemoveParameterFromFSMDefinition(FSMDefinition fsmDefinition, int parameterIndex)
        {
            Property(fsmDefinition, "<Parameters>k__BackingField").DeleteArrayElementAtIndex(parameterIndex);
            ApplyModifiedProperties();
        }


        public static void AddParameterToFSMDefinition(FSMParameter parameter, FSMDefinition fsmDefinition)
        {
            Property(fsmDefinition, "<Parameters>k__BackingField");
            AppendAndGotoNewArrayElement(fsmDefinition.Parameters.Length).objectReferenceValue = parameter;
            ApplyModifiedProperties();
        }

       




        public static void SetParameterInitialTrigger(FSMParameter parameter, bool initial)
        {
            Property(parameter, "<InitialTrigger>k__BackingField").boolValue = initial;
            ApplyModifiedProperties();
        }

        public static void SetParameterInitialBool(FSMParameter parameter, bool initial)
        {
            Property(parameter, "<InitialBool>k__BackingField").boolValue = initial;
            ApplyModifiedProperties();
        }

        public static void SetParameterInitialFloat(FSMParameter parameter, float initial)
        {
            Property(parameter, "<InitialFloat>k__BackingField").floatValue = initial;
            ApplyModifiedProperties();
        }

        public static void SetParameterType(FSMParameter parameter, int typeIndex)
        {
            Property(parameter, "<Type>k__BackingField").intValue = typeIndex;
            ApplyModifiedProperties();
        }

        public static void SetConditionParameter(FSMTransition transition, int conditionIndex, FSMParameter parameter)
        {
            GoToPropertyOfCondition(transition, conditionIndex, "_parameter")
                .objectReferenceValue = parameter;
            ApplyModifiedProperties();
        }

        public static void SetConditionBoolRequirement(FSMTransition transition, int conditionIndex, bool requiredBool)
        {
            GoToPropertyOfCondition(transition, conditionIndex, "_equalsForBoolParameter")
                .boolValue = requiredBool;
            ApplyModifiedProperties();
        }

        public static void SetConditionFloatComparisonType(FSMTransition transition, int conditionIndex, int comparisonTypeAsInt)
        {
            GoToPropertyOfCondition(transition, conditionIndex, "_comparisonTypeForFloatParameter")
               .intValue = comparisonTypeAsInt;
            ApplyModifiedProperties();
        }

        

        public static void SetConditionOtherFloatParameterToCompareTo(FSMTransition transition, int conditionIndex, FSMParameter compareTo)
        {
            GoToPropertyOfCondition(transition, conditionIndex, "_otherFloatParameterToCompareTo")
              .objectReferenceValue = compareTo;
            ApplyModifiedProperties();
        }

        public static void SetConditionFloatValueToCompareTo(FSMTransition transition, int conditionIndex, float compareTo)
        {
            GoToPropertyOfCondition(transition, conditionIndex, "_comparedToForFloatParameter")
              .floatValue = compareTo;
            ApplyModifiedProperties();
        }



        public static void AddDefaultCondition(FSMTransition transition, FSMDefinition fsmDefinition)
        {
            Property(transition, "_conditions");
            AppendAndGotoNewArrayElement(transition.Conditions.Length);
            _serializedProperty.FindPropertyRelative("_parameter").objectReferenceValue = fsmDefinition.Parameters[0];
            _serializedProperty.FindPropertyRelative("_equalsForBoolParameter").boolValue = false;
            _serializedProperty.FindPropertyRelative("_comparisonTypeForFloatParameter").intValue = 0;
            _serializedProperty.FindPropertyRelative("_otherFloatParameterToCompareTo").objectReferenceValue = null;
            _serializedProperty.FindPropertyRelative("_comparedToForFloatParameter").floatValue = 0;
            ApplyModifiedProperties();
        }

        

        public static void RemoveTransitionAtIndex(FSMDefinition fsmDefinition, int index)
        {
            Property(fsmDefinition, "<Transitions>k__BackingField").DeleteArrayElementAtIndex(index);
            ApplyModifiedProperties();
        }

        public static void RemoveConditionAtIndex(FSMTransition transition, int index)
        {
            Property(transition, "_conditions").DeleteArrayElementAtIndex(index);
            ApplyModifiedProperties();
        }

        public static void SetFSMTransitionLogFailureReason(FSMTransition transition, bool logFailureReason)
        {
            Property(transition, "<LogFailureReason>k__BackingField").boolValue = logFailureReason;
            ApplyModifiedProperties();
        }

        public static void SetFSMTransitionDisable(FSMTransition transition, bool disable)
        {
            Property(transition, "_disable").boolValue = disable;
            ApplyModifiedProperties();
        }

        public static void SetFSMTransitionMinDurationInFrom(FSMTransition transition, float minDurationInFrom)
        {
            Property(transition, "_minDurationInFrom").floatValue = minDurationInFrom;
            ApplyModifiedProperties();
        }

        public static void SetFSMTransitionMinDurationParameter(FSMTransition transition, FSMParameter parameter)
        {
            Property(transition, "_parameterForMinDurationInFrom").objectReferenceValue = parameter;
            ApplyModifiedProperties();
        }

        public static void SetFSMDefinitionDefaultState(FSMDefinition fsmDefinition, FSMState defaultState)
        {
            Property(fsmDefinition, "<DefaultState>k__BackingField").objectReferenceValue = defaultState;
            ApplyModifiedProperties();
        }

        public static void AddTransitionToFSMDefinition(FSMDefinition fsmDefinition, FSMTransition transition)
        {
            Property(fsmDefinition, "<Transitions>k__BackingField");
            AppendAndGotoNewArrayElement(fsmDefinition.Transitions.Length).objectReferenceValue = transition;
            ApplyModifiedProperties();
        }

        public static void SetFSMTransitionFromAndTo(FSMTransition transition, FSMState from, FSMState to)
        {
            Property(transition, "_from").objectReferenceValue = from;
            SwitchProperty("_to").objectReferenceValue = to;
            ApplyModifiedProperties();
        }

        public static void AddStateToFSMDefinition(FSMDefinition fsmDefinition, FSMState state)
        {
            bool noStatesYet = fsmDefinition.EditorInfo.States == null;
            int statesLength = noStatesYet ? 0 : fsmDefinition.EditorInfo.States.Length;

            bool noPositionsYet = fsmDefinition.EditorInfo.StateEditorPositions == null;
            int positionsLength = noPositionsYet ? 0 : fsmDefinition.EditorInfo.StateEditorPositions.Length;

            if (statesLength != positionsLength)
            {
                throw new System.InvalidOperationException("Unequal lengths of arrays in the fsm definition's editor info "
                    + statesLength + " " + positionsLength + ", fsmDefinition: " + fsmDefinition.name);
            }

            SerializedProperty editorInfoProperty = Property(fsmDefinition, "<EditorInfo>k__BackingField");
            SubProperty("<States>k__BackingField");
            AppendAndGotoNewArrayElement(statesLength).objectReferenceValue = state;

            _serializedProperty = editorInfoProperty;
            SubProperty("<StateEditorPositions>k__BackingField");
            AppendAndGotoNewArrayElement(statesLength).vector2Value = Camera.main.transform.position;

            ApplyModifiedProperties();
        }

        public static void RemoveStateAtIndex(FSMDefinition fsmDefinition, int index)
        {
            if (fsmDefinition.EditorInfo.States.Length != fsmDefinition.EditorInfo.StateEditorPositions.Length)
                throw new System.InvalidOperationException("Unequal lengths of arrays in the fsm definition's editor info.");

            SerializedProperty editorInfoProperty = Property(fsmDefinition, "<EditorInfo>k__BackingField");
            SubProperty("<States>k__BackingField").DeleteArrayElementAtIndex(index);

            _serializedProperty = editorInfoProperty;
            SubProperty("<StateEditorPositions>k__BackingField").DeleteArrayElementAtIndex(index);

            ApplyModifiedProperties();
        }





        public static void SetGameEventsOfState(FSMState state
            , GameEventScriptableObject onEnter, GameEventScriptableObject onUpdate, GameEventScriptableObject onExit)
        {
            Property(state, "<OnEnter>k__BackingField").objectReferenceValue = onEnter;
            SwitchProperty("<OnUpdate>k__BackingField").objectReferenceValue = onUpdate;
            SwitchProperty("<OnExit>k__BackingField").objectReferenceValue = onExit;
            ApplyModifiedProperties();
        }


        public static void SavePositionOfState(FSMDefinition fsmDefinition, FSMEditorOneState stateEditor)
        {
            Property(fsmDefinition, "<EditorInfo>k__BackingField");

            if (!stateEditor.IsForAnystate)
            {
                SubProperty("<StateEditorPositions>k__BackingField");
                ArrayElement(FindStateIndexInFSMDefinition(fsmDefinition, stateEditor.State));
            }
            else
            {
                SubProperty("<StateEditorForAnystatePosition>k__BackingField");
            }

            _serializedProperty.vector2Value = stateEditor.transform.position;
            ApplyModifiedProperties();
        }



        // split this out
        public static Vector2 GetPositionOfState(FSMDefinition fsmDefinition, FSMState state)
        {
            if (state == null)
            {
                return fsmDefinition.EditorInfo.StateEditorForAnystatePosition;
            }
            int index = FindStateIndexInFSMDefinition(fsmDefinition, state);
            return fsmDefinition.EditorInfo.StateEditorPositions[index];
        }

        // split this out
        private static int FindStateIndexInFSMDefinition(FSMDefinition fsmDefinition, FSMState state)
        {
            int i = 0;
            for (; i < fsmDefinition.EditorInfo.States.Length; i++)
            {
                if (fsmDefinition.EditorInfo.States[i] == state)
                    break;
            }
            if (i >= fsmDefinition.EditorInfo.StateEditorPositions.Length)
                throw new System.Exception("Error while initializing editor for one state");
            return i;
        }

        

    }
}
#endif
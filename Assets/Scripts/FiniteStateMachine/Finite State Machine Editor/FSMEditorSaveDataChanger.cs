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
        public static void RemoveParameterFromFSMDefinition(FSMDefinition fsmDefinition, int parameterIndex)
        {
            SerializedObject so = new SerializedObject(fsmDefinition);
            SerializedProperty arrayProperty = so.FindProperty("<Parameters>k__BackingField");
            arrayProperty.DeleteArrayElementAtIndex(parameterIndex);
            so.ApplyModifiedPropertiesWithoutUndo();
        }


        public static void AddParameterToFSMDefinition(FSMParameter parameter, FSMDefinition fsmDefinition)
        {
            int newIndex = fsmDefinition.Parameters.Length;
            SerializedObject so = new SerializedObject(fsmDefinition);
            SerializedProperty arrayProperty = so.FindProperty("<Parameters>k__BackingField");
            arrayProperty.InsertArrayElementAtIndex(newIndex);
            arrayProperty.GetArrayElementAtIndex(newIndex).objectReferenceValue = parameter;
            so.ApplyModifiedPropertiesWithoutUndo();
        }




        public static void SetParameterInitialTrigger(FSMParameter parameter, bool initial)
        {
            SerializedObject so = new SerializedObject(parameter);
            so.FindProperty("<InitialTrigger>k__BackingField").boolValue = initial;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetParameterInitialBool(FSMParameter parameter, bool initial)
        {
            SerializedObject so = new SerializedObject(parameter);
            so.FindProperty("<InitialBool>k__BackingField").boolValue = initial;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetParameterInitialFloat(FSMParameter parameter, float initial)
        {
            SerializedObject so = new SerializedObject(parameter);
            so.FindProperty("<InitialFloat>k__BackingField").floatValue = initial;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetParameterType(FSMParameter parameter, int typeIndex)
        {
            SerializedObject so = new SerializedObject(parameter);
            so.FindProperty("<Type>k__BackingField").intValue = typeIndex;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetConditionParameter(FSMTransition transition, int conditionIndex, FSMParameter parameter)
        {
            SerializedObject so = new SerializedObject(transition);
            SerializedProperty conditionsArrayProperty = so.FindProperty("_conditions");
            SerializedProperty conditionProperty = conditionsArrayProperty.GetArrayElementAtIndex(conditionIndex);
            conditionProperty.FindPropertyRelative("_parameter").objectReferenceValue = parameter;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetConditionBoolRequirement(FSMTransition transition, int conditionIndex, bool requiredBool)
        {
            SerializedObject so = new SerializedObject(transition);
            SerializedProperty conditionsArrayProperty = so.FindProperty("_conditions");
            SerializedProperty conditionProperty = conditionsArrayProperty.GetArrayElementAtIndex(conditionIndex);
            conditionProperty.FindPropertyRelative("_equalsForBoolParameter").boolValue = requiredBool;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetConditionFloatComparisonType(FSMTransition transition, int conditionIndex
            , int comparisonTypeAsInt)
        {
            SerializedObject so = new SerializedObject(transition);
            SerializedProperty conditionsArrayProperty = so.FindProperty("_conditions");
            SerializedProperty conditionProperty = conditionsArrayProperty.GetArrayElementAtIndex(conditionIndex);
            conditionProperty.FindPropertyRelative("_comparisonTypeForFloatParameter").intValue = comparisonTypeAsInt;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetConditionOtherFloatParameterToCompareTo(FSMTransition transition, int conditionIndex, FSMParameter compareTo)
        {
            SerializedObject so = new SerializedObject(transition);
            SerializedProperty conditionsArrayProperty = so.FindProperty("_conditions");
            SerializedProperty conditionProperty = conditionsArrayProperty.GetArrayElementAtIndex(conditionIndex);
            conditionProperty.FindPropertyRelative("_otherFloatParameterToCompareTo").objectReferenceValue = compareTo;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetConditionFloatValueToCompareTo(FSMTransition transition, int conditionIndex, float compareTo)
        {
            SerializedObject so = new SerializedObject(transition);
            SerializedProperty conditionsArrayProperty = so.FindProperty("_conditions");
            SerializedProperty conditionProperty = conditionsArrayProperty.GetArrayElementAtIndex(conditionIndex);
            conditionProperty.FindPropertyRelative("_comparedToForFloatParameter").floatValue = compareTo;
            so.ApplyModifiedPropertiesWithoutUndo();
        }



        public static void AddDefaultCondition(FSMTransition transition, FSMDefinition fsmDefinition)
        {
            int newIndex = transition.Conditions.Length;
            SerializedObject so = new SerializedObject(transition);
            SerializedProperty conditionsArrayProperty = so.FindProperty("_conditions");
            conditionsArrayProperty.InsertArrayElementAtIndex(newIndex);
            SerializedProperty conditionProperty = conditionsArrayProperty.GetArrayElementAtIndex(newIndex);
            conditionProperty.FindPropertyRelative("_parameter").objectReferenceValue = fsmDefinition.Parameters[0];
            conditionProperty.FindPropertyRelative("_equalsForBoolParameter").boolValue = false;
            conditionProperty.FindPropertyRelative("_comparisonTypeForFloatParameter").intValue = 0;
            conditionProperty.FindPropertyRelative("_otherFloatParameterToCompareTo").objectReferenceValue = null;
            conditionProperty.FindPropertyRelative("_comparedToForFloatParameter").floatValue = 0;
            so.ApplyModifiedPropertiesWithoutUndo();

        }

        

        public static void RemoveTransitionAtIndex(FSMDefinition fsmDefinition, int index)
        {
            SerializedObject so = new SerializedObject(fsmDefinition);
            SerializedProperty transitionsArrayProperty = so.FindProperty("<Transitions>k__BackingField");
            transitionsArrayProperty.DeleteArrayElementAtIndex(index);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void RemoveConditionAtIndex(FSMTransition transition, int index)
        {
            SerializedObject so = new SerializedObject(transition);
            SerializedProperty conditionsArrayProperty = so.FindProperty("_conditions");
            conditionsArrayProperty.DeleteArrayElementAtIndex(index);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetFSMTransitionLogFailureReason(FSMTransition transition, bool logFailureReason)
        {
            SerializedObject so = new SerializedObject(transition);
            so.FindProperty("<LogFailureReason>k__BackingField").boolValue = logFailureReason;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetFSMTransitionDisable(FSMTransition transition, bool disable)
        {
            SerializedObject so = new SerializedObject(transition);
            so.FindProperty("_disable").boolValue = disable;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetFSMTransitionMinDurationInFrom(FSMTransition transition, float minDurationInFrom)
        {
            SerializedObject so = new SerializedObject(transition);
            so.FindProperty("_minDurationInFrom").floatValue = minDurationInFrom;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetFSMTransitionMinDurationParameter(FSMTransition transition, FSMParameter parameter)
        {
            SerializedObject so = new SerializedObject(transition);
            so.FindProperty("_parameterForMinDurationInFrom").objectReferenceValue = parameter;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void SetFSMDefinitionDefaultState(FSMDefinition fsmDefinition, FSMState defaultState)
        {
            SerializedObject so = new SerializedObject(fsmDefinition);
            so.FindProperty("<DefaultState>k__BackingField").objectReferenceValue = defaultState;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        public static void AddTransitionToFSMDefinition(FSMDefinition fsmDefinition, FSMTransition transition)
        {
            SerializedObject so = new SerializedObject(fsmDefinition);
            int newIndex = fsmDefinition.Transitions.Length;
            SerializedProperty transitionsArrayProperty = so.FindProperty("<Transitions>k__BackingField");
            transitionsArrayProperty.InsertArrayElementAtIndex(newIndex);
            SerializedProperty transitionProperty = transitionsArrayProperty.GetArrayElementAtIndex(newIndex);
            transitionProperty.objectReferenceValue = transition;
            so.ApplyModifiedProperties();
        }

        public static void SetFSMTransitionFromAndTo(FSMTransition transition, FSMState from, FSMState to)
        {
            SerializedObject so = new SerializedObject(transition);
            so.FindProperty("_from").objectReferenceValue = from;
            so.FindProperty("_to").objectReferenceValue = to;
            so.ApplyModifiedProperties();
        }

        public static void AddStateToFSMDefinition(FSMDefinition fsmDefinition, FSMState state)
        {
            bool noStatesYet = fsmDefinition.EditorInfo.States == null;
            bool noPositionsYet = fsmDefinition.EditorInfo.StateEditorPositions == null;
            int statesLength = noStatesYet ? 0 : fsmDefinition.EditorInfo.States.Length;
            int positionsLength = noPositionsYet ? 0 : fsmDefinition.EditorInfo.StateEditorPositions.Length;
            if (statesLength != positionsLength)
                throw new System.InvalidOperationException("Unequal lengths of arrays in the fsm definition's editor info "
                    + statesLength + " " + positionsLength);

            SerializedObject so = new SerializedObject(fsmDefinition);

            int newIndex = noStatesYet ? 0 : fsmDefinition.EditorInfo.States.Length;

            SerializedProperty statesArrayProperty = so.FindProperty("<EditorInfo>k__BackingField")
               .FindPropertyRelative("<States>k__BackingField");
            statesArrayProperty.InsertArrayElementAtIndex(newIndex);
            SerializedProperty stateProperty = statesArrayProperty.GetArrayElementAtIndex(newIndex);
            stateProperty.objectReferenceValue = state;
            
            Vector2 position = Camera.main.transform.position;
            SerializedProperty positionsArrayProperty = so.FindProperty("<EditorInfo>k__BackingField")
                .FindPropertyRelative("<StateEditorPositions>k__BackingField");
            positionsArrayProperty.InsertArrayElementAtIndex(newIndex);
            SerializedProperty positionProperty = positionsArrayProperty.GetArrayElementAtIndex(newIndex);
            positionProperty.vector2Value = position;

            so.ApplyModifiedProperties();
        }

        public static void RemoveStateAtIndex(FSMDefinition fsmDefinition, int index)
        {
            if (fsmDefinition.EditorInfo.States.Length != fsmDefinition.EditorInfo.StateEditorPositions.Length)
                throw new System.InvalidOperationException("Unequal lengths of arrays in the fsm definition's editor info.");

            SerializedObject so = new SerializedObject(fsmDefinition);

            SerializedProperty statesArrayProperty = so.FindProperty("<EditorInfo>k__BackingField")
               .FindPropertyRelative("<States>k__BackingField");
            statesArrayProperty.DeleteArrayElementAtIndex(index);

            SerializedProperty positionsArrayProperty = so.FindProperty("<EditorInfo>k__BackingField")
                .FindPropertyRelative("<StateEditorPositions>k__BackingField");
            positionsArrayProperty.DeleteArrayElementAtIndex(index);

            so.ApplyModifiedPropertiesWithoutUndo();
        }





        public static void SetGameEventsOfState(FSMState state
            , GameEventScriptableObject onEnter, GameEventScriptableObject onUpdate, GameEventScriptableObject onExit)
        {
            SerializedObject so = new SerializedObject(state);
            SerializedProperty onEnterProperty = so.FindProperty("<OnEnter>k__BackingField");
            SerializedProperty onUpdateProperty = so.FindProperty("<OnUpdate>k__BackingField");
            SerializedProperty onExitProperty = so.FindProperty("<OnExit>k__BackingField");
            onEnterProperty.objectReferenceValue = onEnter;
            onUpdateProperty.objectReferenceValue = onUpdate;
            onExitProperty.objectReferenceValue = onExit;
            so.ApplyModifiedProperties();
        }



        public static Vector2 GetPositionOfState(FSMDefinition fsmDefinition, FSMState state)
        {
            if (state == null)
            {
                return fsmDefinition.EditorInfo.StateEditorForAnystatePosition;
            }
            int index = FindStateIndexInFSMDefinition(fsmDefinition, state);
            return fsmDefinition.EditorInfo.StateEditorPositions[index];
        }

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

        public static void SavePositionOfState(FSMDefinition fsmDefinition, FSMEditorOneState stateEditor)
        {
            // Essentially _fsmDefinition.EditorInfo.StateEditorPositions[index] = stateRect.transform.position;

            SerializedObject so = new SerializedObject(fsmDefinition);

            if (!stateEditor.IsForAnystate)
            {
                int index = FindStateIndexInFSMDefinition(fsmDefinition, stateEditor.State);
                so.FindProperty("<EditorInfo>k__BackingField")
                    .FindPropertyRelative("<StateEditorPositions>k__BackingField")
                    .GetArrayElementAtIndex(index)
                    .vector2Value = stateEditor.transform.position;
            }
            else
            {
                so.FindProperty("<EditorInfo>k__BackingField")
                   .FindPropertyRelative("<StateEditorForAnystatePosition>k__BackingField")
                   .vector2Value = stateEditor.transform.position;
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

    }
}
#endif
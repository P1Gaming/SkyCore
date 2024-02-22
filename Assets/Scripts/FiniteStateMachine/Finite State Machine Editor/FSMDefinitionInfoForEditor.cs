using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

// info saved with a state machine for the editor

namespace FiniteStateMachineEditor
{
    [System.Serializable]
    public class FSMDefinitionInfoForEditor
    {
        [field: SerializeField]
        public FSMState[] States { get; private set; }

        [field: SerializeField]
        public Vector2[] StateEditorPositions { get; private set; }
    }
}
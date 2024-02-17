using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
    // FiniteStateMachineInstance could directly reference its initial state and transitions, but this
    // makes it easier to set things up, because you set up everything in folders (scriptable objects)
    // and then just need to reference this from the scene.

    /// <summary>
    /// Holds references to other scriptable object instances to define a finite state machine,
    /// and has a list of parameters which transitions can use.
    /// </summary>
    [CreateAssetMenu(menuName = "Finite State Machine/State Machine Definition")]
    public class FSMDefinition : ScriptableObject
    {
        [field: SerializeField]
        public FSMState DefaultState { get; private set; }
        [field: SerializeField]
        public FSMParameter[] Parameters { get; private set; }
        [field: SerializeField]
        public FSMTransition[] Transitions { get; private set; }

        public void CheckValid(Dictionary<FSMParameter, float> floatParameters)
        {
            foreach (FSMTransition transition in Transitions)
            {
                transition.CheckValid(floatParameters);
            }
        }
    }
}
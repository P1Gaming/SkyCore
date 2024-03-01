using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Making this for communication between the drone's two state machines for actions and movements.
// E.g. to replicate the current behavior, the drone shouldn't move while scanning things.

namespace FiniteStateMachine
{
    /// <summary>
    /// Makes a state machine instance's bool parameter true while a state machine instance is in particular states, and otherwise false.
    /// </summary>
    [System.Serializable]
    public class BoolTrueWhileInCertainFSMStates
    {
        [SerializeField]
        private FSMParameter _bool;
        [SerializeField]
        private FSMState[] _trueInStates;

        private FiniteStateMachineInstance _fsmInstance;
        private GameEventResponses _responses = new();

        /// <summary>
        /// Provides the state machine instance whose bool parameter will be set, and the identifier for selective listeners.
        /// E.g. to set a bool parameter in a drone state machine instance when a particular jelly's state machine is in
        /// certain states, provide the drone fsm instance and the same monobehaviour passed into the jelly's fsm instance Update call.
        /// </summary>
        public void Initialize(FiniteStateMachineInstance fsmInstanceWithTheBool, MonoBehaviour identifierIfNeedSelectiveListening)
        {
            _fsmInstance = fsmInstanceWithTheBool;

            // When enter any of the states, set the bool true. When exit any of the states, set the bool false.

            (GameEventScriptableObject, System.Action)[] eventsAndResponses
                = new (GameEventScriptableObject, System.Action)[_trueInStates.Length * 2];

            int n = _trueInStates.Length;
            for (int i = 0; i < n; i++)
            {
                eventsAndResponses[i] = (_trueInStates[i].OnEnter, SetTrue);
                eventsAndResponses[i + n] = (_trueInStates[i].OnExit, SetFalse);
            }

            if (identifierIfNeedSelectiveListening == null)
            {
                _responses.SetResponses(eventsAndResponses);
            }
            else
            {
                _responses.SetSelectiveResponses(identifierIfNeedSelectiveListening, eventsAndResponses);
            }
        }

        public void Register()
        {
            _responses.Register();
        }

        public void Unregegister()
        {
            _responses.Unregister();
        }

        private void SetTrue() => _fsmInstance.SetBool(_bool, true);

        private void SetFalse() => _fsmInstance.SetBool(_bool, false);
    }
}

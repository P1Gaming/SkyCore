using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

-Based on https://github.com/DavidJones011/P1-AILearning/tree/main

-A finite state machine (fsm) has multiple states, and is in one state at a time.

-Each state can do things when entered, exited, and each frame.

-MAIN PURPOSE: There are transitions between states, which happen when conditions are met. Those 
 conditions are checked each frame. This is easier to work with than doing the logic with a bunch 
 of if statements.

-It's probably easiest to use scriptable objects to set things up. You can just drag and drop
 things to set up references, e.g. to decide which states a transition goes between. Could use prefabs
 for that, but that'd require instantiating prefabs which is messier, and wouldn't work with
 GameEventScriptableObject.

-Will use GameEventScriptableObject to define what happens when a state is entered/exited/each frame.

-Need a way to get data to the transition from e.g. the drone, to check if the transition should
 occur. To do that, will mimic Unity's animator. The animator has a list of parameters and their
 initial values, and you can call e.g. SetBool(name, value) to set the data.
*/

namespace FiniteStateMachine
{
    /// <summary>
    /// A finite state machine is in one state at a time, and has transitions between states.
    /// 
    /// This is an instance of a finite state machine, based on a FSMDefinition (scriptable objects),
    /// so this stores the current state and values of each parameter.
    /// </summary>
    public class FiniteStateMachineInstance
    {
        private MonoBehaviour _identifierForSelectiveListeners;
        private FSMDefinition _definition;
        private FSMState _currentState;
        private float _timeEnteredCurrentState;
        private bool _logTransitions;

        // names and values of parameters (like in Unity's animator)
        private Dictionary<FSMParameter, bool> _bools = new();
        private Dictionary<FSMParameter, bool> _triggers = new();
        private Dictionary<FSMParameter, float> _floats = new();

        public FiniteStateMachineInstance(FSMDefinition definition
            , MonoBehaviour identifierForSelectiveListeners
            , bool logTransitions)
        {
            _definition = definition;
            _identifierForSelectiveListeners = identifierForSelectiveListeners;
            _logTransitions = logTransitions;

            foreach (FSMParameter x in _definition.Parameters)
            {
                if (x.Type == FSMParameter.ParameterType.Bool)
                {
                    _bools.Add(x, x.InitialBool);
                }
                else if (x.Type == FSMParameter.ParameterType.Trigger)
                {
                    _triggers.Add(x, x.InitialTrigger);
                }
                else if (x.Type == FSMParameter.ParameterType.Float)
                {
                    _floats.Add(x, x.InitialFloat);
                }
            }

            _definition.CheckValid(_floats);
        }

        /// <summary>
        /// Checks for a transition, and returns true if it makes a transition.
        /// </summary>
        private bool CheckTransition()
        {
            float durationInCurrentState = Time.time - _timeEnteredCurrentState;
            FSMState newState = null;
            for (int i = 0; i < _definition.Transitions.Length; i++)
            {
                _definition.Transitions[i].Check(_currentState, _definition.DefaultState
                    , durationInCurrentState, _bools, _triggers, _floats, ref newState);

                if (newState != null)
                {
                    for (int j = i + 1; j < _definition.Transitions.Length; j++)
                    {
                        FSMTransition notChecked = _definition.Transitions[j];
                        if (notChecked.LogFailureReason && notChecked.FromCorrectState(_currentState))
                        {
                            Debug.Log("Failure reason: another transition happened because earlier in list", notChecked);
                        }
                    }

                    if (_logTransitions)
                    {
                        Debug.Log($"Transition at frame #{Time.frameCount}: \"{_currentState.name}\" --> \"{newState.name}\"");
                    }
                    _currentState.OnExit?.Raise(_identifierForSelectiveListeners);
                    _currentState = newState;
                    _timeEnteredCurrentState = Time.time;
                    _currentState.OnEnter?.Raise(_identifierForSelectiveListeners);
                    return true;
                }
            }
            return false;
        }

        public void SetBool(FSMParameter parameter, bool setTo)
        {
            if (!_bools.ContainsKey(parameter))
            {
                Debug.LogError("This finite state machine doesn't have the bool parameter being set.", parameter);
                return;
            }

            _bools[parameter] = setTo;
        }

        public void SetTrigger(FSMParameter parameter)
        {
            if (!_triggers.ContainsKey(parameter))
            {
                Debug.LogError("This finite state machine doesn't have the trigger parameter being set.", parameter);
                return;
            }

            _triggers[parameter] = true;
        }

        public void SetFloat(FSMParameter parameter, float setTo)
        {
            if (!_floats.ContainsKey(parameter))
            {
                Debug.LogError("This finite state machine doesn't have the float parameter being set.", parameter);
                return;
            }

            _floats[parameter] = setTo;
        }

        public bool GetBool(FSMParameter parameter)
        {
            if (!_bools.ContainsKey(parameter))
            {
                Debug.LogError("This finite state machine doesn't have the bool parameter.", parameter);
                return false;
            }

            return _bools[parameter];
        }

        public bool GetTrigger(FSMParameter parameter)
        {
            if (!_triggers.ContainsKey(parameter))
            {
                Debug.LogError("This finite state machine doesn't have the trigger parameter.", parameter);
                return false;
            }

            return _triggers[parameter];
        }

        public float GetFloat(FSMParameter parameter)
        {
            if (!_floats.ContainsKey(parameter))
            {
                Debug.LogError("This finite state machine doesn't have the float parameter.", parameter);
                return float.NaN;
            }

            return _floats[parameter];
        }

        /// <summary>
        /// Updates the state of this finite state machine, and raises events. Generally will be called
        /// once per frame.
        /// </summary>
        public void Update()
        {
            if (_currentState == null)
            {
                // Will only happen the 1st time this method runs.
                _currentState = _definition.DefaultState;
                _timeEnteredCurrentState = Time.time;
                _currentState.OnEnter?.Raise(_identifierForSelectiveListeners);
            }

            // Check transitions until no transition happens. Don't allow infinite loop.
            int numTransitions = 0;
            while (CheckTransition())
            {
                numTransitions++;
                if (numTransitions > 1000)
                {
                    Debug.LogError("Error: likely infinite loop in a finite state machine", _definition);
                    break;
                }
            }

            _currentState.OnUpdate?.Raise(_identifierForSelectiveListeners);
        }
    }
}
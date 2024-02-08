using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachine
{
    /// <summary>
    /// A parameter in a finite state machine, like in Unity's animator.
    /// </summary>
    [CreateAssetMenu(menuName = "Finite State Machine/Parameter")]
    public class FSMParameter : ScriptableObject
    {
        [field: SerializeField]
        public ParameterType Type { get; private set; } = ParameterType.Bool;

        [field: SerializeField]
        public bool InitialBool { get; private set; }

        [field: SerializeField]
        public bool InitialTrigger { get; private set; }

        [field: SerializeField]
        public float InitialFloat { get; private set; }

        public enum ParameterType
        {
            Bool, Trigger, Float
        }
    }
}
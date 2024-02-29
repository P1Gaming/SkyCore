#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FiniteStateMachine;
using UnityEditor;

namespace FiniteStateMachineEditor
{
    public class FSMEditorOneState : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rectTransform;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private GameObject _selectedIndicator;
        [SerializeField]
        private Image _border;
        [SerializeField]
        private Color _borderColorForDefaultState;
        [SerializeField]
        private Color _borderColorForAnystate;

        private Color _borderColorForNormalState;

        private FSMDefinition _fsmDefinition;
        private List<FSMEditorOneTransition> _connectedTransitions = new();
        private Vector3[] _fourCorners = new Vector3[4];

        public FSMState State { get; private set; }
        public bool IsForAnystate => State == null;

        public string Name => IsForAnystate ? "Any State" : State.name;

        public List<FSMEditorOneTransition> ConnectedTransitions => _connectedTransitions;

        public void Initialize(FSMState state, FSMDefinition fsmDefinition)
        {
            _borderColorForNormalState = _border.color;

            State = state;
            _fsmDefinition = fsmDefinition;

            UpdateText();

            _rectTransform.transform.position = FSMEditorSaveDataChanger.GetPositionOfState(_fsmDefinition, State);

            UpdateWhetherDefaultState();

            if (state == null)
            {
                _border.color = _borderColorForAnystate;
            }
        }

        public void UpdateText()
        {
            _text.text = Name;
        }

        public void AddConnectedTransition(FSMEditorOneTransition transition)
        {
            _connectedTransitions.Add(transition);
        }

        public bool WorldPointIsInsideRect(Vector2 worldPoint)
        {
            _rectTransform.GetWorldCorners(_fourCorners);
            Vector2 bottomLeft = _fourCorners[0]; // world space canvas
            Vector2 topRight = _fourCorners[2];

            return worldPoint.x > bottomLeft.x && worldPoint.x < topRight.x
                && worldPoint.y > bottomLeft.y && worldPoint.y < topRight.y;
        }

        public void SetSelected(bool selected)
        {
            _selectedIndicator.SetActive(selected);
        }

        public void UpdateWhetherDefaultState()
        {
            _border.color = State == _fsmDefinition.DefaultState ? _borderColorForDefaultState : _borderColorForNormalState;
        }

        public void Drag(Vector2 move)
        {
            _rectTransform.position += (Vector3)move;
            for (int i = 0; i < _connectedTransitions.Count; i++)
            {
                _connectedTransitions[i].UpdateLine();
            }
        }

        public void RemoveDeletedTransition(FSMEditorOneTransition transition)
        {
            _connectedTransitions.Remove(transition);
        }
        
    }
}
#endif
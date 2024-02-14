#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FiniteStateMachine;

namespace FiniteStateMachineEditor
{
    public class FSMEditorOneTransition : MonoBehaviour
    {
        [SerializeField]
        private LineRenderer _line;
        [SerializeField]
        private LineRenderer _arrowLeftHalf;
        [SerializeField]
        private LineRenderer _arrowRightHalf;
        [SerializeField]
        private Color _selectedColor;
        [SerializeField]
        private Color _colorWhileCreating;


        private Color _normalColor;

        public FSMEditorOneState From { get; private set; }
        public FSMEditorOneState To { get; private set; }
        public FSMTransition Transition { get; private set; }
        private int _nthBetweenSameTwoStates;

        private List<FSMEditorOneTransition> _existingTransitions;

        public void Initialize(FSMTransition transition, List<FSMEditorOneState> editorStates
            , List<FSMEditorOneTransition> existingTransitions)
        {
            _existingTransitions = existingTransitions;
            Transition = transition;
            From = FindAssociatedEditorState(transition.From, editorStates);
            To = FindAssociatedEditorState(transition.To, editorStates);
            From.AddConnectedTransition(this);
            To.AddConnectedTransition(this);

            _normalColor = _line.startColor;

            _nthBetweenSameTwoStates = NthBetweenSameTwoStates(_existingTransitions, From, To);

            UpdateLine();
        }

        public void InitializeSpecialOneForCreatingNewTransitions()
        {
            SetColor(_colorWhileCreating);
        }

        public float SqrDistanceFromLineSegment(Vector2 point)
        {
            GetLinePoints(out Vector2 a, out Vector2 b);

            // a and b are the line segment's ends.

            // stack overflow shortest distance between a point and a line segment


            Vector2 bMinusA = b - a;
            Vector2 pointMinusA = point - a;

            float sqrLength = bMinusA.sqrMagnitude;
            if (sqrLength == 0)
                return (point - a).sqrMagnitude;

            float t = Vector2.Dot(pointMinusA, bMinusA) / sqrLength;
            t = Mathf.Clamp01(t);

            Vector2 closestPointOnSegment = a + t * bMinusA;
            return (point - closestPointOnSegment).sqrMagnitude;
        }

        public void SetWhetherSelected(bool selected)
        {
            SetColor(selected ? _selectedColor : _normalColor);
        }

        private void SetColor(Color c)
        {
            _line.startColor = c;
            _line.endColor = c;
            _arrowLeftHalf.startColor = c;
            _arrowLeftHalf.endColor = c;
            _arrowRightHalf.startColor = c;
            _arrowRightHalf.endColor = c;
        }

        private FSMEditorOneState FindAssociatedEditorState(FSMState state, List<FSMEditorOneState> editorStates)
        {
            // TODO: deal with when from / to are null (meaning from anystate, or to default state. Maybe remove the latter functionality.)
            for (int i = 0; i < editorStates.Count; i++)
            {
                if (editorStates[i].State == state)
                    return editorStates[i];
            }
            throw new System.InvalidOperationException("didnt find the associated editorState");
        }

        public void UpdateLine()
        {
            GetLinePoints(out Vector2 posFrom, out Vector2 posTo);
            SetLine(posFrom, posTo);
        }

        public void SetLine(Vector2 posFrom, Vector2 posTo)
        {
            Vector3 displacement = posFrom - posTo;
            float distance = displacement.magnitude;
            float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg + 90f;

            transform.position = posFrom;
            _line.SetPosition(1, new Vector3(0, distance, 0));
            transform.rotation = Quaternion.Euler(0, 0, angle);

            Vector3 arrowPosition = new Vector3(0, distance * .6f, 0);
            _arrowLeftHalf.transform.localPosition = arrowPosition;
            _arrowRightHalf.transform.localPosition = arrowPosition;
        }

        private void GetLinePoints(out Vector2 posFrom, out Vector2 posTo)
        {
            Vector3 adjustmentToAvoidIdenticalLines = AdjustmentToAvoidIdenticalLines(_nthBetweenSameTwoStates);
            posFrom = From.transform.position + adjustmentToAvoidIdenticalLines;
            posTo = To.transform.position + adjustmentToAvoidIdenticalLines;
        }

        public static Vector2 AdjustmentToAvoidIdenticalLines(int nthBetweenSameTwoStates)
        {
            return new Vector3(Mathf.Min(140, 30 * nthBetweenSameTwoStates), 0);
        }

        public static int NthBetweenSameTwoStates(List<FSMEditorOneTransition> existingTransitions
           , FSMEditorOneState from, FSMEditorOneState to)
        {
            int result = 0;
            for (int i = 0; i < existingTransitions.Count; i++)
            {
                FSMEditorOneTransition other = existingTransitions[i];
                if (other.From == from && other.To == to || other.From == to && other.To == from)
                {
                    result++;
                }
            }
            return result;
        }
    }
}
#endif
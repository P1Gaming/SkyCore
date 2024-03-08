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
        private LineFor2DEditor _line;
        [SerializeField]
        private LineFor2DEditor _arrowLeftHalf;
        [SerializeField]
        private LineFor2DEditor _arrowRightHalf;
        [SerializeField]
        private Color _normalColor = Color.white;
        [SerializeField]
        private Color _selectedColor;
        [SerializeField]
        private Color _colorWhileCreating;

        public FSMEditorOneState From { get; private set; }
        public FSMEditorOneState To { get; private set; }
        public FSMTransition Transition { get; private set; }
        private int _nthBetweenSameTwoStates;

        private List<FSMEditorOneTransition> _existingTransitions;

        public void Initialize(FSMTransition transition, List<FSMEditorOneState> editorStates
            , List<FSMEditorOneTransition> existingTransitions, CameraInfo cameraInfo)
        {
            _existingTransitions = existingTransitions;
            Transition = transition;
            From = FindAssociatedEditorState(transition.From, editorStates);
            To = FindAssociatedEditorState(transition.To, editorStates);
            From.AddConnectedTransition(this);
            To.AddConnectedTransition(this);

            _nthBetweenSameTwoStates = NthBetweenSameTwoStates(_existingTransitions, null, From, To);

            _line.Initialize(cameraInfo);
            _arrowLeftHalf.Initialize(cameraInfo);
            _arrowRightHalf.Initialize(cameraInfo);

            UpdateLine();
            SetColor(_normalColor);
        }

        public void InitializeSpecialOneForCreatingNewTransitions(CameraInfo cameraInfo)
        {
            _line.Initialize(cameraInfo);
            _arrowLeftHalf.Initialize(cameraInfo);
            _arrowRightHalf.Initialize(cameraInfo);
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
            _line.SetColor(c);
            _arrowLeftHalf.SetColor(c);
            _arrowRightHalf.SetColor(c);
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

        public void UpdateNthBetweenSameTwoStates()
        {
            _nthBetweenSameTwoStates = NthBetweenSameTwoStates(_existingTransitions, this, From, To);
        }

        

        public void SetLine(Vector2 posFrom, Vector2 posTo)
        {
            _line.SetLine(posFrom, posTo);

            Vector2 displacement = posTo - posFrom;

            float rotationOfMainLine = Mathf.Atan2(displacement.y, displacement.x);
            Vector2 arrowLinesFrom = Vector2.Lerp(posFrom, posTo, .6f);
            Vector2 leftArrowDisplacement = RotateVector(new Vector2(0, 15), (45f + 90) * Mathf.Deg2Rad + rotationOfMainLine);
            Vector2 rightArrowDisplacement = RotateVector(new Vector2(0, 15), (-45f + 90) * Mathf.Deg2Rad + rotationOfMainLine);

            _arrowLeftHalf.SetLine(arrowLinesFrom, arrowLinesFrom + leftArrowDisplacement);
            _arrowRightHalf.SetLine(arrowLinesFrom, arrowLinesFrom + rightArrowDisplacement);

            UpdateLineWidth();
        }

        public void UpdateLineWidth()
        {
            // need to do this to keep the width constant pixel size, so whenever the camera size changes.
            _line.UpdateWidth();
            _arrowLeftHalf.UpdateWidth();
            _arrowRightHalf.UpdateWidth();
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
            , FSMEditorOneTransition transitionIfAlreadyAddedToList
           , FSMEditorOneState from, FSMEditorOneState to)
        {
            int result = 0;
            for (int i = 0; i < existingTransitions.Count; i++)
            {
                FSMEditorOneTransition other = existingTransitions[i];
                if (other == transitionIfAlreadyAddedToList)
                    return result;
                if (other.From == from && other.To == to || other.From == to && other.To == from)
                {
                    result++;
                }
            }
            return result;
        }

        private static Vector2 RotateVector(Vector2 v, float radians)
        {
            float cos = Mathf.Cos(radians);
            float sin = Mathf.Sin(radians);
            float x = cos * v.x - sin * v.y;
            float y = sin * v.x + cos * v.y;
            return new Vector2(x, y);
        }
    }
}
#endif
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachineEditor
{
    public class LineFor2DEditor : MonoBehaviour
    {
        private const float LINE_TOTAL_PIXEL_WIDTH = 3.5f;
        private const float LINE_BORDER_PIXEL_WIDTH = 3.5f;//1.6f;
        private const float POSITION_Z = 0f;

        [SerializeField]
        private MeshRenderer _renderer;

        private CameraInfo _cameraInfo;
        private MaterialPropertyBlock _propertyBlock;
        private LineShaderPropertyIDs _propertyIDs = new();

        private class LineShaderPropertyIDs
        {
            public int color = Shader.PropertyToID("_Color");
            public int colorFullyTransparent = Shader.PropertyToID("_ColorFullyTransparent");
            public int fractionWidthBeforeAntialias = Shader.PropertyToID("_FractionWidthBeforeAntialias");
        }

        public void Initialize(CameraInfo cameraInfo)
        {
            _cameraInfo = cameraInfo;
            _propertyBlock = new();

            float lineFractionWidthBeforeAntialias = (LINE_TOTAL_PIXEL_WIDTH - LINE_BORDER_PIXEL_WIDTH) / LINE_TOTAL_PIXEL_WIDTH;
            _propertyBlock.SetFloat(_propertyIDs.fractionWidthBeforeAntialias, lineFractionWidthBeforeAntialias);
        }

        public void SetColor(Color c)
        {
            _propertyBlock.SetColor(_propertyIDs.color, c);
            c.a = 0;
            _propertyBlock.SetColor(_propertyIDs.colorFullyTransparent, c);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        public void SetLine(Vector2 from, Vector2 to)
        {
            Vector2 midpoint = (from + to) / 2;

            transform.localPosition = new Vector3(midpoint.x, midpoint.y, POSITION_Z);
            Vector2 displacement = to - from;
            float rotation = Mathf.Atan2(displacement.y, displacement.x);
            transform.localRotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * rotation);

            float width = LINE_TOTAL_PIXEL_WIDTH * _cameraInfo.UnitsPerPixelWithDevelopmentScreenSize;
            float length = displacement.magnitude;
            transform.localScale = new Vector3(length, width, 1f);
        }

        public void UpdateWidth()
        {
            float width = LINE_TOTAL_PIXEL_WIDTH * _cameraInfo.UnitsPerPixelWithDevelopmentScreenSize;
            transform.localScale = new Vector3(transform.localScale.x, width, 1f);
        }
    }
}
#endif
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachineEditor
{
    public class CameraInfo
    {
        private Camera _mainCamera;


        public CameraInfo()
        {
            _mainCamera = Camera.main;
        }

        public float ZoomOut
        {
            get => _mainCamera.orthographicSize;
            set => _mainCamera.orthographicSize = value;
        }

        public Vector2 MouseWorldPosition => _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        public Vector2 MouseViewportPosition => _mainCamera.ScreenToViewportPoint(Input.mousePosition);

        public Vector2 ViewportSizeWorldUnits
        {
            get
            {
                float cameraHeightInWorldUnits = _mainCamera.orthographicSize * 2f;
                return new Vector2(cameraHeightInWorldUnits * _mainCamera.pixelWidth / _mainCamera.pixelHeight, cameraHeightInWorldUnits);
            }
        }

        public Vector2 CameraPosition
        {
            get => _mainCamera.transform.position;
            set => _mainCamera.transform.position = new Vector3(value.x, value.y, _mainCamera.transform.position.z);
        }

        public float UnitsPerPixelWithDevelopmentScreenSize => _mainCamera.orthographicSize * 2f / 1080;



        public Vector2 ViewportPoint(Vector2 x) => _mainCamera.WorldToViewportPoint(x);

        public void MoveWorldPosToViewportPos(Vector2 fromWorldPos, Vector2 toViewportPos)
        {
            Vector2 translation = TranslationToMoveWorldPosToViewportPos(fromWorldPos, toViewportPos);
            TranslateCamera(translation);
        }

        public Vector2 TranslationToMoveWorldPosToViewportPos(Vector2 fromWorldPos, Vector2 toViewportPos)
        {
            Vector2 viewportPosChange = ViewportPoint(fromWorldPos) - toViewportPos;
            return Vector2.Scale(viewportPosChange, ViewportSizeWorldUnits);
        }

        public void TranslateCamera(Vector2 displacementRelativeCameraRotation)
        {
            _mainCamera.transform.Translate(displacementRelativeCameraRotation);
        }
    }
}
#endif
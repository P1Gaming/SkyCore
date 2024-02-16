#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FiniteStateMachineEditor
{
    public class FSMEditorCameraControls
    {
        private readonly string PAN_BUTTON = "right click";
        private const float ZOOM_MULTIPLICATION_SPEED = .2f;

        private const float MIN_ZOOMOUT_IS_FRACTION_OF_MAX_ZOOMOUT = .01f;
        private const float SCROLL_PER_SEC = 1f;
        private const float FRACTION_OF_RECENT_SCROLL_PER_SEC = 5f;

        private const float PAN_PER_SEC_PER_ZOOM = .1f;
        private const float FRACTION_OF_RECENT_PAN_PER_SEC = 5f;

        private RectTransform _workspace;
        private RectTransform _nonZoomedAreaLeftForSeeingWorkspace;
        private Vector3[] _fourCorners = new Vector3[4];
        private float _minZoomOut;
        private float _maxZoomOut;
        private Camera _mainCamera;
        private CameraInfo _cameraInfo;
        private Vector2 _priorMouseViewportPos;
        private Vector2 _recentPan = Vector2.zero;
        private float _recentMouseScroll;
        private bool _pannedEver = false;
        public bool ApplicationGainedFocusPriorFrame { get; private set; }

        public FSMEditorCameraControls(RectTransform workspace, RectTransform nonZoomedAreaLeftForSeeingWorkspace
            , CameraInfo cameraInfo)
        {
            _workspace = workspace;
            _nonZoomedAreaLeftForSeeingWorkspace = nonZoomedAreaLeftForSeeingWorkspace;
            _cameraInfo = cameraInfo;
            _mainCamera = Camera.main;

            float workspaceWidth = _workspace.rect.width;
            float workspaceHeight = _workspace.rect.height;
            float visibleAreaWidth = _nonZoomedAreaLeftForSeeingWorkspace.rect.width;
            float visibleAreaHeight = _nonZoomedAreaLeftForSeeingWorkspace.rect.height;
            _maxZoomOut = workspaceWidth * 1080 / visibleAreaWidth / 2;
            _maxZoomOut = Mathf.Min(_maxZoomOut, workspaceHeight * 1080 / visibleAreaHeight / 2);
            _minZoomOut = _maxZoomOut * MIN_ZOOMOUT_IS_FRACTION_OF_MAX_ZOOMOUT;
        }

        public void MoveCameraToPutWorldPosInCenterOfScreenSpaceRect(Vector2 worldPosition, RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(_fourCorners); // "world" is a bad name here, it just means global
            Vector2 bottomLeft = _fourCorners[0];
            Vector2 topRight = _fourCorners[2];
            Vector2 center = bottomLeft + (topRight - bottomLeft) / 2;
            Vector2 currentCenterWorldPos = _mainCamera.ScreenToWorldPoint(center);
            _cameraInfo.CameraPosition += worldPosition - currentCenterWorldPos;
        }

        public void OnUpdate()
        {
            // pan and zoom with mouse
            AddToRecentPan();
            ApplyPan();

            if (!ApplicationGainedFocusPriorFrame && MouseIsInsideZoomableArea())
            {
                // when the game view regains focus, mouseScrollDelta suddenly gains
                // all the scroll from while it wasn't focused. (prior frame b/c
                // OnApplicationFocus is called after Update())
                _recentMouseScroll += Input.mouseScrollDelta.y;
            }

            ApplyZoom();
            KeepCameraPositionInBounds();

            _priorMouseViewportPos = _cameraInfo.MouseViewportPosition;
            ApplicationGainedFocusPriorFrame = false;
        }

        public void OnApplicationFocus(bool focus)
        {
            if (focus)
                ApplicationGainedFocusPriorFrame = true;
        }

        private void AddToRecentPan()
        {
            Vector2 mouseViewportPosChange = _priorMouseViewportPos - _cameraInfo.MouseViewportPosition;
            Vector2 pan = Vector2.Scale(_cameraInfo.ViewportSizeWorldUnits, mouseViewportPosChange);

            bool pannedAlready = _pannedEver;
            _pannedEver = Input.GetButton(PAN_BUTTON);
            if (!pannedAlready)
            {
                // Without this, the camera position changes upon first time starting to pan.
                // (confirm this is still an issue, maybe can remove this)
                return;
            }

            if (Input.GetButton(PAN_BUTTON))
            {
                // Keep the mouse at the same world position. (Except it's different with a rotating camera.)
                _recentPan += pan;
            }
        }

        private void ApplyPan()
        {
            float panLeft = _recentPan.magnitude;

            // fractionalPan has the same explanation as described for zoom's fractionalScroll.
            float fractionalPan = panLeft * (1f - Mathf.Exp(-Time.unscaledDeltaTime * FRACTION_OF_RECENT_PAN_PER_SEC));
            float maxPan = Time.unscaledDeltaTime * PAN_PER_SEC_PER_ZOOM * _cameraInfo.ZoomOut + Mathf.Max(0, fractionalPan);
            float panToUse = Mathf.Min(maxPan, panLeft);

            Vector2 pan = panToUse * _recentPan.normalized;
            _recentPan -= pan;

            _cameraInfo.TranslateCamera(pan);
        }

        private void ApplyZoom()
        {
            if (_recentMouseScroll == 0)
            {
                return;
            }

            Vector2 mousePositionBeforeZoom = _cameraInfo.MouseWorldPosition;
            Vector2 mouseViewportPosBeforeZoom = _cameraInfo.MouseViewportPosition;

            #region explanation of fractionalScroll
            // Unless _recentMouseScroll is too low, use an amount proportional to it (so you can scroll faster and it'll
            // zoom faster), plus a little (so the camera zoom stops changing eventually).
            // The fraction to use shouldn't depend on dt, e.g. the same amount left after two dts of .05 or one dt of .1.
            // So repeatedly take a little fraction many times.

            // Without that, fractionalScroll = Time.deltaTime * FRACTION_OF_RECENT_SCROLL_PER_SEC * Mathf.Abs(_recentMouseScroll).
            // Take away from Mathf.Abs(_recentMouseScroll) in small chunks of delta time.

            // x is the amount left over so far, so initially Mathf.Abs(_recentMouseScroll). 
            // float k = Time.deltaTime * FRACTION_OF_RECENT_SCROLL_PER_SEC;

            // for (int i = 0; i < 10000; i++)
            //     x *= (1 - k / 10000);

            // equivalent:

            // float multiplier = 1;
            // for (int i = 0; i < 10000; i++)
            //     multiplier *= (1 - k / 10000);
            // x *= multiplier

            // lim((1 - k / z)^z) as z -> infinity is e ^ -k 
            // So multiplier = e ^ -k, so amount left over = x * e ^ -k, so amount to use = x - x * e ^ -k = x * (1 - e ^ -k)
            #endregion
            float fractionalScroll = Mathf.Abs(_recentMouseScroll) * (1f - Mathf.Exp(-Time.unscaledDeltaTime * FRACTION_OF_RECENT_SCROLL_PER_SEC));
            float maxScroll = Time.unscaledDeltaTime * SCROLL_PER_SEC + Mathf.Max(0, fractionalScroll);
            float scrollToUse = _recentMouseScroll;
            if (Mathf.Abs(scrollToUse) > maxScroll)
            {
                scrollToUse = Mathf.Sign(_recentMouseScroll) * maxScroll;
            }
            _recentMouseScroll -= scrollToUse;

            #region explanation of newZoom
            // Multiply zoom by 1 - ZOOM_MULTIPLICATION_SPEED * scroll.
            // Except it shouldn't depend on e.g. delta time, so do that many times with many pieces of scroll.
            // lim(1 - x/z)^z as z -> infinity is e^-x.
            #endregion
            float newZoom = _cameraInfo.ZoomOut * Mathf.Exp(-ZOOM_MULTIPLICATION_SPEED * scrollToUse);
            newZoom = Mathf.Clamp(newZoom, _minZoomOut, _maxZoomOut);
            _cameraInfo.ZoomOut = newZoom;

            // Keep the mouse at the same world position
            _cameraInfo.MoveWorldPosToViewportPos(mousePositionBeforeZoom, mouseViewportPosBeforeZoom);
        }

        private void KeepCameraPositionInBounds()
        {
            Vector2 workspaceBottomLeft = _workspace.rect.min; // _workspace is in a world space canvas
            Vector2 workspaceTopRight = _workspace.rect.max;

            _nonZoomedAreaLeftForSeeingWorkspace.GetWorldCorners(_fourCorners);
            Vector2 visibleAreaBottomLeft = _mainCamera.ScreenToWorldPoint(_fourCorners[0]);
            Vector2 visibleAreaTopRight = _mainCamera.ScreenToWorldPoint(_fourCorners[2]);

            Vector2 newPos = _cameraInfo.CameraPosition;

            if (visibleAreaBottomLeft.x < workspaceBottomLeft.x)
                newPos.x -= visibleAreaBottomLeft.x - workspaceBottomLeft.x;
            if (visibleAreaBottomLeft.y < workspaceBottomLeft.y)
                newPos.y -= visibleAreaBottomLeft.y - workspaceBottomLeft.y;
            if (visibleAreaTopRight.x > workspaceTopRight.x)
                newPos.x -= visibleAreaTopRight.x - workspaceTopRight.x;
            if (visibleAreaTopRight.y > workspaceTopRight.y)
                newPos.y -= visibleAreaTopRight.y - workspaceTopRight.y;
            _cameraInfo.CameraPosition = newPos;
        }

        private bool MouseIsInsideZoomableArea()
        {
            _nonZoomedAreaLeftForSeeingWorkspace.GetWorldCorners(_fourCorners); // "world" is screen here (but not local space)
            Vector2 visibleAreaBottomLeft = _fourCorners[0];
            Vector2 visibleAreaTopRight = _fourCorners[2];
            return Input.mousePosition.x > visibleAreaBottomLeft.x && Input.mousePosition.x < visibleAreaTopRight.x
                && Input.mousePosition.y > visibleAreaBottomLeft.y && Input.mousePosition.y < visibleAreaTopRight.y;
        }
    }
}
#endif
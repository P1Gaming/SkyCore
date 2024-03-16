using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// Don't use CinemachineVirtualCamera.enabled. Its code controls that internally.

public class CameraSystem : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera _firstPersonCamera;
    [SerializeField]
    private CinemachineVirtualCamera _tutorialCamera;
    [SerializeField]
    private CinemachineVirtualCamera _cutsceneCamera;

    private CinemachineVirtualCamera _currentCamera;

    private static CameraSystem _instance;
    private static CameraSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CameraSystem>();
                _instance._currentCamera = FirstPersonCamera;
            }
            return _instance;
        }
    }

    public static CinemachineVirtualCamera FirstPersonCamera => Instance._firstPersonCamera;
    public static CinemachineVirtualCamera TutorialCamera => Instance._tutorialCamera;
    public static CinemachineVirtualCamera CutsceneCamera => Instance._cutsceneCamera;

    public static bool PauseCameraMovement
    {
        get => Instance._currentCamera.gameObject.activeSelf;
        set => Instance._currentCamera.gameObject.SetActive(!value);
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _currentCamera = FirstPersonCamera;
        }
        
    }

    public static void SwitchToFirstPersonCamera() => Instance.SwitchTo(Instance._firstPersonCamera);
    public static void SwitchToTutorialCamera() => Instance.SwitchTo(Instance._tutorialCamera);
    public static void SwitchToCutsceneCamera() => Instance.SwitchTo(Instance._cutsceneCamera);

    private void SwitchTo(CinemachineVirtualCamera camera)
    {
        _currentCamera.gameObject.SetActive(false);
        _currentCamera = camera;
        _currentCamera.gameObject.SetActive(true);
    }
}

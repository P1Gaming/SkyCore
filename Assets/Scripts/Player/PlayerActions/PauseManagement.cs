using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Player.View;
using System;

public class PauseManagement : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera[] _virtualCamera;
    [SerializeField]
    private FirstPersonView _firstPersonView;
    [SerializeField]
    private GameObject _jellyInteractVisual;
    [SerializeField, Range(0f,1f)]
    private float _pauseTimeScaleVariance = .001f;

    private PlayerActions _pauseAction;
    private bool _paused = false;
    private bool _inventoryOpen = false;

    private EscMenuToggle _pMenu;

    private static PauseManagement _instance;
    public static PauseManagement Instance
    {
        get
        {
            if (_instance == null)
            {
                // Need to do this if another script's Awake needs it before this
                // one's Awake runs
                _instance = FindObjectOfType<PauseManagement>();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);

        _pauseAction = new PlayerActions();
        _pauseAction.UI.Pause.performed += _ => DeterminePause();
        _paused = !_paused;

        _pMenu = FindObjectOfType<EscMenuToggle>();

        _pMenu.SetActive(false);
        _firstPersonView.enabled = true;
    }

    private void OnEnable()
    {
        _pauseAction.Enable();
    }

    private void OnDisable()
    {
        _pauseAction.Disable();
    }

    private void SetPause(bool pause)
    {
        _paused = pause;
        _pMenu.SetActive(!pause);
        if (Convert.ToInt32(pause) == 0)
        {
            Time.timeScale = Convert.ToInt32(pause);
        }
        else
        {
            Time.timeScale = Convert.ToInt32(pause);
        }
        Cursor.visible = !pause;
        Cursor.lockState = pause ? CursorLockMode.Locked : CursorLockMode.None;
        foreach (var CinemachineVirtualCamera in _virtualCamera)
        {
            CinemachineVirtualCamera.enabled = pause;
        }
        _firstPersonView.enabled = pause;
        _jellyInteractVisual.SetActive(pause);
    }

    // <summary> 
    // Controls the state of the pause menu.
    // Has the ability to be disabled by other scripts if needed,
    // to prevent uneccesary bugs with movement and other UIs.
    // <summary>
    public void DeterminePause()
    {
        if (!JellyInteractBase.AnyInteracting && !_inventoryOpen)
        {
            SetPause(!_paused);
        }
    }

    public void OnSetPauseButton(bool state)
    {
        _paused = state;
        DeterminePause();
    }

    public bool IsPaused()
    {
        //With the way paused is set up right now, need to return the opposite
        return !_paused;
    }

    public void InventoryInteraction(bool report)
    {
        //Placeholder until namespace issues are solved, cannot add namespace UI.Inventory to the script.
        _inventoryOpen = report;
    }

}

using Jellies;
using Player;
using UI.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Class managing detection of interactble objects by the player
/// </summary>
public class PlayerInteraction : MonoBehaviour
{

    // The closest raycast hit
    private RaycastHit _closestHit = new RaycastHit();

    [SerializeField]
    private GameEventScriptableObject _onShowInteractionGUI;

    [SerializeField]
    private GameEventScriptableObject _onHideInteractionGUI;

    // How close the player must be to interact with an object
    [SerializeField]
    private float _interactionDistance = 3f;

    // The main camera transform
    [SerializeField]
    private Camera _mainCamera;

    // The first person camera transform
    [SerializeField]
    private Transform _fpsCamera;

    // The layers that should be checked for interaction
    [SerializeField]
    private LayerMask _interactableLayer;

    // The current interactable if any
    [SerializeField]
    private Interactable _interactable;

    private JellyInteractBase _jellyCurrent;

    private static bool _inventoryOpen;

    /// <summary>
    /// If there is a raycast hit and no interactable assigned, enter interaction
    /// If there is a raycast hit and an interactable assigned, stay in interaction
    /// If there is not a raycast hit and an interactable assign, leave interaction
    /// </summary>
    private void Awake()
    {
        gameObject.GetComponent<PlayerInput>().onActionTriggered += HandleAction;
    }

    void Update()
    {
        if (CheckRaycast())
        {
            if (!_interactable)
            {
                if (_closestHit.collider.gameObject.GetComponent<Interactable>())
                {
                    OnInteractionEnter();
                }
            }
            else
            {
                OnInteractionStay();
            }
        }
        else if (_interactable)
        {
            OnInteractionExit();
        }
    }

    /// <summary>
    /// Set the reference of this interactable to the raycast hit's, and enable the interactable
    /// </summary>
    private void OnInteractionEnter()
    {
        _interactable = _closestHit.collider.gameObject.GetComponent<Interactable>();
        _interactable.enabled = true;
        _onShowInteractionGUI.Raise();
        // Debug.Log("Interaction with " + closestHit.collider.gameObject + " enabled.");

        if (_interactable.gameObject.GetComponent<JellyInteractBase>() != null)
        {
            _jellyCurrent = _interactable.gameObject.GetComponent<JellyInteractBase>();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    private void OnInteractionStay()
    {
        if (Input.GetButtonDown("Interaction Button"))
        {
            //this is is a kludge, there will need to be a system for determining which interaction to use
            //but doing it like this for now b/c some functionality is better than none
            _interactable.Interact(0, this);
        }
    }

    /// <summary>
    /// Disable the selected interactable, and set the reference to null
    /// </summary>
    private void OnInteractionExit()
    {
        _interactable.enabled = false;
        _onHideInteractionGUI.Raise();
        // Debug.Log("Interaction with " + interactable.gameObject + " disabled.");
        _interactable = null;
        _jellyCurrent = null;
    }

    /// <summary>
    /// Check for objects within interaction distance
    /// </summary>
    /// <returns>True if the raycast hits an object</returns>
    private bool CheckRaycast()
    {
        Ray ray = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        float distance = _interactionDistance + Vector3.Distance(_mainCamera.transform.position, _fpsCamera.position);
        return Physics.Raycast(ray, out _closestHit, distance, _interactableLayer, QueryTriggerInteraction.Ignore);
    }

    private void HandleAction(InputAction.CallbackContext context)
    {
        if (context.action.name == "Interact" && _jellyCurrent != null)
        {
            if (!_jellyCurrent.Interacting && !_inventoryOpen)
            {
                _jellyCurrent.InteractStart();
            }
        }
    }

    public static void InventoryState(bool state)
    {
        _inventoryOpen = state;
    }

}
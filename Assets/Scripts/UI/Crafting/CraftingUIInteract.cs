using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// Manages the player to be able to open Crafting UI from pressing c when looking and close to the workbench
/// </summary>
public class CraftingUIInteract : MonoBehaviour
{
    [SerializeField]
    private float _interactionDistance = 2f; // distance for when you can interact with the workbench
    private InputAction _interactionAction; // Use Key C to interact with crafting UI
    [SerializeField]
    private GameObject _workBench; // Reference to the workbench GameObject
    [SerializeField]
    private GameObject _workBenchUI; // Reference to the workbench UI GameObject

    public bool IsNearWorkBench { get; private set; } // Indicates if the player is near the workbench
    public float InteractionDistance { get { return _interactionDistance; } }
    private bool _isUIOpen = false; // Indicates if the workbench UI is open
    private PlayerInput _playerInput; // Reference to the PlayerInput component

    private MenuController _menuController; // Reference to the MenuController script
    private Page _workbenchPage; // Reference to the workbench page in the menu

    /// <summary>
    /// Enable the interaction action when the script is enabled
    /// The interaction action is associated with the "Crafting" input action 
    /// Allows the player to trigger the interaction with the workbench.
    /// </summary> 
    private void OnEnable()
    {
        _interactionAction?.Enable();
    }

    /// <summary>
    /// Disable the interaction action when the script is enabled
    /// The interaction action is associated with the "Crafting" input action 
    /// Allows the player to trigger the interaction with the workbench.
    /// </summary> 
    private void OnDisable()
    {
        _interactionAction?.Disable();
    }

    /// <summary>
    /// Initializes references to the MenuController and Page components, 
    /// As well as the PlayerInput component attached to the player
    /// Locks the cursor and subscribes to the "performed" event
    /// </summary> 
    private void Start()
    {
        _menuController = FindObjectOfType<MenuController>(); // Find the MenuController component
        _workbenchPage = _workBenchUI.GetComponent<Page>(); // Get the Page component from the workbench UI

        _playerInput = GetComponent<PlayerInput>(); // Get the PlayerInput component attached to the player

        LockCursor(); // Lock the cursor at the start

        if (TryGetComponent(out PlayerInput input))
        {
            _interactionAction = input.actions.FindAction("Crafting"); // Find the "Crafting" action in the input actions
            if (_interactionAction != null)
            {
                _interactionAction.performed += ctx => Interact(); // The "performed" event of the interaction action
            }
        }
    }

    /// <summary>
    /// Handles the player's interaction with the workbench
    /// Player is near the workbench, it toggles the visibility of the workbench UI
    /// Adds or removes the workbench page from the menu
    /// </summary>
    private void Interact()
    {
        if (IsNearWorkBench)
        {
            if (_isUIOpen)
            {
                LockCursor(); // Lock the cursor when the workbench UI is inactive
                EnablePlayerMovement(); // Enable player movement
                _menuController.PopPage(); // Remove the workbench page from the menu
                _workBenchUI.GetComponent<CraftingUICode>().OnClose();
            }
            else
            {
                UnlockCursor(); // Unlock the cursor when the workbench UI is active
                DisablePlayerMovement(); // Disable player movement
                _menuController.PushPage(_workbenchPage); // Add the workbench page to the menu
            }

            _isUIOpen = !_isUIOpen; // Toggle the UI open state
        }
    }

    /// <summary>
    /// Fixed interval and calls the DetectWorkBench() function to check if the player is near the workbench
    /// </summary>
    private void FixedUpdate()
    {
        DetectWorkBench();
    }

    /// <summary>
    /// Casts a ray from the player's position forward and checks if it gggoes through workbench
    /// If you detect the workbench, flag is set to true; otherwise, it's set to false
    /// </summary>
    private void DetectWorkBench()
    {
        // Create a ray from the player forward
        Ray ray = new Ray(transform.position, transform.forward);

        // Check if the player is near the workbench
        if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance))
        {
            if (hit.collider.gameObject == _workBench)
            {
                IsNearWorkBench = true;
            }
            else
            {
                IsNearWorkBench = false;
            }
        }
        else
        {
            IsNearWorkBench = false;
        }
    }

    /// <summary>
    /// Toggles the active state of the workbench UI and updates the _isUIOpen flag
    /// Also enables/disables player movement and locks/unlocks the cursor
    /// </summary>
    private void ToggleWorkBenchUI()
    {
        _workBenchUI.SetActive(!_workBenchUI.activeSelf); // Toggle the workbench UI active state
        _isUIOpen = _workBenchUI.activeSelf; // Update the UI open state

        if (_isUIOpen)
        {
            UnlockCursor(); // Unlock the cursor when the workbench UI is active
            DisablePlayerMovement(); // Disable player movement
        }
        else
        {
            LockCursor(); // Lock the cursor when the workbench UI is inactive
            EnablePlayerMovement(); // Enable player movement
        }
    }

    /// <summary>
    /// Locks the cursor and hides it
    /// </summary>
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor in the center of the screen
        Cursor.visible = false; // Hide the cursor
    }

    /// <summary>
    /// Unlocks the cursor and makes it appear
    /// </summary>
    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    /// <summary>
    /// Disables the "Move" and "Look" actions of the player input
    /// </summary>
    private void DisablePlayerMovement()
    {
        _playerInput.currentActionMap.FindAction("Move").Disable(); // Disable the "Move" action
        _playerInput.currentActionMap.FindAction("Look").Disable(); // Disable the "Look" action
    }

    /// <summary>
    /// Enables the "Move" and "Look" actions of the player input
    /// </summary>
    private void EnablePlayerMovement()
    {
        _playerInput.currentActionMap.FindAction("Move").Enable(); // Enable the "Move" action
        _playerInput.currentActionMap.FindAction("Look").Enable(); // Enable the "Look" action
    }
}
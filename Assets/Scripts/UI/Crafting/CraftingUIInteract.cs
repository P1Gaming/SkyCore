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
    private GameObject _workBenchUI; // Reference to the workbench UI GameObject

    public bool IsNearWorkBench { get; private set; } // Indicates if the player is near the workbench
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
                CursorMode.RemoveReasonForFreeCursor();
                InputIgnoring.ChangeNumberOfReasonsToIgnoreInputsForMovementAndInteractionThings(false);
                _menuController.PopPage(); // Remove the workbench page from the menu
                _workBenchUI.GetComponent<CraftingUICode>().OnClose();
            }
            else
            {
                CursorMode.AddReasonForFreeCursor();
                InputIgnoring.ChangeNumberOfReasonsToIgnoreInputsForMovementAndInteractionThings(true);

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
        // this code seems like the PlayerInteraction script. Not sure if it's identical.
        // Maybe we can merge this into the interaction system.
        // This code probably hasn't been worked on for a long time, not sure what state it's in. I've never crafted anything.

        // Create a ray from the player forward
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);


        // Uncomment this line of code to see a visualization of the ray cast in both edit and play modes.
        /*
        Debug.DrawLine(Camera.main.transform.position,
                       Camera.main.transform.position + Camera.main.transform.forward * _interactionDistance,
                       Color.blue,
                       0.5f);
        */


        // Check if the player is near the workbench
        if (Physics.Raycast(ray, out RaycastHit hit, _interactionDistance))
        {
            if (hit.collider.gameObject.CompareTag("WorkBench"))
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
}
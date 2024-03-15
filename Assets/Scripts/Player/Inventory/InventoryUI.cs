using UnityEngine;
using UnityEngine.InputSystem;
using Player.Motion;
using Player.View;


public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private int _hotBarStacksCapacity = 3;
    [SerializeField]
    private int _resourceSectionStacksCapacity = 12;
    [SerializeField]
    private int _jellySectionStacksCapacity = 15;
    [SerializeField]
    private int _toolSectionStacksCapacity = 9;

    [SerializeField]
    private GameObject _interactableVisual;
    [SerializeField]
    private GameObject _hotBarHighlight;


    [SerializeField]
    private GameObject _inventorySlotPrefab;

    [SerializeField, Tooltip("Controls open close animation of backpack panel")]
    private Animator _animator;

    [SerializeField, Tooltip("Add the GameObject representing thr hotbar that has a grid layout group component")]
    private GameObject _hotBarGrid;


    [SerializeField, Tooltip("Add the GameObject representing an inventory section that has a grid layout group component")]
    private GameObject _inventoryGridR;

    [SerializeField, Tooltip("Add the GameObject representing an inventory section that has a grid layout group component")]
    private GameObject _inventoryGridJelly;

    [SerializeField, Tooltip("Add the GameObject representing an inventory section that has a grid layout group component")]
    private GameObject _inventoryGridTool;

    [SerializeField]
    private Transform _itemParentDuringDragAndDrop;

    [SerializeField]
    private float _animationDuration = .5f;

    private InputAction _backpackAction;
    private bool _isInBackpackMode;
        
    public InventoryDragAndDrop _dragAndDrop;

    private static InventoryUI _instance;
    public static InventoryUI Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindGameObjectWithTag("InventoryUI")?
                    .GetComponent<InventoryUI>();
            }
            return _instance;
        }
    }

    public InventorySection HotbarSection { get; private set; }
    public InventorySection ResourceSection { get; private set; }
    public InventorySection JellySection { get; private set; }
    public InventorySection ToolSection { get; private set; }

    public GameObject HotbarHighlight => _hotBarHighlight;
    public GameObject HotBarGrid => _hotBarGrid;



    private void Awake()
    {
        _dragAndDrop = new InventoryDragAndDrop();

        ResourceSection = new InventorySection(_resourceSectionStacksCapacity, ItemIdentity.ItemSortType.Resource
            , _inventorySlotPrefab, _inventoryGridR, _itemParentDuringDragAndDrop, _dragAndDrop, null);

        JellySection = new InventorySection(_jellySectionStacksCapacity, ItemIdentity.ItemSortType.JellyItem
            , _inventorySlotPrefab, _inventoryGridJelly, _itemParentDuringDragAndDrop, _dragAndDrop, null);

        ToolSection = new InventorySection(_toolSectionStacksCapacity, ItemIdentity.ItemSortType.Tool
            , _inventorySlotPrefab, _inventoryGridTool, _itemParentDuringDragAndDrop, _dragAndDrop, null);

        HotbarSection = new InventorySection(_hotBarStacksCapacity, ItemIdentity.ItemSortType.None
            , _inventorySlotPrefab, _hotBarGrid, _itemParentDuringDragAndDrop, _dragAndDrop
            , new InventorySection[] { ResourceSection, JellySection, ToolSection });


        GameObject player = GameObject.FindGameObjectWithTag("Player");

        PlayerInput playerInput = player.GetComponent<PlayerInput>();
        _backpackAction = playerInput.actions.FindAction("Backpack", true);
        _backpackAction.performed += OnBackpack;
    }

    private void OnEnable()
    {
        _backpackAction?.Enable();
    }

    private void OnDisable()
    {
        _backpackAction?.Disable();
    }

    private void LateUpdate()
    {
        _dragAndDrop.OnLateUpdate();

        // The animation should be 1 second long.
        // The InventoryTime parameter is the position in the animation (see the animator state).
        float animationTime = _animator.GetFloat("InventoryTime");
        animationTime += (_isInBackpackMode ? 1 : -1) * Time.unscaledDeltaTime / _animationDuration;
        animationTime = Mathf.Clamp01(animationTime);
        _animator.SetFloat("InventoryTime", animationTime);
    }

        

    private void OnBackpack(InputAction.CallbackContext context)
    {
        Debug.Log("Pressing Inventory");

        bool becomeInBackpackMode = !_isInBackpackMode;

        if (becomeInBackpackMode && PlayerMovement.Instance.IsMoving)
        {
            //the player being able to open the inventory while moving causes a sliding bug
            //    -is it still a bug?
            Debug.Log("Cancel pressing inventory because of player movement.");
            return;
        }
        else if(becomeInBackpackMode && PauseManagement.Instance.IsPaused())
        {
            //the player has the game pause
            Debug.Log("Cancel pressing inventory because of pause game.");
            return;
        }
        else if (becomeInBackpackMode && JellyInteractBase.AnyInteracting)
        {
            //the player has the game pause
            Debug.Log("Cancel pressing inventory because of jelly Interaction");
            return;
        }
        else if (becomeInBackpackMode && IslandHeartInteractBase.AnyInteracting)
        {
            //the player has the game paused
            Debug.Log("Cancel pressing inventory because of island heart Interaction");
            return;
        }



        PauseManagement.Instance.InventoryInteraction(becomeInBackpackMode); // notify pause manager that inventory is open
        _isInBackpackMode = becomeInBackpackMode;
        Cursor.visible = becomeInBackpackMode;
        Cursor.lockState = becomeInBackpackMode ? CursorLockMode.None : CursorLockMode.Locked;

        FirstPersonView.Instance.enabled = !becomeInBackpackMode;
        PlayerMovement.Instance.enabled = !becomeInBackpackMode;
        _interactableVisual.SetActive(!becomeInBackpackMode);

        PlayerInteraction.InventoryState(becomeInBackpackMode);

        if (becomeInBackpackMode)
        {
            _dragAndDrop.EnableInput();
            
        }
        else
        {
            _dragAndDrop.DisableInputAndStop();
            PlayerMovement.Instance.enabled = true;
        }
    }







}
using Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Player.Motion;
using Player.View;

namespace UI.Inventory
{
    /// <summary>
    /// Organizes items into inventory slots, and handles the UI for inventory.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField]
        private FirstPersonView _firstPersonView;
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
        private InventoryBaseUI _hotBarUI;
        private InventoryBaseUI _inventoryUIR;
        private InventoryBaseUI _inventoryUIJ;
        private InventoryBaseUI _inventoryUIT;
        private InventoryDragAndDrop _dragAndDrop;

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

        public GameObject HotbarHighlight => _hotBarHighlight;
        public InventoryBaseUI HotbarUI => _hotBarUI;
        public GameObject HotBarGrid => _hotBarGrid;


        private void Awake()
        {
            // need to do this in Awake b/c needs to happen before OnEnable (Start() happens after OnEnable()).
            // Maybe all the initialization can just be in Awake.

            InventoryBase hotBarNonUI = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScene>().HotBar;
            _hotBarUI = new InventoryBaseUI(_inventorySlotPrefab, _hotBarGrid, hotBarNonUI, _itemParentDuringDragAndDrop
                , ItemBase.ItemSortType.None);

            InventoryBase inventoryR
                = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScene>().InventoryResource;
            _inventoryUIR = new InventoryBaseUI(_inventorySlotPrefab, _inventoryGridR
                , inventoryR, _itemParentDuringDragAndDrop, ItemBase.ItemSortType.Resource);

            InventoryBase inventoryJ
                = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScene>().InventoryJelly;
            _inventoryUIJ = new InventoryBaseUI(_inventorySlotPrefab, _inventoryGridJelly
                , inventoryJ, _itemParentDuringDragAndDrop, ItemBase.ItemSortType.JellyItem);

            InventoryBase inventoryT
                = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScene>().InventoryTool;
            _inventoryUIT = new InventoryBaseUI(_inventorySlotPrefab, _inventoryGridTool
                , inventoryT, _itemParentDuringDragAndDrop, ItemBase.ItemSortType.Tool);

            
        }

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            PlayerInput playerInput = player == null ? null : player.GetComponent<PlayerInput>();
            _backpackAction = playerInput == null ? null : playerInput.actions.FindAction("Backpack", true);
            if (_backpackAction != null)
            {
                _backpackAction.performed += OnBackpack;
            }

            _dragAndDrop = new InventoryDragAndDrop();
            _dragAndDrop.DisableInputAndStop();

            _hotBarUI.InventoryOrHotBar.SetDragAndDrop(_dragAndDrop);
            _inventoryUIR.InventoryOrHotBar.SetDragAndDrop(_dragAndDrop);
            _inventoryUIJ.InventoryOrHotBar.SetDragAndDrop(_dragAndDrop);
            _inventoryUIT.InventoryOrHotBar.SetDragAndDrop(_dragAndDrop);
        }

        private void LateUpdate()
        {
            _dragAndDrop.OnLateUpdate();

            // This assumes the animation is 1 second long.
            float animationTime = _animator.GetFloat("InventoryTime");
            animationTime += (_isInBackpackMode ? 1 : -1) * Time.unscaledDeltaTime / _animationDuration;
            animationTime = Mathf.Clamp01(animationTime);
            _animator.SetFloat("InventoryTime", animationTime);
        }

        private void OnEnable()
        {
            _backpackAction?.Enable();
        }

        private void OnDisable()
        {
            _backpackAction?.Disable();
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

            _firstPersonView.enabled = !becomeInBackpackMode;
            _interactableVisual.SetActive(!becomeInBackpackMode);

            PlayerInteraction.InventoryState(becomeInBackpackMode);

            if (becomeInBackpackMode)
            {
                _dragAndDrop.EnableInput();
                PlayerMovement.Instance.enabled = false;
            }
            else
            {
                _dragAndDrop.DisableInputAndStop();
                PlayerMovement.Instance.enabled = true;
            }
        }
    }

}
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
        private HotBarUI _hotBarUI;

        [Header("Inventory Components")]
        [SerializeField, Tooltip("adjust this to change where the inventory slots are centered")]
        private Vector2 _inventoryCenter;

        [SerializeField, Tooltip("Prefab of the inventory slot element")]
        private GameObject _inventorySlotObject;

        [SerializeField, Tooltip("how many rows,columns of inventory space should there be")]
        private Vector2Int _inventoryDimensions;

        [SerializeField, Tooltip("How far apart the inventory slots are from each other")]
        private Vector2 _inventorySpacing;

        [SerializeField, Tooltip("The UI game object for the resource menu")]
        private GameObject _resourceMenu;

        [SerializeField, Tooltip("The UI game object for the jelly item menu")]
        private GameObject _jellyItemMenu;

        [SerializeField, Tooltip("The UI game object for the tool menu")]
        private GameObject _toolMenu;

        [SerializeField, Tooltip("Controls open close animation of backpack panel")]
        private Animator _animator;

        [SerializeField, Tooltip("The UI game object for the backpack button")]
        private GameObject _backpackButton;

        [SerializeField, Tooltip("Add the GameObject representing an inventory section that has a grid layout group component (there are multiple, just use one for now)")]
        private GameObject _inventoryGridR;
        [SerializeField, Tooltip("Add the GameObject representing an inventory section that has a grid layout group component (there are multiple, just use one for now)")]
        private GameObject _inventoryGridJelly;
        [SerializeField, Tooltip("Add the GameObject representing an inventory section that has a grid layout group component (there are multiple, just use one for now)")]
        private GameObject _inventoryGridTool;

        [SerializeField]
        private Transform _itemParentDuringDragAndDrop;

        private InputAction _backpackAction;

        private bool _isInBackpackMode;

        [Header("Animation Control")]
        [SerializeField, Range(0.1f, 1f)]
        private float _animationSpeed = .25f;
        // Maybe have one InventoryOrHotBarUI for each inventory section, and rename it to InventorySection and the hotbar
        // would be considered an inventory section.
        private InventoryUIBase _inventoryUIR;
        private InventoryUIBase _inventoryUIJ;
        private InventoryUIBase _inventoryUIT;

        private InventoryDragAndDrop _dragAndDrop;


        private PauseManagement _pauseManager;

        //Issue to fix, able to start interaction when inventory is open. Cannot call this script since
        //the namespace UI.Inventory is nt being recognized as a using namespace. Need some way to notify jelly that
        //the backpack is currently open. Cause Time.timescale soft lock.

        private void Awake()
        {
            _pauseManager = PauseManagement.Instance;
            // need to do this in Awake b/c needs to happen before OnEnable (Start() happens after OnEnable()).
            // Maybe all the initialization can just be in Awake.
            InventoryBase inventoryR
                = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScene>().InventoryResource;
            _inventoryUIR = new InventoryUIBase(_inventorySlotObject, _inventoryGridR
                , inventoryR, _itemParentDuringDragAndDrop, ItemBase.ItemSortType.Resource);
            InventoryBase inventoryJ
                = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScene>().InventoryJelly;
            _inventoryUIJ = new InventoryUIBase(_inventorySlotObject, _inventoryGridJelly
                , inventoryJ, _itemParentDuringDragAndDrop, ItemBase.ItemSortType.JellyItem);
            InventoryBase inventoryT
                = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScene>().InventoryTool;
            _inventoryUIT = new InventoryUIBase(_inventorySlotObject, _inventoryGridTool
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

            _inventoryUIR.InventoryOrHotBar.SetDragAndDrop(_dragAndDrop);
            _inventoryUIJ.InventoryOrHotBar.SetDragAndDrop(_dragAndDrop);
            _inventoryUIT.InventoryOrHotBar.SetDragAndDrop(_dragAndDrop);
            _hotBarUI.SetDragAndDrop(_dragAndDrop);
        }

        private void LateUpdate()
        {
            _dragAndDrop.OnLateUpdate();

            float animatorCurVal = _animator.GetFloat("InventoryTime");
            if (_isInBackpackMode)
            {
                if (animatorCurVal < 1f)
                {
                    animatorCurVal += Time.fixedDeltaTime * _animationSpeed;
                    _animator.SetFloat("InventoryTime", animatorCurVal);
                }
            }
            else
            {
                if (animatorCurVal > 0f)
                {
                    animatorCurVal -= Time.fixedDeltaTime * _animationSpeed;
                    _animator.SetFloat("InventoryTime", animatorCurVal);
                }
            }
        }

        private void OnEnable()
        {
            _backpackAction?.Enable();
            _inventoryUIR.OnEnable();
            _inventoryUIJ.OnEnable();
            _inventoryUIT.OnEnable();
        }

        private void OnDisable()
        {
            _backpackAction?.Disable();
        }

        private void OnDestroy()
        {
            // maybe can just do this in OnDisable, but doing this here for consistency with HotBarUI.
            _inventoryUIR.OnDestroy();
            _inventoryUIJ.OnDestroy();
            _inventoryUIT.OnDestroy();
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
            else if(becomeInBackpackMode && _pauseManager.IsPaused())
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

            

            _pauseManager.InventoryInteraction(becomeInBackpackMode); // notify pause manager that inventory is open
            _isInBackpackMode = becomeInBackpackMode;
            Cursor.visible = becomeInBackpackMode;
            Cursor.lockState = becomeInBackpackMode ? CursorLockMode.None : CursorLockMode.Locked;

            _firstPersonView.enabled = !becomeInBackpackMode;
            _interactableVisual.SetActive(!becomeInBackpackMode);

            PlayerInteraction.InventoryState(becomeInBackpackMode);

            if (becomeInBackpackMode)
            {
                _dragAndDrop.EnableInput();
                Time.timeScale = 0;
            }
            else
            {
                _dragAndDrop.DisableInputAndStop();
                Time.timeScale = 1;
            }
        }
    }

    #region copy before removing stuff which might be being worked on 9/29/23

    //public class InventoryUI : MonoBehaviour
    //{
    //    [SerializeField, Tooltip("adjust this to change where the inventory slots are centered")]
    //    private Vector2 _inventoryCenter;

    //    [SerializeField, Tooltip("Prefab of the inventory slot element")]
    //    private GameObject _inventorySlotObject;

    //    [SerializeField, Tooltip("how many rows,columns of inventory space should there be")]
    //    private Vector2Int _inventoryDimensions;

    //    [SerializeField, Tooltip("How far apart the inventory slots are from each other")]
    //    private Vector2 _inventorySpacing;

    //    [SerializeField, Tooltip("The UI game object for the resource menu")]
    //    private GameObject _resourceMenu;

    //    [SerializeField, Tooltip("The UI game object for the jelly item menu")]
    //    private GameObject _jellyItemMenu;

    //    [SerializeField, Tooltip("The UI game object for the tool menu")]
    //    private GameObject _toolMenu;

    //    [SerializeField, Tooltip("The UI game object for the backpack button")]
    //    private GameObject _backpackButton;

    //    private InputAction _scrollAction;
    //    private InputAction _backpackAction;

    //    //private Canvas _canvas;
    //    private PlayerInput _playerInput;
    //    private Body _playerBody;

    //    private bool _isInventoryOpen;

    //    private void Start()
    //    {
    //        GameObject _player = GameObject.FindGameObjectWithTag("Player");
    //        if (_player != null)
    //        {
    //            _playerInput = _player.GetComponent<PlayerInput>();
    //            _playerBody = _player.GetComponent<Body>();
    //        }

    //        /*        _canvas = GameObject.FindGameObjectWithTag("UICanvas").GetComponent<Canvas>();
    //            if (_canvas == null)
    //            {
    //                Debug.LogWarning("Canvas is null");
    //            }*/

    //        _scrollAction = _playerInput.actions.FindAction("HotBarScroll", true);
    //        _backpackAction = _playerInput.actions.FindAction("Backpack", true);
    //        _scrollAction.performed += ctx => OnHotBarScroll(ctx);
    //        _backpackAction.performed += ctx => OnBackpack(ctx);
    //    }

    //    // Update is called once per frame
    //    private void Update()
    //    {

    //    }

    //    private void OnEnable()
    //    {
    //        _scrollAction?.Enable();
    //        _backpackAction?.Enable();
    //    }

    //    private void OnDisable()
    //    {
    //        _scrollAction?.Disable();
    //        _backpackAction?.Disable();
    //    }

    //    private void OnBackpack(InputAction.CallbackContext context)
    //    {
    //        Debug.Log("Pressing Inventory");
    //        if (!_isInventoryOpen)
    //        {
    //            //the player being able to open the inventory while moving causes a sliding bug
    //            if (_playerBody.IsMoving)
    //            {
    //                _isInventoryOpen = !_isInventoryOpen;


    //                //TODO:  Get this to work in player
    //                /* UpdateInventoryDisplay();

    //                GetComponent<Motion.RectilinearMovement>().enabled = false;
    //                GetComponent<View.FirstPersonView>().enabled = false;
    //                GetComponent<CharacterController>().enabled = false;
    //                GetComponent<Body>().enabled = false;*/
    //                Cursor.lockState = CursorLockMode.None;
    //                Cursor.visible = true;
    //            }
    //        }
    //        else
    //        {
    //            _isInventoryOpen = !_isInventoryOpen;

    //            /*            for (int i = 0; i < _inventorySlots.Length; i++)
    //                    {
    //                        for (int j = 0; j < _inventorySlots[i].Length; j++)
    //                        {
    //                            _inventorySlots[i][j].enabled = false;
    //                            _inventorySlots[i][j].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
    //                        }
    //                    }*/

    //            /*            GetComponent<Motion.RectilinearMovement>().enabled = true;
    //                    GetComponent<View.FirstPersonView>().enabled = true;
    //                    GetComponent<CharacterController>().enabled = true;
    //                    GetComponent<Body>().enabled = true;*/
    //            Cursor.lockState = CursorLockMode.Locked;
    //            Cursor.visible = false;
    //            //_heldIndex[0] = _heldIndex[1] = -1;
    //        }
    //    }

    //    private void OnHotBarScroll(InputAction.CallbackContext context)
    //    {

    //        if (!_isInventoryOpen)
    //        {
    //            float scrolling = context.ReadValue<float>();
    //            if (scrolling > 0)
    //            {
    //                //Debug.Log("Scrolling up");
    //                /*                _hotbarSelected++;
    //                            if (_hotbarSelected >= _theInventory[0].Length)
    //                            {
    //                                _hotbarSelected = 0;
    //                            }*/
    //            }
    //            else if (scrolling < 0)
    //            {
    //                //Debug.Log("Scrolling down");
    //                /*                _hotbarSelected--;
    //                            if (_hotbarSelected < 0)
    //                            {
    //                                _hotbarSelected = _theInventory[0].Length - 1;
    //                            }*/
    //            }
    //            //UpdateHotbar();

    //        }
    //    }

    //    //these ensure the display is reflective of what the data values actually are
    //    private void UpdateInventoryDisplay()
    //    {
    //        /*       for (int i = 0; i < _inventorySlots.Length; i++)
    //            {
    //                for (int j = 0; j < _inventorySlots[i].Length; j++)
    //                {
    //                    _inventorySlots[i][j].enabled = true;
    //                    _inventorySlots[i][j].sprite = _theInventory[i][j].ItemSprite;
    //                    _inventorySlots[i][j].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = _theInventory[i][j].StackAmount.ToString();
    //                    if (_inventorySlots[i][j].sprite == null)
    //                    {
    //                        _inventorySlots[i][j].sprite = _slotObject.GetComponent<Image>().sprite;
    //                        _inventorySlots[i][j].gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "";
    //                    }
    //                }
    //            }
    //            UpdateHotbar();*/
    //    }

    //    //first inventory slot pressed is rembemered, and the item therein is moved to the next slot pressed
    //    public void SlotPressed(int index0, int index1)
    //    {
    //        /*       if (_heldIndex[0] == -1)
    //            {
    //                if (_theInventory[index0][index1].ItemID != -1)
    //                {
    //                    _heldIndex[0] = index0;
    //                    _heldIndex[1] = index1;
    //                }
    //            }
    //            else
    //            {
    //                if (_theInventory[index0][index1].ItemID == -1)
    //                {
    //                    int amount = _theInventory[_heldIndex[0]][_heldIndex[1]].StackAmount;
    //                    _theInventory[index0][index1].FillSlot(
    //                        _theInventory[_heldIndex[0]][_heldIndex[1]].ItemID,
    //                        _inventoryImage,
    //                        _theInventory[_heldIndex[0]][_heldIndex[1]].ItemSprite,
    //                        amount);

    //                    _theInventory[_heldIndex[0]][_heldIndex[1]].FillSlot(_inventoryImage);
    //                    UpdateInventoryDisplay();
    //                    _heldIndex[0] = _heldIndex[1] = -1;
    //                }
    //                else //when the second selected slot also has an item, swap the two
    //                {
    //                    _heldSlot = _theInventory[index0][index1];
    //                    _theInventory[index0][index1] = _theInventory[_heldIndex[0]][_heldIndex[1]];
    //                    _theInventory[_heldIndex[0]][_heldIndex[1]] = _heldSlot;

    //                    _heldSlot.FillSlot(_inventoryImage);
    //                    UpdateInventoryDisplay();
    //                }
    //            }*/
    //    }
    //}

    #endregion

}
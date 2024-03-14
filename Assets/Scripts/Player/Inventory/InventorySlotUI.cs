using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI.Inventory
{
    /// <summary>
    /// UI for a single inventory slot. Shows an item, including while the mouse is dragging it to another slot.
    /// </summary>
    public class InventorySlotUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject _imageGameobject;
        [SerializeField]
        private GameObject _countTextGameobject;

        private Image _image;
        private TextMeshProUGUI _countText;
        private Transform _itemParentDuringDragAndDrop;

        private ItemBase.ItemSortType _slotType;

        private Vector3 _imagePosRelativeSlotPos;
        private Vector3 _textPosRelativeImagePos;

        public InventoryBaseUI InventoryOrHotBarUI { get; private set; }
        public bool IsNotEmpty => InventoryOrHotBarUI.IsNotEmpty(this);
        public ItemStack ItemStack => InventoryOrHotBarUI.GetItemInSlot(this);
        public int SlotIndex => InventoryOrHotBarUI.GetIndexOfSlot(this);

        public ItemBase.ItemSortType SlotType => _slotType;


        private void Awake()
        {
            _image = _imageGameobject.GetComponent<Image>();
            _countText = _countTextGameobject.GetComponent<TextMeshProUGUI>();

            _imagePosRelativeSlotPos = _imageGameobject.transform.position - transform.position;
            _textPosRelativeImagePos = _countTextGameobject.transform.position - _imageGameobject.transform.position;
        }

        /// <summary>
        /// Make the item in this slot follow the mouse.
        /// </summary>
        public void FollowMouse()
        {
            // Only move the gameobjects which show the item's image and count. Don't move everything
            // because then the item slot would be considered below the mouse while following it.

            _imageGameobject.transform.position = Input.mousePosition;
            _countTextGameobject.transform.position = Input.mousePosition + _textPosRelativeImagePos;
        }

        /// <summary>
        /// Initialization after instantiating the prefab.
        /// </summary>
        public void InitializeAfterInstantiate(InventoryBaseUI inventoryOrHotBarUI, Transform itemParentDuringDragAndDrop, ItemBase.ItemSortType type)
        {
            InventoryOrHotBarUI = inventoryOrHotBarUI;
            _itemParentDuringDragAndDrop = itemParentDuringDragAndDrop;
            _slotType = type;
            ShowItem(null);
        }

        /// <summary>
        /// Show an item. If the item is null, make this slot empty.
        /// </summary>
        public void ShowItem(ItemStack item)
        {
            bool show = item != null; 

            _countTextGameobject.SetActive(show);
            _imageGameobject.SetActive(show);
            if (show)
            {
                _image.sprite = item.itemInfo.Icon;
                _countText.text = item.amount.ToString();
            }
        }

        /// <summary>
        /// Start dragging the item in this slot.
        /// </summary>
        public void StartItemDrag()
        {
            _imageGameobject.transform.SetParent(_itemParentDuringDragAndDrop, true);
            _countTextGameobject.transform.SetParent(_itemParentDuringDragAndDrop, true);
        }

        /// <summary>
        /// Stop dragging the item in this slot.
        /// </summary>
        public void StopItemDrag()
        {
            _imageGameobject.transform.SetParent(transform.parent, true);
            _countTextGameobject.transform.SetParent(transform.parent, true);

            _imageGameobject.transform.position = transform.position + _imagePosRelativeSlotPos;
            _countTextGameobject.transform.position = _imageGameobject.transform.position + _textPosRelativeImagePos;
        }
    }
}
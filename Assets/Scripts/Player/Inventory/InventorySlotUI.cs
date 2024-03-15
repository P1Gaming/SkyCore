using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


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
    private InventoryDragAndDrop _dragAndDrop;


    private Vector3 _imagePosRelativeSlotPos;
    private Vector3 _textPosRelativeImagePos;


    public ItemStack _itemStack;

    public ItemIdentity.ItemSortType SortType => InventoryOrHotBarUI.SortType;
    public InventorySection InventoryOrHotBarUI { get; private set; }
    public bool IsNotEmpty => _itemStack != null;


    private void Awake()
    {
        _image = _imageGameobject.GetComponent<Image>();
        _countText = _countTextGameobject.GetComponent<TextMeshProUGUI>();

        _imagePosRelativeSlotPos = _imageGameobject.transform.position - transform.position;
        _textPosRelativeImagePos = _countTextGameobject.transform.position - _imageGameobject.transform.position;
    }

    public void InitializeAfterInstantiate(InventorySection inventoryOrHotBarUI
        , Transform itemParentDuringDragAndDrop, InventoryDragAndDrop dragAndDrop)
    {
        InventoryOrHotBarUI = inventoryOrHotBarUI;
        _itemParentDuringDragAndDrop = itemParentDuringDragAndDrop;
        _dragAndDrop = dragAndDrop;
        OnItemStackChanged();
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

    public void OnItemStackChanged()
    {
        if (_itemStack != null)
        {
            if (_itemStack.amount < 0)
            {
                throw new System.Exception("_itemStack.amount < 0, indicating a" +
                    " bug happening before this was called. _itemStack.amount: " + _itemStack.amount);
            }

            if (_itemStack.amount == 0)
            {
                _itemStack = null;
                _dragAndDrop.CheckDraggedStackNowEmpty();
            }
        }

        bool show = _itemStack != null; 
        _countTextGameobject.SetActive(show);
        _imageGameobject.SetActive(show);

        if (show)
        {
            _image.sprite = _itemStack.identity.Icon;
            _countText.text = _itemStack.amount.ToString();
        }
        else
        {
            StopItemDrag();
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

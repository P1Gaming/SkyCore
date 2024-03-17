using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


/// <summary>
/// UI for a single inventory slot. Shows an item, including while the mouse is dragging it to another slot.
/// </summary>
public class InventorySlot : MonoBehaviour
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

    public ItemIdentity.ItemSortType SortType { get; private set; }
    public bool IsNotEmpty => _itemStack != null;


    private void Awake()
    {
        _image = _imageGameobject.GetComponent<Image>();
        _countText = _countTextGameobject.GetComponent<TextMeshProUGUI>();

        _imagePosRelativeSlotPos = _imageGameobject.transform.position - transform.position;
        _textPosRelativeImagePos = _countTextGameobject.transform.position - _imageGameobject.transform.position;
    }

#if UNITY_EDITOR
    private void LateUpdate()
    {
        // Ensure the thing being shown matches _itemStack, to check for bugs.
        // Shouldn't just show stuff every frame in the final product, for performance especially garbage allocation.

        if (_itemStack == null)
        {
            if (_imageGameobject.activeSelf || _countTextGameobject.activeSelf)
            {
                throw new System.Exception("In InventorySlot, _itemStack is null but slot gameobject active " 
                    + _imageGameobject.activeSelf + " " + _countTextGameobject.activeSelf);
            }
        }
        else
        {
            if (_image.sprite != _itemStack.identity.Icon)
            {
                throw new System.Exception("In InventorySlot, _image.sprite != _itemStack.identity.Icon. Item stack identity: "
                    + _itemStack.identity.name + ", amount: " + _itemStack.amount);
            }
            if (_countText.text != "" + _itemStack.amount)
            {
                throw new System.Exception("In InventorySlot, _countText.text != _itemStack.amount. Item stack identity: "
                    + _itemStack.identity.name + ", amount: " + _itemStack.amount + ", amount shown: " + _countText.text);
            }
        }
    }
#endif

    public void InitializeAfterInstantiate(ItemIdentity.ItemSortType sortType
        , Transform itemParentDuringDragAndDrop, InventoryDragAndDrop dragAndDrop)
    {
        SortType = sortType;
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
        if (_itemStack != null && _itemStack.amount <= 0)
        {
            if (_itemStack.amount < 0)
            {
                throw new System.Exception("_itemStack.amount < 0, indicating a" +
                    " bug happening before this was called. _itemStack.amount: " + _itemStack.amount);
            }
            _itemStack = null;
        }

        bool show = _itemStack != null; 
        _countTextGameobject.SetActive(show);
        _imageGameobject.SetActive(show);

        if (show)
        {
            _image.sprite = _itemStack.identity.Icon;
            _countText.text = _itemStack.amount.ToString();
        }


        _dragAndDrop.CheckDraggedStackNowEmpty();
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
        _imageGameobject.transform.SetParent(transform.parent, true); // this script is on a child gameObject of the slot prefab
        _countTextGameobject.transform.SetParent(transform.parent, true);

        _imageGameobject.transform.position = transform.position + _imagePosRelativeSlotPos;
        _countTextGameobject.transform.position = _imageGameobject.transform.position + _textPosRelativeImagePos;
    }
}

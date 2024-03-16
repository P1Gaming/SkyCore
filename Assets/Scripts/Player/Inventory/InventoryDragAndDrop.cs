using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;


/// <summary>
/// Handles dragging items between inventory slots.
/// </summary>
public class InventoryDragAndDrop
{
    private const float GENERATOR_DROPPABLE_RADIUS_AROUND_ISLAND_HEARTS = 3f;

    private InputAction _click;
    private bool _stillHoveringOverSlotBeingDragged;
    private InventorySlot _beingDragged;

    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private PointerEventData _eventData;
    private EventSystem _eventSystem;

    private RectTransform[] _whereToConsiderMouseInsideInventory;

    public InventoryDragAndDrop(RectTransform[] whereToConsiderMouseInsideInventory)
    {
        _whereToConsiderMouseInsideInventory = whereToConsiderMouseInsideInventory;
        _click = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>().actions.FindAction("ClickForDragAndDrop", true);
        _click.started += OnClickStart;
        _click.canceled += OnClickCancel;
        _click.Disable();
    }

    public void EnableInput()
    {
        _click.Enable();
    }

    public void DisableInputAndStop()
    {
        if (_beingDragged != null)
        {
            TryMoveDraggedItemToSlot(_beingDragged, true);
        }
        _click.Disable();
    }

    private void OnClickStart(InputAction.CallbackContext context)
    {
        if (PauseManagement.IsPaused)
        {
            return;
        }

        // There are 2 ways to click and drag between slots:
        // 1. Press, release without moving the mouse outside the slot, then move the mouse to a different slot and press again.
        // 2. Press, move the mouse to a different slot, and release.
        if (_beingDragged == null)
        {
            TryBeginDrag();
        }
        else
        {
            TryFinishDrag();
        }
    }

    private void OnClickCancel(InputAction.CallbackContext context)
    {
        if (PauseManagement.IsPaused)
        {
            return;
        }

        if (_beingDragged != null)
        {
            CheckStoppedHoveringOverOriginalSlot();
            if (!_stillHoveringOverSlotBeingDragged && FindInventorySlotBelowMouse() != null)
            {
                TryFinishDrag();
            }
        }
    }

   

    public void OnLateUpdate()
    {
        if (_beingDragged != null)
        {
            if (PauseManagement.IsPaused)
            {
                TryMoveDraggedItemToSlot(_beingDragged, true);
            }
            else
            {
                _beingDragged.FollowMouse();
            }
        }

        CheckStoppedHoveringOverOriginalSlot();
    }

    private void CheckStoppedHoveringOverOriginalSlot()
    {
        if (_stillHoveringOverSlotBeingDragged && FindInventorySlotBelowMouse() != _beingDragged)
        {
            _stillHoveringOverSlotBeingDragged = false;
        }
    }

    



    private void TryBeginDrag()
    {
        InventorySlot slotBelowMouse = FindInventorySlotBelowMouse();
        if (slotBelowMouse != null && slotBelowMouse.IsNotEmpty)
        {
            _beingDragged = slotBelowMouse;
            _beingDragged.StartItemDrag();
            _stillHoveringOverSlotBeingDragged = true;
        }
    }

    private void TryFinishDrag()
    {
        if (_beingDragged._itemStack == null)
        {
            throw new System.Exception("_beingDragged._itemStack is null. This shouldn't be possible");
        }

        bool isOutsideInventory = true;
        for (int i = 0; i < _whereToConsiderMouseInsideInventory.Length; i++)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_whereToConsiderMouseInsideInventory[i], Input.mousePosition))
            {
                isOutsideInventory = false;
                break;
            }
        }

        if (isOutsideInventory)
        {
            FinishDragOutsideInventory();
        }
        else
        {
            InventorySlot slotBelowMouse = FindInventorySlotBelowMouse();
            if (slotBelowMouse != null 
                && !InventoryInfoGetter.UnmatchedSortTypes(_beingDragged._itemStack.identity.SortType, slotBelowMouse.SortType))
            {
                TryMoveDraggedItemToSlot(slotBelowMouse);
            }
        }
    }

    private void FinishDragOutsideInventory()
    {
        TryTossItem();
    }

    private void TryMoveDraggedItemToSlot(InventorySlot moveTo, bool throwIfFails = false)
    {
        if (moveTo == _beingDragged)
        {
            _beingDragged.StopItemDrag();
            _beingDragged = null;
            return;
        }

        InventorySlot wasBeingDragged = _beingDragged; // the next line can make _beingDragged null
        if (Inventory.TryMoveItemBetweenSlots(_beingDragged, moveTo))
        {
            wasBeingDragged.StopItemDrag();
            _beingDragged = null;
        }
        else if (throwIfFails)
        {
            throw new System.Exception("Couldn't move the dragged item to slot");
        }
    }



    public void CheckDraggedStackNowEmpty()
    {
        if (_beingDragged != null && _beingDragged._itemStack == null)
        {
            _beingDragged.StopItemDrag();
            _beingDragged = null;
        }
    }

    /// <summary>
    /// Toss an item if it is dragged out of the inventory.
    /// </summary>
    private void TryTossItem()
    {
        if (_beingDragged._itemStack.amount <= 0)
        {
            throw new System.Exception("item stack amount is <= 0 in TossItem. amount: " + _beingDragged._itemStack.amount);
        }

        // If the item is a generator and the closest island heart is too far away, return.
        if (_beingDragged._itemStack.identity is GeneratorItemIdentity)
        {
            GameObject[] IslandHeartPos = GameObject.FindGameObjectsWithTag("IslandHeart");
            float closestIslandHeart = Mathf.Infinity;

            foreach (GameObject IslandHeart in IslandHeartPos)
            {
                float distance = (IslandHeart.transform.position - GameObject.FindGameObjectWithTag("Player").transform.position).magnitude;
                if (distance < closestIslandHeart)
                {
                    closestIslandHeart = distance;
                }
            }

            if (closestIslandHeart > GENERATOR_DROPPABLE_RADIUS_AROUND_ISLAND_HEARTS)
            {
                return;
            }
        }

        //Spawn item in front of the player
        Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
        Vector3 playerFwd = GameObject.FindWithTag("Player").transform.forward;
        Vector3 updatedPos = new Vector3(playerPos.x, playerPos.y, playerPos.z) + playerFwd * 2f;
        GameObject itemPrefab = _beingDragged._itemStack.identity.ItemPrefab;
        Object.Instantiate(itemPrefab, updatedPos, GameObject.FindWithTag("Player").transform.rotation);

        //Decrement Item

        _beingDragged._itemStack.amount--;
        _beingDragged.OnItemStackChanged();
        
    }





    private InventorySlot FindInventorySlotBelowMouse()
    {
        // source: https://forum.unity.com/threads/how-to-detect-if-mouse-is-over-ui.1025533/
        // is there a better way to do this?

        if (_eventSystem != EventSystem.current)
        {
            _eventSystem = EventSystem.current;

            if (_eventSystem == null)
            {
                return null;
            }

            _eventData = new PointerEventData(_eventSystem);
        }

        _eventData.position = Input.mousePosition;

        _raycastResults.Clear();
        _eventSystem.RaycastAll(_eventData, _raycastResults);

        foreach (RaycastResult raycastResult in _raycastResults)
        {
            InventorySlot result = raycastResult.gameObject.GetComponent<InventorySlot>();
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }


}


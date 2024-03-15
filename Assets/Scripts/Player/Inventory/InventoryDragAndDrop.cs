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
    private InputAction _click;
    private bool _clickHappeningAndHoveringOverOriginalSlot;
    private InventorySlotUI _beingDragged;

    private List<RaycastResult> _raycastResults = new List<RaycastResult>();
    private PointerEventData _eventData;
    private EventSystem _eventSystem;

    public InventoryDragAndDrop()
    {

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerInput playerInput = player == null ? null : player.GetComponent<PlayerInput>();
        _click = playerInput == null ? null : playerInput.actions.FindAction("ClickForDragAndDrop", true);
        _click.started += OnClickStart;
        _click.canceled += OnClickCancel;
        _click.Disable();
    }

   





    public void CheckDraggedStackNowEmpty()
    {
        if (_beingDragged != null && _beingDragged._itemStack == null)
        {
            _beingDragged.StopItemDrag();
            _beingDragged = null;
        }
    }

    private void OnClickStart(InputAction.CallbackContext context)
    {
        OnClickStart(FindInventorySlotBelowMouse());
    }

    private void OnClickCancel(InputAction.CallbackContext context)
    {
        OnClickEnd(FindInventorySlotBelowMouse());
    }

    public void EnableInput()
    {
        _click?.Enable();
    }

    public void DisableInputAndStop()
    {
        CheckCancelDrag();
        _click?.Disable();
    }

    public void OnLateUpdate()
    {
        if (_beingDragged != null)
        {
            _beingDragged.FollowMouse();
        }

        // Keep track of whether the mouse is still hovering over the same slot after click started.
        if (_clickHappeningAndHoveringOverOriginalSlot)
        {
            InventorySlotUI belowMouse = FindInventorySlotBelowMouse();
            if (belowMouse != _beingDragged)
            {
                _clickHappeningAndHoveringOverOriginalSlot = false;
            }
        }
    }


        
        

        

    private void MoveDraggedItemToSlot(InventorySlotUI moveTo)
    {
        InventorySection.MoveItemBetweenSlots(_beingDragged, moveTo);
        _beingDragged.StopItemDrag();
        _beingDragged = null;
    }

    /// <summary>
    /// Updates drag and drop when a mouse click starts.
    /// </summary>
    public void OnClickStart(InventorySlotUI slotBelowMouse)
    {
        //Check if there is a slot, if not toss the item
        if (slotBelowMouse == null)
        {
            if (!_beingDragged || _beingDragged._itemStack == null)
            {
                return;
            }
            TossItem();
            return;
        }

            
        if (_beingDragged != null && (_beingDragged._itemStack.identity.SortType == slotBelowMouse.SortType || slotBelowMouse.SortType == ItemIdentity.ItemSortType.None))
        {
            // Clicking again after clicking to pick up the item, so the player
            // released the item without a slot below the mouse.
            MoveDraggedItemToSlot(slotBelowMouse);
        }
        else if (slotBelowMouse.IsNotEmpty)
        {
            _beingDragged = slotBelowMouse;
            _beingDragged.StartItemDrag();
            _clickHappeningAndHoveringOverOriginalSlot = true;
        }
    }

    /// <summary>
    /// Updates drag and drop when a mouse click ends.
    /// </summary>
    public void OnClickEnd(InventorySlotUI slotBelowMouse)
    {
        if (_beingDragged != null && slotBelowMouse != null)
        {
            // x: clicked a slot then released the mouse button without the cursor leaving that slot.
            // Without checking that, clicking (and not holding down) would immediately return the item
            // to the same inventory slot.
            bool x = _clickHappeningAndHoveringOverOriginalSlot && (slotBelowMouse == _beingDragged);
            if (!x)
            {
                if(_beingDragged._itemStack.identity.SortType != slotBelowMouse.SortType && slotBelowMouse.SortType != ItemIdentity.ItemSortType.None)
                {
                    return;
                }
                MoveDraggedItemToSlot(slotBelowMouse);
            }
        }
        //If there isn't a slot, toss the item.
        else
        {
            if (!_beingDragged || _beingDragged._itemStack == null)
            {
                return;
            }
            TossItem();
            return;
        }
    }

    /// <summary>
    /// If an item is being dragged, put it back in its original inventory slot.
    /// </summary>
    public void CheckCancelDrag()
    {
        if (_beingDragged != null)
        {
            MoveDraggedItemToSlot(_beingDragged);
        }
    }

    /// <summary>
    /// Toss an item if it is dragged out of the inventory.
    /// </summary>
    private void TossItem()
    {
        if (_beingDragged._itemStack.amount > 0)
        {
            GameObject droppedItem = _beingDragged._itemStack.identity.ItemPrefab;

            //Check what the item is

            //Is the Item a Generator?
            if (_beingDragged._itemStack.identity is GeneratorItemIdentity)
            {
                //If so check to see if the island heart is close by. If not just return and do nothing.
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

                if (closestIslandHeart > 3)
                {
                    return;
                }
            }

            //Spawn item in front of the player
            Vector3 playerPos = GameObject.FindWithTag("Player").transform.position;
            Vector3 playerFwd = GameObject.FindWithTag("Player").transform.forward;
            Vector3 updatedPos = new Vector3(playerPos.x, playerPos.y, playerPos.z) + playerFwd * 2f;
            UnityEngine.Object.Instantiate(droppedItem, updatedPos, GameObject.FindWithTag("Player").transform.rotation);

            //Decrement Item

            _beingDragged.InventoryOrHotBarUI.TrySubtractItemAmount(_beingDragged._itemStack.identity, 1);
        }
    }





    private InventorySlotUI FindInventorySlotBelowMouse()
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
            InventorySlotUI result = raycastResult.gameObject.GetComponent<InventorySlotUI>();
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }


}


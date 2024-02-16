using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Player;

namespace UI.Inventory
{
    /// <summary>
    /// Handles dragging items between inventory slots.
    /// </summary>
    public class InventoryDragAndDrop
    {
        private InventorySlotBelowMouseFinder _slotBelowMouseFinder = new InventorySlotBelowMouseFinder();
        private InputAction _click;
        private bool _clickHappeningAndHoveringOverOriginalSlot;
        private InventorySlotUI _beingDragged;

        public InventoryDragAndDrop()
        {

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerInput playerInput = player == null ? null : player.GetComponent<PlayerInput>();
            _click = playerInput == null ? null : playerInput.actions.FindAction("ClickForDragAndDrop", true);
            _click.started += OnClickStart;
            _click.canceled += OnClickCancel;
        }

        private void OnClickStart(InputAction.CallbackContext context)
        {
            OnClickStart(_slotBelowMouseFinder.FindInventorySlotBelowMouse());
        }

        private void OnClickCancel(InputAction.CallbackContext context)
        {
            OnClickEnd(_slotBelowMouseFinder.FindInventorySlotBelowMouse());
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

            if (_clickHappeningAndHoveringOverOriginalSlot)
            {
                InventorySlotUI belowMouse = _slotBelowMouseFinder.FindInventorySlotBelowMouse();
                if (belowMouse != _beingDragged)
                {
                    _clickHappeningAndHoveringOverOriginalSlot = false;
                }
            }
        }


        
        

        

        private void MoveDraggedItemToSlot(InventorySlotUI moveTo)
        {
            InventoryUIBase.MoveItemBetweenSlots(_beingDragged, moveTo);
            _beingDragged.StopItemDrag();
            _beingDragged = null;
        }

        /// <summary>
        /// Updates drag and drop when a mouse click starts.
        /// </summary>
        public void OnClickStart(InventorySlotUI slotBelowMouse)
        {
            if (slotBelowMouse == null)
            {
                return;
            }

            if (_beingDragged != null && (_beingDragged.ItemStack.itemInfo.SortType == slotBelowMouse.SlotType || slotBelowMouse.SlotType == ItemBase.ItemSortType.None))
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
                    if(_beingDragged.ItemStack.itemInfo.SortType != slotBelowMouse.SlotType && slotBelowMouse.SlotType != ItemBase.ItemSortType.None)
                    {
                        return;
                    }
                    MoveDraggedItemToSlot(slotBelowMouse);
                }
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

        


    } 
}

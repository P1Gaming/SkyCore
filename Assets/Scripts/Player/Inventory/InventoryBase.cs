using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Inventory;

namespace Player
{
    public class InventoryBase
    {
        private int _stacksCapacity;
        private ItemBase.ItemSortType _sortType;
        private InventoryBase[] _overflowTo;


        private HashSet<ItemStack> _items = new HashSet<ItemStack>(); // unordered. UI code stores positioning in slots.
        private InventoryDragAndDrop _dragAndDrop;

        private InventoryBaseUI _inventoryBaseUI;

        public int StacksCapacity => _stacksCapacity;

        public ItemBase.ItemSortType SortType => _sortType;

        public InventoryBase(int stacksCapacity, ItemBase.ItemSortType sortType)
        {
            _stacksCapacity = stacksCapacity;
            _sortType = sortType;
        }

        public void SetDragAndDrop(InventoryDragAndDrop dragAndDrop) => _dragAndDrop = dragAndDrop;
        public void SetInventoryBaseUI(InventoryBaseUI inventoryBaseUI) => _inventoryBaseUI = inventoryBaseUI;

        private void AddToStack(ItemStack item, int amount)
        {
            item.amount += amount;
            if (item.amount > item.itemInfo.MaxStack)
            {
                int overflowAmount = item.amount - item.itemInfo.MaxStack;
                item.amount -= overflowAmount;
                TryAddItemAsNewStack(new ItemStack(item.itemInfo, overflowAmount));
            }
            //Check if amount is zero or less, if so, remove it from the inventory
            if (item.amount <= 0)
            {
                _items.Remove(item);
                _dragAndDrop.CheckDraggedStackNowEmpty();
            }
            _inventoryBaseUI.OnChangeItem(item);
            HoldingItemHandler.Instance.UpdateHeldItem();
        }

        private bool TryAddItemAsNewStack(ItemStack item)
        {
            if (_items.Count < _stacksCapacity)
            {
                _items.Add(item);
                _inventoryBaseUI.OnChangeItem(item);
                return true;
            }
            else if (_overflowTo != null)
            {
                // Sorts to the proper inventory section
                foreach (InventoryBase overflowOption in _overflowTo)
                {
                    if (item.itemInfo.SortType == overflowOption.SortType || overflowOption.SortType == ItemBase.ItemSortType.None)
                    {
                        return overflowOption.TryAddItem(item);
                    }
                }

                throw new InvalidOperationException("Can't add item because type is not found");
            }
            else
            {
                Debug.Log("Can't add item because inventory is full (TODO: toss excess items on the ground)");
                return false;
            }
        }
       
        private ItemStack GetItem(ItemBase itemInfo, bool requireStackNotFull)
        {
            foreach (ItemStack item in _items)
            {
                if (item.itemInfo.ID == itemInfo.ID && (!requireStackNotFull || !item.IsStackFull))
                {
                    return item;
                }
            }
            return null;
        }

        public bool HasItem(ItemBase itemInfo)
        {
            foreach (ItemStack item in _items)
            {
                if (item.itemInfo.ID == itemInfo.ID)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryAddItem(ItemStack item)
        {
            bool successfullyAdded;
            ItemStack itemHereAlready = GetItem(item.itemInfo, requireStackNotFull: true);
            if (itemHereAlready == null || !item.itemInfo.IsStackable)
            {
                successfullyAdded = TryAddItemAsNewStack(item);
            }
            else
            {
                AddToStack(itemHereAlready, item.amount);
                successfullyAdded = true;
            }

            HoldingItemHandler.Instance.UpdateHeldItem();

            return successfullyAdded;
        }

        public bool HasRoomForItem(ItemStack item, bool includeOverflowInventories = false)
        {
            return HasRoomForItem(item.itemInfo, item.amount, includeOverflowInventories);
        }

        public bool HasRoomForItem(PickupItem item, bool includeOverflowInventories = false)
        {
            return HasRoomForItem(item.ItemInfo, item.Amount, includeOverflowInventories);
        }

        public bool HasRoomForItem(ItemBase itemInfo, float amount, bool includeOverflowInventories = false)
        {
            // First check if the items can be added to an existing stack.
            ItemStack itemHereAlready = GetItem(itemInfo, requireStackNotFull: true);
            if (itemHereAlready != null &&
                itemHereAlready.itemInfo.MaxStack - itemHereAlready.amount >= amount)
            {
                // The stack we found has enough room for the passed in item stack, so return true.
                return true;
            }

            // We couldn't fit the items in an existing stack, so see if we can add them as a new stack.
            if (_items.Count < _stacksCapacity)
            {
                // There is at least one empty slot in this inventory, so we know the passed in item stack can fit.
                return true;
            }

            // Since this inventory does not have enough room and if includeOverflowInventories is on
            // we'll check the overflow inventories.
            if (includeOverflowInventories)
            {
                foreach (InventoryBase otherInventory in _overflowTo)
                {
                    if (itemInfo.SortType == otherInventory.SortType &&
                        otherInventory.HasRoomForItem(itemInfo, amount, includeOverflowInventories))
                    {
                        return true;
                    }

                } // end foreach
            }


            return false;
        }

        public bool TrySubtractItemAmount(ItemBase itemInfo, int subtractedAmount)
        {
            ItemStack item = GetItem(itemInfo, requireStackNotFull: false);
            if (item == null || item.amount < subtractedAmount)
            {
                return false;
            }

            AddToStack(item, -subtractedAmount);

            return true;
        }

        public static void MoveItemBetweenInventorySections(InventoryBase from, InventoryBase to, ItemStack item)
        {
            from._items.Remove(item);
            to._items.Add(item);

            // I'm not sure whether to invoke the OnChangeItem event. That currently is just for InventoryOrHotBarUI,
            // and this is called by that so it's not necessary.
        }

        public void SetOverflowTo(InventoryBase[] overflowTo)
        {
            _overflowTo = overflowTo;
        }
    }
}
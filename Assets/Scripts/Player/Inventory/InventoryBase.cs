using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Handles the adding of items into the hotbar or inventory. Once full, items overflow
    /// into another instance of this class, or onto the ground.
    /// </summary>
    [Serializable] // to show this in the inspector
    public class InventoryBase
    {
        [SerializeField, Tooltip("Max number of stacks (so the number of inventory slots).")]
        private int _stacksCapacityResource;
        private InventoryBase[] _overflowTo;

        private HashSet<ItemStack> _items = new HashSet<ItemStack>(); // unordered. UI code stores positioning in slots.

        [SerializeField, Tooltip("Designated sort type of inventory section.")]
        private ItemBase.ItemSortType _sortType;

        public event Action<ItemStack> OnChangeItem;

        public int StacksCapacityResource => _stacksCapacityResource;

        public ItemBase.ItemSortType SortType => _sortType;

        /// <summary>
        /// Creates a new item stack if the items picked up puts the amount over the max for the next open stack.
        /// </summary>
        /// <param name="item">The stack that has not reached the max items yet</param>
        /// <param name="amount">The number of items to add to the stack</param>
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
            }
            OnChangeItem?.Invoke(item);
        }

        /// <summary>
        /// Adds the new item to the hot bar unless the hot bar is full it will try to add the item
        /// to the inventory.
        /// </summary>
        /// <param name="item">The item that will be added to the hotbar</param>
        private bool TryAddItemAsNewStack(ItemStack item)
        {
            if (_items.Count < _stacksCapacityResource)
            {
                _items.Add(item);
                OnChangeItem?.Invoke(item);
                return true;
            }
            else if (_overflowTo != null)
            {
                //Sorts to the proper inventory section
                foreach(InventoryBase _o in _overflowTo)
                {
                    if(item.itemInfo.SortType == _o.SortType || _o.SortType == ItemBase.ItemSortType.None)
                    {
                        if(_o.TryAddItem(item))
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        
                    }
                }
                Debug.Log("Can't add item because type is not found");
                return false;
            }
            else
            {
                //TODO: Drop the excess items
                Debug.Log("Can't add item because inventory is full");
                return false;
            }
        }

        /// <summary>
        /// Finds an Item (which means a stack of items) in this inventory or hot bar.
        /// </summary>
        /// <param name="itemInfo">The type of item to find a stack of</param>
        /// <param name="requireStackNotFull">Whether to only find a stack which isn't full</param>
        /// <returns>Returns the stack of items, if not found returns null</returns>
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

        /// <summary>
        /// Try to add the picked up item to the inventory of any gameobject with an inventory (Usually the player)
        /// </summary>
        /// <param name="item">The item component that was on the collided item pickup</param>
        /// <returns> Returns true if the item can be added, otherwise false.</returns>
        public bool TryAddItem(ItemStack item)
        {
            bool successfullyAdded = false;
            ItemStack itemHereAlready = GetItem(item.itemInfo, requireStackNotFull: true);
            if ((itemHereAlready is null) || !item.itemInfo.IsStackable)
            {
                successfullyAdded = TryAddItemAsNewStack(item);
            }
            else
            {
                AddToStack(itemHereAlready, item.amount);
                successfullyAdded = true;
            }

            PrintHotbar();
            return successfullyAdded;
        }

        /// <summary>
        /// Check if the item exists in the HotBar & the item's amount >= subtractedAmount
        /// </summary>
        /// <param name="itemInfo"></param>
        /// <param name="subtractedAmount"></param>
        /// <returns>return the possibility to substract</returns>
        public bool TrySubtractItemAmount(ItemBase itemInfo, int subtractedAmount)
        {
            ItemStack item = GetItem(itemInfo, requireStackNotFull: false);
            if ((item is null) || item.amount < subtractedAmount)
            {
                return false;
            }

            AddToStack(item, -subtractedAmount);
            //PrintHotbar();

            return true;
        }

        /// <summary>
        /// Transfers an item between two InventoryOrHotBars.
        /// </summary>
        public static void MoveItemBetweenInventorySections(InventoryBase from, InventoryBase to, ItemStack item)
        {
            from._items.Remove(item);
            to._items.Add(item);

            // I'm not sure whether to invoke the OnChangeItem event. That currently is just for InventoryOrHotBarUI,
            // and this is called by that so it's not necessary.
        }

        /// <summary>
        /// For printing out the hotbar list. Used for debugging before UI was setup.
        /// </summary>
        public void PrintHotbar()
        {
            string toLog = "Hotbar:\n";
            foreach (ItemStack item in _items)
            {
                toLog += item + "\n";
            }
            Debug.Log(toLog);
        }

        /// <param name="overflowTo">When full and try add item, goes here, or on ground if null.</param>
        public void SetOverflowTo(InventoryBase[] overflowTo)
        {
            _overflowTo = overflowTo;
        }
    }
}
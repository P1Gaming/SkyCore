using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using System.Diagnostics;
using static UnityEditor.Progress;


namespace UI.Inventory
{
    /// <summary>
    /// Organizes items into inventory slots, and handles showing their info.
    /// </summary>
    public class InventoryUIBase
    {

        private InventorySlotUI[] _slotUIsResource;

        private Dictionary<ItemStack, int> _itemsInSlotIndexes = new Dictionary<ItemStack, int>();

        public InventoryBase InventoryOrHotBar { get; private set; }

        private ItemBase.ItemSortType _typeAllowed;

        public InventoryUIBase(GameObject inventorySlotPrefab, GameObject inventoryOrHotBarGrid
            , InventoryBase inventoryOrHotBar, Transform itemParentDuringDragAndDrop, ItemBase.ItemSortType type)
        {
            InventoryOrHotBar = inventoryOrHotBar;

            _slotUIsResource = new InventorySlotUI[inventoryOrHotBar.StacksCapacityResource];

            _typeAllowed = type;
            for (int i = 0; i < _slotUIsResource.Length; i++)
            {
                GameObject slot = Object.Instantiate(inventorySlotPrefab, inventoryOrHotBarGrid.transform);
                _slotUIsResource[i] = slot.GetComponentInChildren<InventorySlotUI>();
                _slotUIsResource[i].InitializeAfterInstantiate(this, itemParentDuringDragAndDrop, _typeAllowed);
            }
        }

        public void OnEnable()
        {
            InventoryOrHotBar.OnChangeItem += OnChangeItem;
        }

        public void OnDestroy()
        {
            InventoryOrHotBar.OnChangeItem -= OnChangeItem;
        }

        /// <summary>
        /// Updates the UI to show the item.
        /// </summary>
        private void OnChangeItem(ItemStack item)
        {
            int index;
            if (!_itemsInSlotIndexes.TryGetValue(item, out index))
            {
                index = GetEmptySlotWithLowestIndex();
                _itemsInSlotIndexes.Add(item, index);
            }
            if (item.amount <= 0)
            {
                _itemsInSlotIndexes.Remove(item);
                _slotUIsResource[index].ShowItem(null);
            }
            else
            {
                _slotUIsResource[index].ShowItem(item);
            }
        }

        private int GetEmptySlotWithLowestIndex()
        {
            for (int i = 0; i < _slotUIsResource.Length; i++)
            {
                if (!_itemsInSlotIndexes.ContainsValue(i))
                {
                    return i;
                }
            }
            //Debug.LogError("Couldn't find a free slot in UI to show the item. This is a bug.");
            return -1; // shouldn't happen
        }

        public int GetIndexOfSlot(InventorySlotUI slot)
        {
            for (int i = 0; i < _slotUIsResource.Length; i++)
            {
                if (slot == _slotUIsResource[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public bool IsNotEmpty(InventorySlotUI slot)
        {
            int index = GetIndexOfSlot(slot);
            return _itemsInSlotIndexes.ContainsValue(index);
        }

        public ItemStack GetItemInSlot(InventorySlotUI slot)
        {
            int index = GetIndexOfSlot(slot);

            foreach (ItemStack item in _itemsInSlotIndexes.Keys)
            {
                if (_itemsInSlotIndexes[item] == index)
                {
                    return item;
                }
            }
            return null;
        }

        public static void MoveItemBetweenSlots(InventorySlotUI from, InventorySlotUI to)
        {
            if (from == to)
            {
                return;
            }

            if(from.SlotType != to.SlotType && from.SlotType != ItemBase.ItemSortType.None && to.SlotType != ItemBase.ItemSortType.None)
            {
                return;
            }


            InventoryUIBase fromUI = from.InventoryOrHotBarUI;
            InventoryUIBase toUI = to.InventoryOrHotBarUI;

            Dictionary<ItemStack, int> fromItemsInSlotIndexes = fromUI._itemsInSlotIndexes;
            Dictionary<ItemStack, int> toItemsInSlotIndexes = toUI._itemsInSlotIndexes;

            ItemStack itemToMove = from.ItemStack;
            ItemStack itemInTo = to.ItemStack; // can be null, if drag and drop an item into an empty slot.

            if (itemToMove.itemInfo.SortType != to.SlotType && to.SlotType != ItemBase.ItemSortType.None)
            {
                return;
            }
            else if (itemInTo != null && itemInTo.itemInfo.SortType != from.SlotType && from.SlotType != ItemBase.ItemSortType.None)
            {
                return;
            }


            int fromSlotIndex = from.SlotIndex;
            int toSlotIndex = to.SlotIndex;

            fromItemsInSlotIndexes.Remove(itemToMove);
            if (!(itemInTo is null))
            {
                toItemsInSlotIndexes.Remove(itemInTo);
            }

            toItemsInSlotIndexes.Add(itemToMove, toSlotIndex);
            if (!(itemInTo is null))
            {
                fromItemsInSlotIndexes.Add(itemInTo, fromSlotIndex);
            }

            if (fromUI != toUI)
            {
                // Move the item between the sets of items for the different inventory sections.
                // If two items are being swapped, move both.
                InventoryBase.MoveItemBetweenInventorySections(fromUI.InventoryOrHotBar
                    , toUI.InventoryOrHotBar, itemToMove);
                if (itemInTo != null)
                {
                    InventoryBase.MoveItemBetweenInventorySections(toUI.InventoryOrHotBar
                    , fromUI.InventoryOrHotBar, itemInTo);
                }
            }

            from.ShowItem(from.ItemStack);
            to.ShowItem(to.ItemStack);

            GameObject.FindGameObjectWithTag("Player").GetComponent<HoldingItemHandler>().UpdateHeldItem();
        }

        public override string ToString()
        {
            List<(ItemStack, int)> itemInSlotIndexesAsList = new();
            foreach (ItemStack item in _itemsInSlotIndexes.Keys)
            {
                itemInSlotIndexesAsList.Add((item, _itemsInSlotIndexes[item]));
            }
            itemInSlotIndexesAsList.Sort((a, b) => a.Item2.CompareTo(b.Item2));

            string result = $"InventoryOrHotBarUI instance ({_slotUIsResource.Length} slots): ";
            for (int i = 0; i < itemInSlotIndexesAsList.Count; i++)
            {
                (ItemStack item, int index) = itemInSlotIndexesAsList[i];
                result += $"[[{index}] {item}] ";
            }

            return result;
        }

        /// <summary>
        /// Get what an item is relative to an index.
        /// </summary>
        public ItemStack GetItemIndex(int index)
        {

            foreach (ItemStack item in _itemsInSlotIndexes.Keys)
            {
                if (index == _itemsInSlotIndexes[item])
                {
                    return item;
                }
            }
            return null;
        }
    }
}
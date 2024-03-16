using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySection
{
    private InventorySlot[] _slots;

    public ItemIdentity.ItemSortType SortType { get; private set; }
    public int StacksCapacity => _slots.Length;


    public InventorySection(int stacksCapacity, ItemIdentity.ItemSortType sortType
        , GameObject inventorySlotPrefab, Transform slotsParent
        , Transform itemParentDuringDragAndDrop, InventoryDragAndDrop dragAndDrop)
    {
        SortType = sortType;
        _slots = new InventorySlot[stacksCapacity];

        for (int i = 0; i < _slots.Length; i++)
        {
            GameObject slot = Object.Instantiate(inventorySlotPrefab, slotsParent);
            _slots[i] = slot.GetComponentInChildren<InventorySlot>();
            _slots[i].InitializeAfterInstantiate(sortType, itemParentDuringDragAndDrop, dragAndDrop);
        }
    }

    public bool HasItem(ItemIdentity itemInfo) => InventoryInfoGetter.HasItem(itemInfo, _slots);

    public ItemStack GetItemAtSlotIndex(int index) => _slots[index]._itemStack;

    public int CountTotalAmount(ItemIdentity identity) => InventoryInfoGetter.CountTotalAmount(identity, _slots);

    public int CountCanAdd(ItemIdentity identity) => InventoryInfoGetter.CountCanAdd(identity, SortType, _slots);


    public void TakeIntoExistingStacks(ItemStack takeFrom)
    {
        if (!InventoryInfoGetter.UnmatchedSortTypes(SortType, takeFrom.identity.SortType))
        {
            while (takeFrom.amount > 0)
            {
                int index = InventoryInfoGetter.IndexOfFirstIncompleteStack(takeFrom.identity, _slots);
                if (index == -1)
                {
                    break; // There's no existing stack which isn't full.
                }
                _slots[index]._itemStack.StealAsManyAsCan(takeFrom);
                _slots[index].OnItemStackChanged();
            }
        }
    }

    public void TakeIntoNewStacks(ItemStack takeFrom)
    {
        if (!InventoryInfoGetter.UnmatchedSortTypes(SortType, takeFrom.identity.SortType))
        {
            while (takeFrom.amount > 0)
            {
                int index = InventoryInfoGetter.IndexOfFirstEmptySlot(_slots);
                if (index == -1)
                {
                    break; // There's no empty slot so this inventory section is full.
                }
                _slots[index]._itemStack = takeFrom.GiveAsManyAsCanAsNewStack();
                _slots[index].OnItemStackChanged();
            }
        }
    }

    public void SubtractAmount(ItemIdentity itemIdentity, ref int amountLeftToSubtract)
    {
        // Subtract from later stacks first
        int index = _slots.Length - 1;
        for (; index >= 0 && amountLeftToSubtract > 0; index--)
        {
            ItemStack inSlot = _slots[index]._itemStack;
            if (inSlot != null && inSlot.identity == itemIdentity)
            {
                int numberToSubtract = System.Math.Min(amountLeftToSubtract, inSlot.amount);
                amountLeftToSubtract -= numberToSubtract;
                inSlot.amount -= numberToSubtract;
            }
        }
    }
}

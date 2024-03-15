using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySection
{
    private InventorySlotUI[] _slotUIs;
    private InventorySection[] _overflowTo; // If this is hotbar, this is the other 3. Otherwise it's null.

    public ItemIdentity.ItemSortType SortType { get; private set; }
    public int StacksCapacity => _slotUIs.Length;


    public InventorySection(int stacksCapacity, ItemIdentity.ItemSortType sortType
        , GameObject inventorySlotPrefab, GameObject inventoryOrHotBarGrid
        , Transform itemParentDuringDragAndDrop, InventoryDragAndDrop dragAndDrop
        , InventorySection[] overflowTo)
    {
        SortType = sortType;
        _slotUIs = new InventorySlotUI[stacksCapacity];
        _overflowTo = overflowTo;

        for (int i = 0; i < _slotUIs.Length; i++)
        {
            GameObject slot = Object.Instantiate(inventorySlotPrefab, inventoryOrHotBarGrid.transform);
            _slotUIs[i] = slot.GetComponentInChildren<InventorySlotUI>();
            _slotUIs[i].InitializeAfterInstantiate(this, itemParentDuringDragAndDrop, dragAndDrop);
        }
    }


    public void TakeInAsManyAsFit(ItemStack item)
    {
        TakeIntoExistingStacks(item);
        TakeIntoNewStacks(item);
        HoldingItemHandler.Instance.UpdateHeldItem();
    }

    private void TakeIntoExistingStacks(ItemStack takeFrom)
    {
        if (!UnmatchedSortTypes(SortType, takeFrom.identity.SortType))
        {
            // Add to existing non-full stacks
            while (takeFrom.amount > 0)
            {
                // Find the first existing stack which isn't full
                int index = 0;
                for (; index < _slotUIs.Length; index++)
                {
                    ItemStack inSlot = _slotUIs[index]._itemStack;
                    if (inSlot != null && inSlot.identity == takeFrom.identity && !inSlot.IsStackFull)
                    {
                        break;
                    }
                }
                if (index == _slotUIs.Length)
                {
                    // There's no existing stack which isn't full.
                    break;
                }
                ItemStack existingStack = _slotUIs[index]._itemStack;


                int amountLeftBeforeFull = takeFrom.identity.MaxStack - existingStack.amount;
                int amountToTake = System.Math.Min(amountLeftBeforeFull, takeFrom.amount);
                existingStack.amount += amountToTake;
                takeFrom.amount -= amountToTake;
                _slotUIs[index].OnItemStackChanged();
            }
        }

        // Repeat for any it overflows to.
        if (_overflowTo != null)
        {
            for (int i = 0; i < _overflowTo.Length; i++)
            {
                _overflowTo[i].TakeIntoExistingStacks(takeFrom);
            }
        }
    }

    private void TakeIntoNewStacks(ItemStack item)
    {
        if (!UnmatchedSortTypes(SortType, item.identity.SortType))
        {
            while (item.amount > 0)
            {
                // Find the first empty slot
                int index = 0;
                for (; index < _slotUIs.Length; index++)
                {
                    if (_slotUIs[index]._itemStack == null)
                    {
                        break;
                    }
                }
                if (index == _slotUIs.Length)
                {
                    // There's no empty slot so this inventory section is full.
                    break;
                }

                // Add a new stack.
                int amountToTake = System.Math.Min(item.amount, item.identity.MaxStack);
                item.amount -= amountToTake;
                _slotUIs[index]._itemStack = new ItemStack(item.identity, amountToTake);
                _slotUIs[index].OnItemStackChanged();
            }
        }

        // Repeat for any it overflows to.
        if (_overflowTo != null)
        {
            for (int i = 0; i < _overflowTo.Length; i++)
            {
                _overflowTo[i].TakeIntoNewStacks(item);
            }
        }
    }

    public static bool UnmatchedSortTypes(ItemIdentity.ItemSortType a, ItemIdentity.ItemSortType b)
    {
        return a != b && a != ItemIdentity.ItemSortType.None && b != ItemIdentity.ItemSortType.None;
    }

    public bool TrySubtractItemAmount(ItemIdentity itemIdentity, int amountToSubtract)
    {
        // Ensure there's enough in inventory
        int count = CountInThis(itemIdentity);
        if (_overflowTo != null)
        {
            for (int i = 0; i < _overflowTo.Length; i++)
            {
                count += _overflowTo[i].CountInThis(itemIdentity);
            }
        }
        if (count < amountToSubtract)
        {
            return false;
        }

        Subtract(itemIdentity, ref amountToSubtract);

        if (amountToSubtract != 0)
        {
            throw new System.Exception("In theory, amountToSubtract should be 0 here, but it's " + amountToSubtract);
        }

        return true;
    }

    private int CountInThis(ItemIdentity itemIdentity)
    {
        int result = 0;
        for (int i = 0; i < _slotUIs.Length; i++)
        {
            ItemStack inSlot = _slotUIs[i]._itemStack;
            if (inSlot != null && inSlot.identity == itemIdentity)
            {
                result += inSlot.amount;
            }
        }
        return result;
    }

    private void Subtract(ItemIdentity itemIdentity, ref int amountLeftToSubtract)
    {
        // Subtract from later stacks first
        int index = _slotUIs.Length - 1;
        for (; index >= 0 && amountLeftToSubtract > 0; index--)
        {
            ItemStack inSlot = _slotUIs[index]._itemStack;
            if (inSlot != null && inSlot.identity == itemIdentity)
            {
                int numberToSubtract = System.Math.Min(amountLeftToSubtract, inSlot.amount);
                amountLeftToSubtract -= numberToSubtract;
                inSlot.amount -= numberToSubtract;
            }
        }

        // Repeat for overflow sections
        if (_overflowTo != null)
        {
            for (int i = 0; i < _overflowTo.Length; i++)
            {
                _overflowTo[i].Subtract(itemIdentity, ref amountLeftToSubtract);
            }
        }
    }






    

    public bool HasRoomForItem(ItemIdentity itemInfo, float amount)
    {
        int countCanAdd = 0;
        CountCanAdd(itemInfo, ref countCanAdd);
        return countCanAdd < amount;
    }

    private void CountCanAdd(ItemIdentity identity, ref int result)
    {
        if (!UnmatchedSortTypes(SortType, identity.SortType)) 
        {
            for (int i = 0; i < _slotUIs.Length; i++)
            {
                ItemStack inSlot = _slotUIs[i]._itemStack;
                if (inSlot == null)
                {
                    result += identity.MaxStack;
                }
                else if (inSlot.identity == identity)
                {
                    result += identity.MaxStack - inSlot.amount;
                }
            }
        }

        if (_overflowTo != null)
        {
            for (int i = 0; i < _overflowTo.Length; i++)
            {
                CountCanAdd(identity, ref result);
            }
        }
    }





    

    

    public bool HasItem(ItemIdentity itemInfo)
    {
        for (int i = 0; i < _slotUIs.Length; i++)
        {
            ItemStack inSlot = _slotUIs[i]._itemStack;
            if (inSlot != null && inSlot.identity == itemInfo)
            {
                return true;
            }
        }
        return false;
    }


    public ItemStack GetItemAtSlotIndex(int index)
    {
        return _slotUIs[index]._itemStack;
    }




    public static void MoveItemBetweenSlots(InventorySlotUI from, InventorySlotUI to)
    {
        if (from == to)
        {
            return;
        }

        if (UnmatchedSortTypes(from.SortType, to.SortType))
        {
            return;
        }

        // Swap their stacks
        ItemStack temp = from._itemStack;
        from._itemStack = to._itemStack;
        to._itemStack = temp;
        from.OnItemStackChanged();
        to.OnItemStackChanged();

        HoldingItemHandler.Instance.UpdateHeldItem();
    }

}

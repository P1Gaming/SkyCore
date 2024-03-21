using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Split things out from InventorySection which don't mutate state, to make that script easier to understand

public static class InventoryInfoGetter
{
    public static bool UnmatchedSortTypes(ItemIdentity.ItemSortType a, ItemIdentity.ItemSortType b)
    {
        return a != b && a != ItemIdentity.ItemSortType.None && b != ItemIdentity.ItemSortType.None;
    }

    public static int IndexOfFirstIncompleteStack(ItemIdentity identity, InventorySlot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            ItemStack inSlot = slots[i]._itemStack;
            if (inSlot != null && inSlot.identity == identity && !inSlot.IsStackFull)
            {
                return i;
            }
        }
        return -1;
    }

    public static int IndexOfFirstEmptySlot(InventorySlot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i]._itemStack == null)
            {
                return i;
            }
        }
        return -1;
    }

    public static bool HasItem(ItemIdentity itemInfo, InventorySlot[] slots)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            ItemStack inSlot = slots[i]._itemStack;
            if (inSlot != null && inSlot.identity == itemInfo)
            {
                return true;
            }
        }
        return false;
    }

    public static int CountTotalAmount(ItemIdentity identity, InventorySlot[] slots)
    {
        int result = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            ItemStack inSlot = slots[i]._itemStack;
            if (inSlot != null && inSlot.identity == identity)
            {
                result += inSlot.amount;
            }
        }
        return result;
    }

    public static int CountCanAdd(ItemIdentity identity, ItemIdentity.ItemSortType sortType, InventorySlot[] slots)
    {
        if (UnmatchedSortTypes(sortType, identity.SortType))
        {
            return 0;
        }
        
        int result = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            ItemStack inSlot = slots[i]._itemStack;
            if (inSlot == null)
            {
                result += identity.MaxStack;
            }
            else if (inSlot.identity == identity)
            {
                result += identity.MaxStack - inSlot.amount;
            }
        }
        return result;
    }
}

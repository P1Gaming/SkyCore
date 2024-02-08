using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a stack of items.
/// </summary>
public class ItemStack
{
    public readonly ItemBase itemInfo;
    public int amount;

    public bool IsStackFull => amount >= itemInfo.MaxStack;

    public ItemStack(ItemBase itemInfo, int amount)
    {
        if (itemInfo == null)
        {
            throw new System.ArgumentNullException("itemInfo");
        }
        if (amount > itemInfo.MaxStack)
        {
            throw new System.ArgumentException("ItemStack constructor's" +
                $" amount ({amount}) > itemInfo.MaxStack ({itemInfo.MaxStack}) (can't fit that many items in a single stack.)");
        }
        this.amount = amount;
        this.itemInfo = itemInfo;
    }

    public override string ToString()
    {
        return "ID: " + itemInfo.ID + " Name: " + itemInfo.Name + " Amount: " + amount;
    }
}

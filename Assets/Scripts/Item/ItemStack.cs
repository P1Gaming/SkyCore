using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a stack of items.
/// </summary>
public class ItemStack
{
    public readonly ItemIdentity identity;
    public int amount;

    public bool IsStackFull => amount >= identity.MaxStack;

    public ItemStack(ItemIdentity itemInfo, int amount)
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
        identity = itemInfo;
    }

    public override string ToString()
    {
        return "Name: " + identity.name + " Amount: " + amount;
    }
}

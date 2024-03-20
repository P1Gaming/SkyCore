using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To eliminate garbage allocation, maybe add a pool to this and make the constructor private.

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

    public ItemStack GiveAsManyAsCanAsNewStack()
    {
        // If this stack isn't overfull (so amount <= identity.MaxStack), this just makes a copy and sets amount to 0.
        int amountToTake = System.Math.Min(amount, identity.MaxStack);
        amount -= amountToTake;
        return new ItemStack(identity, amountToTake);
    }

    public void StealAsManyAsCan(ItemStack other)
    {
        int amountBeforeFull = identity.MaxStack - amount;
        int amountToSteal = System.Math.Min(amountBeforeFull, other.amount);
        amount += amountToSteal;
        other.amount -= amountToSteal;
    }

    public override string ToString()
    {
        return "Name: " + identity.name + " Amount: " + amount;
    }
}

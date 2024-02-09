using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object for an item jelly item type. Jelly item items should be created from this through the
/// asset menu.
/// </summary>
[CreateAssetMenu(fileName = "ItemJellyItem", menuName = "Item/ItemJellyItem")]
public class ItemJellyItem : ItemBase
{
    [SerializeField] private JellyItemType _jellyItemType;

    public enum JellyItemType
    {
        None,
        Berry
        
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object for an item resource type. Resource items should be created from this through the
/// asset menu.
/// </summary>
[CreateAssetMenu(fileName = "ItemResource", menuName = "Item/ItemResource")]
public class ItemResource : ItemBase
{
    [SerializeField] private bool _isPlaceable;
    [SerializeField] private ResourceType _resourceType;

    public enum ResourceType
    {
        None,
        GrassBlock,
        WoodBlock
    }

}

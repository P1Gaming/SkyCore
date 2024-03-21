using System.Collections.Generic;
using UnityEngine;

// Create a subclass of this for each item, even if it'll be an empty subclass. That way we can add to it without
// needing to re-assign a bunch of references, and check if an item is a particular identity with the "is" keyword.
public abstract class ItemIdentity : ScriptableObject
{
    public enum ItemSortType
    {
        None,
        Tool,
        JellyItem,
        Resource
    }

    [field: SerializeField] public Sprite Icon { get; private set; }
    [field: SerializeField] public GameObject ItemPrefab { get; private set; }
    [field: SerializeField] public int MaxStack { get; private set; }
    [field: SerializeField] public ItemSortType SortType { get; private set; }

    [field: Tooltip("This sets how close the player has to get before the item is pulled toward him. Set this value to 0 to make the item be unaffected by the player or any other object that has an ItemAttractor script on it.")]
    [field: Range(0f, ItemAttractor.MAX_ATTRACTION_RADIUS)]
    [field: SerializeField] public float AttractionRadius { get; private set; } // How close the player must get before the item is pulled to the player.





}

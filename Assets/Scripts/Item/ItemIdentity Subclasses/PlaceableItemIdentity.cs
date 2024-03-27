using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlaceableItemIdentity", menuName = "ItemIdentity/PlaceableItemIdentity")]
public class PlaceableItemIdentity : ItemIdentity
{
    [field: SerializeField, Tooltip("Can the item be placed in the world?")]
    public bool CanBePlaced { get; private set; }

    [field: SerializeField, Tooltip("What does the block look like?")]
    public GameObject BlockRef { get; private set; }
}

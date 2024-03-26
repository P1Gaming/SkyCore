using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "BerryItemIdentity", menuName = "ItemIdentity/BerryItemIdentity")]
public class BerryItemIdentity : ItemIdentity
{
    [field: SerializeField, Tooltip("Saturation value to be added to jelly after feeding")]
    public int SaturationValue { get; private set; }
}

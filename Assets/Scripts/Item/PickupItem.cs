using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A stack of items in the world, which the player can pick up by activating a trigger collider.
/// </summary>
public class PickupItem : MonoBehaviour
{
    [SerializeField]
    private ItemBase _itemInfo;

    [SerializeField]
    private int _amount = 1;

    public ItemBase ItemInfo => _itemInfo;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player.InventoryScene inventoryAndHotBar))
        {
            if(inventoryAndHotBar.GoIntoFirst.TryAddItem(new ItemStack(_itemInfo, _amount)))
            {
                if(_itemInfo.ID == 1)
                {
                    if (DroneOnDewPickup.instance.didtheThing == false)
                    {
                        DroneOnDewPickup.instance.Activate();
                    }
                }
                Destroy(gameObject);
            }
        }
    }
}

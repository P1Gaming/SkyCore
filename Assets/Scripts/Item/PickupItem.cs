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

    private Rigidbody _rigidbody;

    public int Amount => _amount;
    public ItemBase ItemInfo => _itemInfo;
    public Rigidbody Rigidbody => _rigidbody;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player.InventoryScene inventoryAndHotBar))
        {
            if (inventoryAndHotBar.HotBar.TryAddItem(new ItemStack(_itemInfo, _amount)))
            {
                Destroy(gameObject);
            }
        }
    }

    
}

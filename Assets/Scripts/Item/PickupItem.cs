using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A stack of items in the world, which the player can pick up by activating a trigger collider.
/// </summary>
public class PickupItem : MonoBehaviour
{
    [SerializeField]
    private ItemIdentity _itemInfo;

    [SerializeField]
    private int _amount = 1;

    private Rigidbody _rigidbody;
    private ItemStack _stack;
    private bool _destroyed;

    public int Amount => _amount;
    public ItemIdentity ItemInfo => _itemInfo;
    public Rigidbody Rigidbody => _rigidbody;


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _stack = new ItemStack(_itemInfo, _amount);
    }

    private void OnTriggerEnter(Collider other) => TryPickup(other);
    private void OnTriggerStay(Collider other) => TryPickup(other);

    private void TryPickup(Collider other)
    {
        if (_destroyed)
        {
            // destroy doesn't happen until the end of the frame
            return;
        }
        if (!other.gameObject.CompareTag("Player"))
        {
            return;
        }

        Inventory.Instance.TakeInAsManyAsFit(_stack);
        if (_stack.amount == 0)
        {
            _destroyed = true;
            Destroy(gameObject);
        }
    }
}

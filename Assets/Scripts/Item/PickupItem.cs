using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A stack of items in the world, which the player can pick up by activating a trigger collider.
/// </summary>
public class PickupItem : MonoBehaviour
{
    [SerializeField]
    private Collider _pickupTrigger;

    [SerializeField]
    private ItemIdentity _itemInfo;

    [SerializeField]
    private int _amount = 1;

    [SerializeField]
    private float _durationToPreventPickupWhenTossed = 3f;

    [field: SerializeField]
    public Rigidbody Rigidbody { get; private set; }

    private ItemStack _stack;
    private bool _destroyed;

    public int Amount => _amount;
    public ItemIdentity ItemInfo => _itemInfo;


    private void Awake()
    {
        _stack = new ItemStack(_itemInfo, _amount);
    }


    public void PreventImmediatePickupWhenTossed() => StartCoroutine(DisableTriggerForDuration());

    private IEnumerator DisableTriggerForDuration()
    {
        _pickupTrigger.enabled = false;
        yield return new WaitForSeconds(_durationToPreventPickupWhenTossed);
        _pickupTrigger.enabled = true;
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

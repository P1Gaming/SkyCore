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

    [SerializeField, Tooltip("Time elapsed before the items collision is enabled.")]
    private float _timeTillEnabled = 1f;

    private Rigidbody _rigidbody;


    private void OnEnable()
    {
        if (_rigidbody == null)
            _rigidbody = GetComponent<Rigidbody>();

        StartCoroutine(EnableCollision(gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Player.InventoryScene inventoryAndHotBar) && GetComponent<Collider>().isTrigger)
        {
            if (inventoryAndHotBar.GoIntoFirst.TryAddItem(new ItemStack(_itemInfo, _amount)))
            {
                Destroy(gameObject);
            }
        }
    }

    IEnumerator EnableCollision(GameObject itemCollider)
    {
        itemCollider.GetComponent<Collider>().isTrigger = false;
        yield return new WaitForSeconds(_timeTillEnabled);
        itemCollider.GetComponent<Collider>().isTrigger = true;
    }

    public int Amount { get => _amount; }
    public ItemBase ItemInfo { get => _itemInfo; }
    public Rigidbody Rigidbody { get => _rigidbody; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A stack of items in the world, which the player can pick up by activating a trigger collider.
/// </summary>
public class PickupItem : MonoBehaviour
{
    private const float TOSS_IMPULSE_PER_MASS = 5f;
    private const float DURATION_TO_PREVENT_PICKUP_WHEN_TOSSED = 1.5f;
    private const float DURATION_OF_CONTINUOUS_COLLISION_DETECTION_WHEN_TOSSED = 20f;

    [SerializeField]
    private Collider _pickupTrigger;

    [SerializeField]
    private ItemIdentity _itemInfo;

    [SerializeField]
    private int _amount = 1;

    [field: SerializeField]
    public Rigidbody Rigidbody { get; private set; }

    public float CurrentAttractionRadius { get; private set; }

    private ItemStack _stack;
    private bool _destroyed;
    private float _defaultSleepThreshold;
    private CollisionDetectionMode _defaultCollisionDetectionMode;

    public int Amount => _amount;
    public ItemIdentity ItemInfo => _itemInfo;


    private void Awake()
    {
        _stack = new ItemStack(_itemInfo, _amount);
        _defaultSleepThreshold = Rigidbody.sleepThreshold;
        _defaultCollisionDetectionMode = Rigidbody.collisionDetectionMode;
        CurrentAttractionRadius = _itemInfo.AttractionRadius;
    }


    public void TossFromInventory(Vector3 direction) 
    {
        CurrentAttractionRadius = _itemInfo.AttractionRadiusIfSpawnedByTossingFromInventory;


        Rigidbody.AddForce(direction * TOSS_IMPULSE_PER_MASS * Rigidbody.mass, ForceMode.Impulse);

        // Don't pick up the item immediately.
        StartCoroutine(DisableTriggerTemporarily());

        // Use continuous collision detection for a bit, then switch to discrete for performance.
        StartCoroutine(UseContinuousCollisionDetectionTemporarily());
    }

    private IEnumerator DisableTriggerTemporarily()
    {
        _pickupTrigger.enabled = false;
        yield return new WaitForSeconds(DURATION_TO_PREVENT_PICKUP_WHEN_TOSSED); 
        _pickupTrigger.enabled = true;
    }

    private IEnumerator UseContinuousCollisionDetectionTemporarily()
    {
        Rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        yield return new WaitForSeconds(DURATION_OF_CONTINUOUS_COLLISION_DETECTION_WHEN_TOSSED);
        Rigidbody.collisionDetectionMode = _defaultCollisionDetectionMode;
    }


    private void OnTriggerEnter(Collider other) 
    {
        if (IsPlayer(other))
        {
            Rigidbody.sleepThreshold = -1; // don't sleep
            TryPickup();
        }
    }

    private void OnTriggerStay(Collider other) 
    {
        if (IsPlayer(other))
        {
            TryPickup();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // this will only work properly if the player has a single collider. Otherwise need to keep
        // track of how many of the player's colliders are inside the collider (so OnTriggerEnter => increment
        // and OnTriggerExit => decrement and if that makes the int 0, do the default sleep threshold.)

        // Controlling sleeping like this to ensure OnTriggerStay continues to get called, but allowing
        // sleeping in general for performance.

        if (IsPlayer(other))
        {
            Rigidbody.sleepThreshold = _defaultSleepThreshold;
        }
    }

    private static bool IsPlayer(Collider collider) => collider.gameObject.CompareTag("Player");

    private void TryPickup()
    {
        if (_destroyed)
        {
            // destroy doesn't happen until the end of the frame
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

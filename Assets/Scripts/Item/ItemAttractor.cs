using Player;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;


/// <summary>
/// Place this class on a GameObject to make it attract items.
/// </summary>
/// <remarks>
/// NOTE: You can make an item ignore item attractors. To do so, go to its ScriptableObject (ItemBase) and set the AttractionRadius property to 0.
/// </remarks>
public class ItemAttractor : MonoBehaviour
{
    [Tooltip("This property controls how strongly the item is pulled in.")]
    [Range(0.1f, 100f)]
    [SerializeField]
    private float _attractionForce = 10f;

    [Tooltip("This is the maximum speed an item is allowed to move at while being pulled in.")]
    [Range(10f, 100f)]
    [SerializeField]
    private float _maxAttractionSpeed = 20f;

    [Tooltip("This controls how well items will home in on the player. NOTE: Increasing the attraction speed of an item will also make it home in better since it'll just reach the attractor that much sooner.")]
    [Range(0f, 1f)]
    [SerializeField]
    private float _HomingPower = 0.75f;


    private Collider _collider;

    private List<PickupItem> _itemsBeingAttracted = new List<PickupItem>();

    // This is how frequently CheckForNewNearbyTargets() gets called.
    private float _itemDetectionFrequency = 1f;

    private float _homingStrength;

    private float _itemDetectionTimer;


    private InventoryScene _inventoryScene;



    private void Awake()
    {
        _collider = GetComponent<Collider>();

        // Get a reference to this object's InventoryScene or InventoryBase component. This script will use whichever is available.
        _inventoryScene = GetComponent<InventoryScene>();

        _homingStrength = _maxAttractionSpeed * (1f - _HomingPower);
    }


    // Update is called once per frame
    void Update()
    {
        _itemDetectionTimer += Time.deltaTime;
        if (_itemDetectionTimer >= _itemDetectionFrequency)
        {
            _itemDetectionTimer = 0;
            CheckForNewNearbyItems();
        }
    }

    private void FixedUpdate()
    {
        PullItems();
    }

    private void CheckForNewNearbyItems()
    {
        // We use MAX_ATTRACTION_RADIUS so any item within the max radius can be detected.
        // Whether it actually starts getting pulled to this attractor depends on how far away it actually is.
        Collider[] colliders = Physics.OverlapSphere(transform.position, ItemBase.MAX_ATTRACTION_RADIUS);
        foreach (Collider c in colliders)
        {
            PickupItem item = c.GetComponent<PickupItem>();
            if (item != null)
            {
                if (!_itemsBeingAttracted.Contains(item) &&
                    InventoryHasRoomFor(new ItemStack(item.ItemInfo, item.Amount)))
                {
                    _itemsBeingAttracted.Add(item);
                }
            }

        } // end foreach

    }

    /// <summary>
    /// Checks if there is enough room for the specified item stack in this GameObject's inventory.
    /// For now, this only supports the player's InventoryScene component. It could be expanded later if needed.
    /// </summary>
    /// <param name="item">The item stack to check if there is enough room for.</param>
    /// <returns>True if there is enough room for the passed in item stack.</returns>
    private bool InventoryHasRoomFor(ItemStack item)
    {
        if (_inventoryScene != null)
        {
            return _inventoryScene.HotBar.HasRoomForItem(item, true);
        }


        return false;
    }

    /// <summary>
    /// This function checks all items that are being pulled into the player.
    /// If an item is null (meaning it was collected by the player and destroyed)
    /// then this function will simply remove it from the list.
    /// </summary>
    private void PullItems()
    {
        for (int i = _itemsBeingAttracted.Count - 1; i >= 0; i--) 
        {
            PickupItem item = _itemsBeingAttracted[i];
            if (item == null) // If it's null, it means the player collected it.
            {
                _itemsBeingAttracted.RemoveAt(i);
            }
            else
            { 
                float itemDistance = Vector3.Distance(transform.position, item.transform.position);
                if (itemDistance <= item.ItemInfo.AttractionRadius &&
                    _inventoryScene.HotBar.HasRoomForItem(item, true))
                {
                    // I made a function call here so we can easily swap out this logic by calling a different function to change the attraction style.
                    AttractItemNatural(item);
                }
                else
                {
                    // The item is too far away now, so remove it from the list so we stop pulling on it.
                    _itemsBeingAttracted.RemoveAt(i--);
                }
            }
            
        } // end foreach
    }

    /// <summary>
    /// This function pulls the item straight toward the player. It is the most basic way to do it.
    /// </summary>
    /// <param name="item">The item to attract.</param>
    private void AttractItemLinear(PickupItem item)
    {
        // Get the direction from the item to this attractor.
        Vector3 direction = (transform.position - item.transform.position).normalized; 

        item.Rigidbody.velocity = direction * _attractionForce;
    }

    /// <summary>
    /// This function pulls the item towards the player, but makes it look like the object has more momentum so it doesn't turn as sharply.
    /// </summary>
    /// <param name="item">The item to attract.</param>
    private void AttractItemNatural(PickupItem item)
    {
        // Get the direction from the item to this attractor.
        Vector3 direction = (transform.position - item.transform.position).normalized;
        direction = item.Rigidbody.velocity + (direction * _attractionForce) / _homingStrength; // This division just slows down how quickly the item homes in on the player. A higher _attractionForce value will also make it appear to home in better.

        // If speed is greator than max, then set it to max.
        direction = direction.magnitude <= _maxAttractionSpeed ? direction : (direction.normalized * _maxAttractionSpeed);

        item.Rigidbody.velocity = direction;
    }
}

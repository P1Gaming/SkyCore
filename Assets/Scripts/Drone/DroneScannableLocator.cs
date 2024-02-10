using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jellies;

public class DroneScannableLocator
{
    private Collider[] _overlapSphereResults = new Collider[10000];
    private HashSet<ItemBase> _scannedItems = new();
    private HashSet<Jellies.JellyType> _scannedJellyTypes = new();
    private int _jellyLayerMask;
    private int _itemLayerMask;

    public DroneScannableLocator()
    {
        _jellyLayerMask = LayerMask.GetMask("Jellies");
        _itemLayerMask = LayerMask.GetMask("PickupItem");
    }

    public bool AlreadyScanned(MonoBehaviour thing)
    {
        Parameters parameters = thing as Parameters;
        if (parameters != null)
        {
            return _scannedJellyTypes.Contains(parameters.TypeOfThisJelly());
        }

        PickupItem item = thing as PickupItem;
        if (item != null)
        {
            return _scannedItems.Contains(item.ItemInfo);
        }

        throw new System.ArgumentException("thing must be a Parameters or a PickupItem");
    }

    public void MarkAsScanned(MonoBehaviour thing)
    {
        Parameters parameters = thing as Parameters;
        if (parameters != null)
        {
            _scannedJellyTypes.Add(parameters.TypeOfThisJelly());
        }

        PickupItem item = thing as PickupItem;
        if (item != null)
        {
            _scannedItems.Add(item.ItemInfo);
        }
    }

    /// <summary>
    /// Finds the PickupItem or Jellies.Parameters nearest the drone, so long as it's near enough to
    /// the player. Prioritizes jellies over items.
    /// </summary>
    public MonoBehaviour TryGetThingToScan(Transform drone, Transform player
        , float maxDistanceFromDrone, float maxDistanceFromPlayer)
    {
        Parameters jelly = TryGetJellyToScan(drone, player, maxDistanceFromDrone, maxDistanceFromPlayer);
        if (jelly != null)
        {
            return jelly;
        }

        return TryGetItemToScan(drone, player, maxDistanceFromDrone, maxDistanceFromPlayer);
    }

    private Parameters TryGetJellyToScan(Transform drone, Transform player
        , float maxDistanceFromDrone, float maxDistanceFromPlayer)
    {
        int numColliders = Physics.OverlapSphereNonAlloc(drone.position, maxDistanceFromDrone
            , _overlapSphereResults, _jellyLayerMask, QueryTriggerInteraction.Ignore);

        float lowestSqrDistanceFromDrone = float.PositiveInfinity;
        Parameters result = null;
        for (int i = 0; i < numColliders; i++)
        {
            _overlapSphereResults[i].TryGetComponent(out Parameters jelly);

            if (jelly == null)
            {
                continue;
            }
            if (_scannedJellyTypes.Contains(jelly.TypeOfThisJelly()))
            {
                continue;
            }

            float sqrDistanceFromPlayer = (jelly.transform.position - player.position).sqrMagnitude;
            if (sqrDistanceFromPlayer > maxDistanceFromPlayer * maxDistanceFromPlayer)
            {
                continue;
            }

            float sqrDistanceFromDrone = (jelly.transform.position - drone.position).sqrMagnitude;
            if (sqrDistanceFromDrone < lowestSqrDistanceFromDrone)
            {
                lowestSqrDistanceFromDrone = sqrDistanceFromDrone;
                result = jelly;
            }
        }

        return result;
    }

    private PickupItem TryGetItemToScan(Transform drone, Transform player
        , float maxDistanceFromDrone, float maxDistanceFromPlayer)
    {
        int numColliders = Physics.OverlapSphereNonAlloc(drone.position, maxDistanceFromDrone
            , _overlapSphereResults, _itemLayerMask, QueryTriggerInteraction.Collide);

        float lowestSqrDistanceFromDrone = float.PositiveInfinity;
        PickupItem result = null;
        for (int i = 0; i < numColliders; i++)
        {
            _overlapSphereResults[i].TryGetComponent(out PickupItem item);
            if (item == null)
            {
                continue;
            }

            if (_scannedItems.Contains(item.ItemInfo))
            {
                continue;
            }

            float sqrDistanceFromPlayer = (item.transform.position - player.position).sqrMagnitude;
            if (sqrDistanceFromPlayer > maxDistanceFromPlayer * maxDistanceFromPlayer)
            {
                continue;
            }

            float sqrDistanceFromDrone = (item.transform.position - drone.position).sqrMagnitude;
            if (sqrDistanceFromDrone < lowestSqrDistanceFromDrone)
            {
                lowestSqrDistanceFromDrone = sqrDistanceFromDrone;
                result = item;
            }
        }

        return result;
    }
}

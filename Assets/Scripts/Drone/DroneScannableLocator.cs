using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneScannableLocator
{
    private Collider[] _overlapSphereResults = new Collider[10000];
    private HashSet<MonoBehaviour> _scannedThings = new();

    // brain death right now, dunno what to call it besides thing.
    public bool AlreadyScanned(MonoBehaviour thing)
    {
        return _scannedThings.Contains(thing);
    }

    /// <summary>
    /// Finds the PickupItem or JellyInteractBase nearest the drone, so long as it's near enough to
    /// the player. Prioritizes jellies over items.
    /// </summary>
    public MonoBehaviour TryGetThingToScan(Transform drone, Transform player
        , float maxDistanceFromDrone, float maxDistanceFromPlayer)
    {
        int numColliders = Physics.OverlapSphereNonAlloc(drone.position, maxDistanceFromDrone, _overlapSphereResults);

        JellyInteractBase closestJelly = ClosestScannable<JellyInteractBase>(drone, player, maxDistanceFromPlayer, numColliders);
        if (closestJelly != null)
        {
            return closestJelly;
        }
        PickupItem closestItem = ClosestScannable<PickupItem>(drone, player, maxDistanceFromPlayer, numColliders);
        return closestItem;
    }

    private T ClosestScannable<T>(Transform drone, Transform player, float maxDistanceFromPlayer
        , int numColliders) where T : MonoBehaviour
    {
        float minSqrDistance = float.PositiveInfinity;
        T closestThing = null;
        for (int i = 0; i < numColliders; i++)
        {
            if (_overlapSphereResults[i].TryGetComponent(out T thing))
            {
                if (!_scannedThings.Contains(thing))
                {
                    float sqrDistanceFromPlayer = (player.position - thing.transform.position).sqrMagnitude;
                    if (sqrDistanceFromPlayer > maxDistanceFromPlayer * maxDistanceFromPlayer)
                    {
                        // The item needs to be within max distance from the player, but so long as that condition is
                        // met, use the item closest to the drone.
                        continue;
                    }

                    float sqrDistance = (drone.position - thing.transform.position).sqrMagnitude;
                    if (sqrDistance < minSqrDistance)
                    {
                        minSqrDistance = sqrDistance;
                        closestThing = thing;
                    }
                }
            }
        }
        return closestThing;
    }

    public void OnFinishedScanning(MonoBehaviour thing)
    {
        _scannedThings.Add(thing);
    }
}

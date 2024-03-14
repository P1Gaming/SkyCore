using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Wandering : MonoBehaviour
{
    public event System.Action<bool> OnChangeDirection;

    [SerializeField]
    private JellyStateMachine _stateMachine;
    [SerializeField]
    private GameEventScriptableObject _updateWandering;
    [SerializeField]
    private ItemBase _berry;
    [SerializeField]
    private NavMeshAgent _meshAgent;
    [SerializeField]
    private JellyInteractBase _jellyInteractBase;

    [SerializeField, Range(0,5), 
        Tooltip("Used to determine the full wandering distance for the jelly to wander.")]
    private float _wandDist = 0.5f;

    [SerializeField, Range(1,10), 
        Tooltip("Used to indicate the radius for jelly to wander, also helps determine if the jelly should wander backwards by comparing it to _wandDist.")]
    private float _wandRange = 2.0f;

    [SerializeField, Range(0,1), 
        Tooltip("Used to indicate an offset to let jelly know it has arrived at it's intended spot.")]
    private float _arrivalOffset = 0.1f;

    private GameEventResponses _gameEventResponses = new();

    private void Awake()
    {
        _gameEventResponses.SetSelectiveResponses(_stateMachine
            , (_updateWandering, UpdateWandering));
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    /// <summary>
    /// On update, if jelly can wander, calls chooseWanderDestination(). Else, if jelly's remaining distance is less than 
    /// stopping distance, then set jelly destination to current position.
    /// </summary>
    private void UpdateWandering()
    {
        if (!_jellyInteractBase.Interacting)
        {
            ChooseWanderDestination();
        }
        else if (_meshAgent.remainingDistance > _meshAgent.stoppingDistance)
        {
            _meshAgent.SetDestination(transform.position);
        }
    }

    /// <summary>
    /// Checks various conditions of the jelly's path and distance to determine if the jelly is near the destination.
    /// </summary>
    /// <param name="nearOffset"> Float variable that'll represent the offset</param>
    /// <returns><c>true</c> If jelly is not on a pending path and jelly's remaining distance is less than the min of jelly's stopping distance or nearOffset 
    /// and either the jelly's doesn't have a path or the squared magnitude is 0, false otherwise. </returns>
    private bool NearDestination(float nearOffset)
    {
        if (!_meshAgent.pathPending 
            && _meshAgent.remainingDistance < Mathf.Min(_meshAgent.stoppingDistance, nearOffset) 
            && (!_meshAgent.hasPath || _meshAgent.velocity.sqrMagnitude == 0))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Creates a new destination for the jelly to wander.
    /// </summary>
    /// <returns><c>true</c> If the jelly is currently on a destination path and not
    /// near the destination plus the offset or the jelly was given a new destination path, false otherwise. </returns>
    private bool ChooseWanderDestination()
    {
        if (!NearDestination(_arrivalOffset))
        {
            return true;
        }
        else 
        {
            // Create new destination for the jelly.

            Transform jellyTransform = transform;
            Vector3 wanderCenter = jellyTransform.position + (jellyTransform.forward * _wandDist);
            if (!CalcRandomWanderPoint(wanderCenter, _wandRange, out Vector3 calcPoint))
            {
                return false;
            }

            _meshAgent.stoppingDistance = 0.1f;
            _meshAgent.SetDestination(calcPoint);
            bool near = 2 * Vector3.Distance(calcPoint, jellyTransform.position) < _wandDist + _wandRange;
            OnChangeDirection?.Invoke(near);
        }
        return true;
    }
    
    /// <summary>
    /// Calculates a random wandering point.
    /// </summary>
    /// <returns><c>true</c> If a new vector3 (calcResult) is created, false otherwise.</returns>
    private bool CalcRandomWanderPoint(Vector3 centerPoint, float rangeToWander, out Vector3 calcResult)
    {
        bool hasBerries = false;
        Vector3 playerPos = new Vector3(0,0,0);
        UI.Inventory.InventoryUI inventory = UI.Inventory.InventoryUI.Instance;
        if (inventory != null)
        {
            hasBerries = inventory.HotBar.HasItem(_berry);
            playerPos = inventory.transform.position;
        }

        for (int i = 0; i < 30; i++)
        {
            Vector3 toSampleAt = centerPoint + (Random.insideUnitSphere * rangeToWander);
            if (hasBerries)
            {
                float lowestDistance = Vector3.Distance(toSampleAt, playerPos);
                for (int j = 0; j < 10; j++)
                {
                    Vector3 randomPoint = centerPoint + (Random.insideUnitSphere * rangeToWander);
                    float nextDistance = Vector3.Distance(randomPoint, playerPos);
                    if (nextDistance < lowestDistance)
                    {
                        lowestDistance = nextDistance;
                        toSampleAt = randomPoint;
                    }
                }
            }
            if (NavMesh.SamplePosition(toSampleAt, out NavMeshHit hit, _meshAgent.height * 2, NavMesh.AllAreas))
            {
                calcResult = hit.position;
                return true;
            }
        }

        calcResult = new Vector3(0,0,0);
        return false;
    }
}
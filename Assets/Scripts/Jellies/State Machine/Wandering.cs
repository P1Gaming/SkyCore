using Jellies.Behaviors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Wandering : State
{
    /// <summary>
    /// Used to indicate a change in direction.
    /// </summary>
    public event System.Action<bool> OnChangeDirection;

    /// <summary>
    /// Used to determine the full wandering distance for the jelly to wander.
    /// </summary>
    [SerializeField, Range(0,5), 
        Tooltip("Used to determine the full wandering distance for the jelly to wander.")]
    private float _wandDist = 0.5f;

    /// <summary>
    /// Used to indicate the radius for jelly to wander, also helps determine if the jelly should wander backwards by comparing it to _wandDist.
    /// </summary>
    [SerializeField, Range(1,10), 
        Tooltip("Used to indicate the radius for jelly to wander, also helps determine if the jelly should wander backwards by comparing it to _wandDist.")]
    private float _wandRange = 2.0f;

    /// <summary>
    /// Used to indicate an offset to let jelly know it has arrived at it's intended spot.
    /// </summary>
    [SerializeField, Range(0,1), 
        Tooltip("Used to indicate an offset to let jelly know it has arrived at it's intended spot.")]
    private float _arrivalOffset = 0.1f;

    /// <summary>
    /// ItemBase variable for berry.
    /// </summary>
    [SerializeField]
    private ItemBase _berry;
    /// <summary>
    /// NavMeshAgent variable from jelly.
    /// </summary>
    private NavMeshAgent _meshAgent;
    /// <summary>
    /// Boolean variable if the jelly can wander or not.
    /// </summary>
    private bool _allowedToWander;

    /// <summary>
    /// On awake, sets private _meshAgent and _allowedToWander.
    /// </summary>
    void Awake()
    {
        _meshAgent = GetComponent<NavMeshAgent>();
        _allowedToWander = true;
    }

    /// <summary>
    /// On update, if jelly can wander, calls chooseWanderDestination(). Else, if jelly's remaining distance is less than 
    /// stopping distance, then set jelly destination to current position.
    /// </summary>
    void Update()
    {
        //If _allowedToWander is true, call chooseWanderDestination function.
        if(_allowedToWander)
        {
            ChooseWanderDestination();
        }
        // Else, if the jelly's remainingDistance is less than stoppingDistance, set jelly destination to current position.
        else 
        {
            if(_meshAgent.remainingDistance > _meshAgent.stoppingDistance)
            {
                _meshAgent.SetDestination(transform.position);
            }
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
        if(!_meshAgent.pathPending 
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
    /// <returns><c>true</c> If the jelly is currently on a destination path and not near the destination plus the offset or the jelly was given a new destination path, false otherwise. </returns>
    private bool ChooseWanderDestination()
    {
        // Is the jelly not near the destination plus with the given offset.
        if(!NearDestination(_arrivalOffset))
        {
            return true;
        }
        // Create new destination for the jelly.
        else 
        {
            Transform jellyTransform = transform;
            Vector3 wanderCenter = jellyTransform.position + (jellyTransform.forward * _wandDist);
            // If calcRandomWanderPoint is false, return false.
            if(!CalcRandomWanderPoint(wanderCenter, _wandRange, out Vector3 calcPoint))
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
    /// <param name="centerPoint"> Vector3 variable that'll represent the center point.</param>
    /// <param name="rangeToWander"> Float variable that'll represent range a jelly can wander.</param>
    /// <param name="calcResult"> Return a Vector3 variable that'll represent the calculated result.</param>
    /// <returns><c>true</c> If a new vector3 (calcResult) is created, false otherwise.</returns>
    private bool CalcRandomWanderPoint(Vector3 centerPoint, float rangeToWander, out Vector3 calcResult)
    {
        Player.InventoryScene invScene = Player.InventoryScene.Instance;
        bool hasBerries = false;
        Vector3 playerPos = new Vector3(0,0,0);
        // If the InventoryScene is not null, set hasBerries if the player has berries or not and set player position with InventoryScene transform position.
        if(invScene != null)
        {
            hasBerries = invScene.HotBar.HasItem(_berry);
            playerPos = invScene.transform.position;
        }
        for(int i = 0; i < 30; i++)
        {
            Vector3 randomPoint1 = centerPoint + (Random.insideUnitSphere * rangeToWander);
            if(hasBerries)
            {
                float dist = 1000000;
                for(int j = 0; j < 10; j++)
                {
                    Vector3 randomPoint2 = centerPoint + (Random.insideUnitSphere * rangeToWander);
                    if(Vector3.Distance(randomPoint2, playerPos) < dist)
                    {
                        dist = Vector3.Distance(randomPoint2,playerPos);
                        randomPoint1 = randomPoint2;
                    }
                }
            }
            if(NavMesh.SamplePosition(randomPoint1, out NavMeshHit hit, _meshAgent.height * 2, NavMesh.AllAreas))
            {
                calcResult = hit.position;
                return true;
            }
        }

        calcResult = new Vector3(0,0,0);
        return false;
    }

    /// <summary>
    /// Set _allowedToWander to given state.
    /// </summary>
    /// <param name="newState"> Bool variable to set _allowedToWander.</param>
    public void ToggleWanderBool(bool newState)
    {
        _allowedToWander = newState;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Jellies.Behaviors;
[RequireComponent(typeof(NavMeshAgent))]
public class Watching : State
{
    [SerializeField,Range(0,10),Tooltip("The distance the jelly watches the subject from")]
    private int _watchDistance;
    private NavMeshAgent _agent;
    private Transform _player;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = Player.Motion.PlayerMovement.Instance.transform;
    }

    private void Update()
    {
        Watch();
    }

    public override void Exit()
    {
        _agent.ResetPath();
        base.Exit();
    }

    /// <summary>
    /// Watches the subject from the _watchDistance 
    /// </summary>
    private void Watch()
    {
        _agent.stoppingDistance = _watchDistance;
        _agent.SetDestination(_player.position);
    }
}

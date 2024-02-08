using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Jellies.Behaviors;
[RequireComponent(typeof(NavMeshAgent))]
public class NewWatching : NewState
{
    [SerializeField,Range(0,10),Tooltip("The distance the jelly watches the subject from")]
    private int _watchDistance;
    private NavMeshAgent _agent;
    private GameObject _subject;
    private GameObject Subject
    {
        set
        {
            _subject = value;
        }
        get
        {
            return _subject;
        }
    }
    /// <summary>
    /// Awake sets _agent to the jelly's NavMeshAgent component
    /// </summary>
    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        
    }

    /// <summary>
    /// If there is a subject, watch the subject
    /// </summary>
    void Update()
    {
        if(Subject != null)
        {
            Watch();
        }
        
    }
    /// <summary>
    /// Tells the jelly what to watch, enters the Watching state
    /// </summary>
    /// <param name="entity">
    /// The game object to watch
    /// </param>
    public override void Enter(GameObject entity)
    {
        Subject = entity;
        base.Enter(Subject);
    }
    public override void Exit()
    {
        Subject = null;
        _agent.ResetPath();
        base.Exit();
    }
    /// <summary>
    /// Watches the subject from the _watchDistance 
    /// </summary>
    /// <returns>
    /// True
    /// </returns>
    private bool Watch()
    {
        _agent.stoppingDistance = _watchDistance;
        _agent.SetDestination(Subject.transform.position);
        return true;
    }
}

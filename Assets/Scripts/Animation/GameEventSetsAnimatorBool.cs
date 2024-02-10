using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A game event sets an animator's bool parameter.
/// </summary>
public class GameEventSetsAnimatorBool : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;
    [SerializeField]
    private GameEventScriptableObject _gameEvent;
    [SerializeField]
    private string _boolName;
    [SerializeField]
    private bool _setTo;

    private int _boolParameterID;

    private void Awake()
    {
        _boolParameterID = Animator.StringToHash(_boolName);
    }

    private void OnEnable()
    {
        _gameEvent.OnRaise += Set;
    }

    private void OnDisable()
    {
        _gameEvent.OnRaise -= Set;
    }

    private void Set()
    {
        _animator.SetBool(_boolParameterID, _setTo);
    }
}

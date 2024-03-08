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

    private GameEventResponses _gameEventResponses = new();

    private void Awake()
    {
        _boolParameterID = Animator.StringToHash(_boolName);
        _gameEventResponses.SetResponses((_gameEvent, Set));
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    private void Set()
    {
        _animator.SetBool(_boolParameterID, _setTo);
    }
}

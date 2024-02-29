using System.Collections;
using UnityEngine;

public class Hopping : MonoBehaviour
{
    [SerializeField]
    private Wandering _wandering;

    [SerializeField, Tooltip("We manipulate local position of the model itself so we need a reference to just the model")]
    private GameObject _model;

    [SerializeField, Tooltip("The amount of time taken for jump to cpmplete")]
    private float _durationOfJump;

    [SerializeField, Tooltip("Total Height in Unity units for the Jelly to hop")]
    private float _totalJumpHeight;

    [SerializeField]
    private JellyStateMachine _stateMachine;
    [SerializeField]
    private GameEventScriptableObject _wanderExit;

    private bool _alreadyHopping;

    private bool _isFinished = true;

    private GameEventResponses _gameEventResponses = new();

    private void Awake()
    {
        _wandering.OnChangeDirection += SetIsFinished;
        _gameEventResponses.SetSelectiveResponses(_stateMachine
            , (_wanderExit, SetIsFinished));
    }

    private void OnEnable() => _gameEventResponses.Register();
    private void OnDisable() => _gameEventResponses.Unregister();

    void Update()
    {
        if (!_alreadyHopping && !_isFinished)
        {
            StartCoroutine(PerformTheHop());            
        }
    }

    private void SetIsFinished(bool newValue)
    {
        _isFinished = newValue;
    }

    private void SetIsFinished()
    {
        _isFinished = true;
    }

    private IEnumerator PerformTheHop()
    {
        _alreadyHopping = true;
        Transform localTransform = _model.transform;
        float trueHeight = _totalJumpHeight / Mathf.Pow(_durationOfJump / 2, 2); 

        for (float i = 0; i < _durationOfJump; i += Time.deltaTime)
        {
            Vector3 localPos = localTransform.localPosition;
            localTransform.localPosition = new Vector3(localPos.x, i * trueHeight * (_durationOfJump - i), localPos.z);
            yield return null;
        }

        _alreadyHopping = false;
    }

}


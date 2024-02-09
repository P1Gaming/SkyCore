using System.Collections;

using Jellies.Behaviors;
using UnityEngine;

    [RequireComponent(typeof(Wandering))]

    
    public class Hopping : MonoBehaviour
    {
    [SerializeField, Tooltip("We manipulate local position of the model itself so we need a reference to just the model")]
    private GameObject _model;

    [SerializeField, Tooltip("The amount of time taken for jump to cpmplete")]
    private float _durationOfJump;

    [SerializeField, Tooltip("Total Height in Unity units for the Jelly to hop")]
    private float _totalJumpHeight;

    private IEnumerable _jump;

    private bool _isObjectInMotion;

    private bool _isFinished=true;

    ///<summary>
    /// Make use of Awake method to bind private bool setters to event methods in the Wandering script
    ///</summary>
    private void Awake()
    {
        Wandering wandering = GetComponent<Wandering>();
        wandering.OnChangeDirection += SetIsFinished;
        wandering.Exited += SetIsFinishedWithState;
    }
    
    void Update()
    {
        if(_isObjectInMotion==true || _isFinished==true)
        {
            return;
        }
        else 
        {
            StartCoroutine(PerformTheHop());            
        }
    }

    private void SetIsFinished(bool newValue)
    {
        _isFinished = newValue;
    }

    private void SetIsFinishedWithState(State stateInfo)
    {
        _isFinished = true;
    }

    private IEnumerator PerformTheHop()
    {
        _isObjectInMotion = true;
        Transform localTransform = _model.transform;
        float trueHeight = _totalJumpHeight / Mathf.Pow((_durationOfJump/2),2); 

        for(float i=0;i<_durationOfJump;i+=Time.deltaTime)
        {
            Vector3 localPos = localTransform.localPosition;
            localTransform.localPosition = new Vector3(localPos.x,i*trueHeight*(_durationOfJump-i), localPos.z);
            yield return null;
        }

        _isObjectInMotion = false;
    }

    }


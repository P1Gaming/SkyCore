using UnityEngine;

/// <summary>
/// This script will be responsible for calling out an event in response to the
/// player being detected with the attached sphere collider. 
/// </summary>
public class DetectPlayer : MonoBehaviour
{
    [SerializeField]
    GameEventScriptableObject _eventToBroadcast;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && _eventToBroadcast != null)
        {
           _eventToBroadcast.Raise();
        }
    }
}

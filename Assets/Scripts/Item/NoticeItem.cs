using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoticeItem : MonoBehaviour
{
    [SerializeField] Sprite theImage;

    public static bool noticed { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("DRONE"))
        {
            if(!noticed)
            {
                noticed = true;
                ShowNewlyScannedThing.ShowImage(theImage);
            }
        }
    }
}

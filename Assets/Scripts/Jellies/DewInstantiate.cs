using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DewInstantiate : MonoBehaviour
{
    [SerializeField]
    private GameObject _dewPrefab;
    
    public void DewSpawn()
    {
        Instantiate(_dewPrefab, transform.position + (transform.right * 1), transform.rotation);
    }
}

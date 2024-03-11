using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DewInstantiate : MonoBehaviour
{
    [SerializeField]
    private GameObject _dewPrefab;

    [SerializeField]
    private float minXSpawnRange = 1f;

    [SerializeField]
    private float maxXSpawnRange = 2f;
    
    public void DewSpawn(int amount)
    {
        for(int i = 0; i < amount; i++)
            Instantiate(_dewPrefab, transform.position + (transform.right * Random.Range(minXSpawnRange, maxXSpawnRange)), transform.rotation);
    }
}

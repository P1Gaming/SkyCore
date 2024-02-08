using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField, Tooltip("Prefab to spawn")]
    GameObject _objectToSpawn;

    bool _objectCanSpawn = true;

    // <summary> 
    // Spawns the Prefab at the given object's location
    // <summary>
    public void SpawnObject()
    {
        if(_objectCanSpawn)
        {
            GameObject spawnedObj = Instantiate(_objectToSpawn, transform.position, _objectToSpawn.transform.rotation);
            Animator anim = spawnedObj.GetComponent<Animator>();
            if(anim != null)
            {
                anim.SetTrigger("Spawn");
            }
            _objectCanSpawn = false;
        }
    }

    // <summary> 
    // Function to be used to reset the spawner
    // <summary>
    public void ResetSpawn()
    {
        _objectCanSpawn = true;
    }

}

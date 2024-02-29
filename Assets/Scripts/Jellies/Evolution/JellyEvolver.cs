using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles evolution of a jelly.
/// </summary>
public class JellyEvolver : MonoBehaviour
{
    [SerializeField, Tooltip("The root gameObject of the jelly.")]
    private GameObject _root;

    [SerializeField]
    private GameObject _evolutionPrefab;

    private List<ICopierWhenJellyEvolves> _copiers = new List<ICopierWhenJellyEvolves>();
    
    private void Update()
    {
        // for testing.
        if (Input.GetKeyDown(KeyCode.F))
        {
            Evolve();
        }
    }

    private void CopyFrom(GameObject oldJelly)
    {
        for (int i = 0; i < _copiers.Count; i++)
        {
            _copiers[i].CopyWhenInstantiatedForEvolution(oldJelly);
        }
    }

    /// <summary>
    /// Evolves this jelly by instantiating the next evolution, copying data to it, 
    /// and destroying the prior evolution jelly.
    /// </summary>
    public void Evolve()
    {
        if (_evolutionPrefab == null)
        {
            return;
        }

        GameObject instantiated = Instantiate(_evolutionPrefab
            , _root.transform.position, _root.transform.rotation);

        JellyEvolver newEvolver = instantiated.GetComponentInChildren<JellyEvolver>();
        newEvolver.CopyFrom(_root);

        Destroy(_root);
    }

    public void AddCopier(ICopierWhenJellyEvolves copier)
    {
        _copiers.Add(copier);
    }
}

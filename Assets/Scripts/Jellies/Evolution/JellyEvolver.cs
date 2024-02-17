using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Handles evolution of a jelly.
/// </summary>
public class JellyEvolver : MonoBehaviour
{
    [Header("Don't revert this in prefab mode on a\nprefab variant, as it's likely\noverriding a hidden field")]
    [SerializeField, Tooltip("The root gameObject of the jelly.")]
    private GameObject _root;

    [SerializeField, HideInInspector]
    private GameObject _evolutionPrefab;
    // ^ if you change the name of this, change the string in SetEvolutionPrefab to match

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

#if UNITY_EDITOR
    public void SetEvolutionPrefab(GameObject evolutionPrefab)
    {
        // Setting _evolutionPrefab directly doesn't seem to save properly, so do this.
        SerializedObject so = new SerializedObject(this);
        so.FindProperty("_evolutionPrefab").objectReferenceValue = evolutionPrefab;
        so.ApplyModifiedPropertiesWithoutUndo();
    }
#endif
}

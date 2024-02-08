using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A scriptable object to store the sequence of prefabs for one sequence of jellies evolutions.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Jelly Evolution Sequence")]
public class JellyEvolutionSequence : ScriptableObject
{
    [SerializeField, Tooltip("The sequence of jelly prefabs or prefab variants.")]
    private GameObject[] _evolutionSequencePrefabs;

#if UNITY_EDITOR
    private void OnValidate()
    {
        for (int i = 0; i < _evolutionSequencePrefabs.Length; i++)
        {
            if (_evolutionSequencePrefabs[i] == null)
            {
                Debug.LogError("In a JellyEvolutionSequence scriptable object, one of the prefabs in the array is null", this);
                return;
            }
        }

        for (int i = 0; i < _evolutionSequencePrefabs.Length - 1; i++)
        {
            JellyEvolver evolver = _evolutionSequencePrefabs[i].GetComponentInChildren<JellyEvolver>();
            evolver.SetEvolutionPrefab(_evolutionSequencePrefabs[i + 1]);
        }
        if (_evolutionSequencePrefabs.Length != 0)
        {
            JellyEvolver lastEvolutionEvolver = _evolutionSequencePrefabs[^1].GetComponentInChildren<JellyEvolver>();
            lastEvolutionEvolver.SetEvolutionPrefab(null);
        }
    }
#endif
}

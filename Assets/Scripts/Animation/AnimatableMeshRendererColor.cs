using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Has an inspector field for a color, which can be animated and is applied to a mesh renderer every frame.
/// </summary>
public class AnimatableMeshRendererColor : MonoBehaviour
{
    [SerializeField]
    private Color _color = Color.white;
    [SerializeField]
    private MeshRenderer _meshRenderer;

    private int _colorPropertyID;
    private MaterialPropertyBlock _properties;

    private void Awake()
    {
        _colorPropertyID = Shader.PropertyToID("_BaseColor");
        _properties = new MaterialPropertyBlock();
    }

    private void LateUpdate()
    {
        // Do it like this for performance, because altering the material directly
        // creates a copy of the material
        _properties.SetColor(_colorPropertyID, _color);
        _meshRenderer.SetPropertyBlock(_properties);
    }
}

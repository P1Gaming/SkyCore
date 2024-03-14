using Player.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceBlockHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _idealBlockPlacement;

    [SerializeField]
    private GameObject _blockHelperReference;

    private GameObject _activeBlockHelper;

    [SerializeField]
    private float _cellSize = 1f;

    [SerializeField]
    private Vector3 _offSet = new Vector3(0.56f, 0.506f, 0.03f);

    // Start is called before the first frame update
    void Start()
    {
        _activeBlockHelper = Instantiate(_blockHelperReference);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 gridPosition = _idealBlockPlacement.transform.position;

        gridPosition.x = Mathf.Floor(gridPosition.x / _cellSize) * _cellSize + _offSet.x;
        gridPosition.y = Mathf.Floor(gridPosition.y / _cellSize) * _cellSize + _offSet.y;
        gridPosition.z = Mathf.Floor(gridPosition.z / _cellSize) * _cellSize + _offSet.z;
        if (_activeBlockHelper)
        {
            _activeBlockHelper.transform.position = gridPosition;
        }
    }
}

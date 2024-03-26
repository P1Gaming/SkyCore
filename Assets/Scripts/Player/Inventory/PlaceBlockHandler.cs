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
    private LayerMask _groundLayer;

    [SerializeField]
    private Vector3 _offSet = Vector3.zero;

    [SerializeField]
    private GameObject _blockRef;

    private List<GameObject> _overlappedObjects = new List<GameObject>();
 
    // Start is called before the first frame update
    void Start()
    {
        _activeBlockHelper = Instantiate(_blockHelperReference);
    }

    // Update is called once per frame

    //Temp comment
    void Update()
    {
        //Check if player is holding placeable item?
        //GetComponentInParent<HoldingItemHandler>().GetCurrentHeldItem().itemInfo.Attributes.;
        Vector3 gridPosition = _idealBlockPlacement.transform.position;

        gridPosition.x = Mathf.Floor(gridPosition.x / _cellSize) * _cellSize + _offSet.x;
        gridPosition.y = Mathf.Floor(gridPosition.y / _cellSize) * _cellSize + _offSet.y;
        gridPosition.z = Mathf.Floor(gridPosition.z / _cellSize) * _cellSize + _offSet.z;

        while (Physics.CheckBox(gridPosition, Vector3.one / 2.001f, Quaternion.identity, _groundLayer))
        {
            gridPosition.y += _cellSize;
        }

        if (_activeBlockHelper)
        {
            _activeBlockHelper.transform.position = gridPosition;
        }

        PlaceBlockChecker();

    }

    private void PlaceBlockChecker()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Instantiate(_blockRef, _activeBlockHelper.transform.position, _activeBlockHelper.transform.rotation);
        }
    }
}

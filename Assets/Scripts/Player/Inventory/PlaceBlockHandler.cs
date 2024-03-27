using Player.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    void Update()
    {
        //Check if player is holding item?
        if (GetComponentInParent<HoldingItemHandler>().HeldItem != null)
        {
            PlaceableItemIdentity placeableItem = GetComponentInParent<HoldingItemHandler>().HeldItem.identity as PlaceableItemIdentity;

            //Check if item can be placed
            if (placeableItem != null && placeableItem.CanBePlaced)
            {
                if (!_activeBlockHelper)
                {
                    _activeBlockHelper = Instantiate(_blockHelperReference);
                }

                //Covert to grid coordinates
                Vector3 gridPosition = _idealBlockPlacement.transform.position;

                gridPosition.x = Mathf.Floor(gridPosition.x / _cellSize) * _cellSize + _offSet.x;
                gridPosition.y = Mathf.Floor(gridPosition.y / _cellSize) * _cellSize + _offSet.y;
                gridPosition.z = Mathf.Floor(gridPosition.z / _cellSize) * _cellSize + _offSet.z;

                //Move up if it runs into a collision
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
            else
            {
                Destroy(_activeBlockHelper);
            }
        }
        else
        {
            Destroy(_activeBlockHelper);
        }
    }

    private void PlaceBlockChecker()
    {
        //Check if the player presses m1 and make sure the backpack is not opened.
        if (Input.GetButtonDown("Fire1") && !Inventory.Instance._isInBackpackMode && !PlayerInteraction.Instance.IgnoreInputs)
        {
            Instantiate(_blockRef, _activeBlockHelper.transform.position, _activeBlockHelper.transform.rotation);
            Inventory.Instance.TrySubtractItemAmount(GetComponentInParent<HoldingItemHandler>().HeldItem.identity, 1);
        }
    }
}

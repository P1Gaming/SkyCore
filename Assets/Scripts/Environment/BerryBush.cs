


using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    [SerializeField, Tooltip("How long the bush takes to regrow berries")]
    private int _cooldown;
    ItemStack _item;
    [SerializeField]
    ItemBase _itemBase;
    private bool _canBeHarvested = false;
    [SerializeField, Tooltip("The number of berries to be harvested")]
    private int _numOfBerries;

    [SerializeField, Tooltip("The visual indicator that the bush is ready to harvest")]
    private GameObject _indicator;
    // Start is called before the first frame update
    void Start()
    {
        SpawnBerry();

    }
    private void Update()
    {
        if (_canBeHarvested && !_indicator.activeInHierarchy)
        {
            _indicator.SetActive(true);
        }
        else if (!_canBeHarvested && _indicator.activeInHierarchy)
        {
            _indicator.SetActive(false);
        }
    }

    private void SpawnBerry()
    {
        _canBeHarvested = true;
        _indicator.SetActive(true);
    }
    public void Harvest()
    {
        if (_canBeHarvested)
        {
            _item = new ItemStack(_itemBase, _numOfBerries);
            if (InventoryScene.Instance.GoIntoFirst.TryAddItem(_item))
            {
                _canBeHarvested = false;
                _indicator.SetActive(false);
            }
            StartCoroutine(BushCooldown());
        }


    }
    private IEnumerator BushCooldown()
    {
        yield return new WaitForSeconds(_cooldown);
        SpawnBerry();
    }
}


using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    [SerializeField, Tooltip("How long the bush takes to regrow berries")]
    private int _cooldown;
    
    [SerializeField]
    private ItemIdentity _berryItemIdentity;
    private bool _canBeHarvested = false;
    [SerializeField, Tooltip("The number of berries to be harvested")]
    private int _numOfBerries;

    [SerializeField, Tooltip("The visual indicator that the bush is ready to harvest")]
    private GameObject _indicator;

    private ItemStack _item;

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
            _item = new ItemStack(_berryItemIdentity, _numOfBerries);
            Inventory.Instance.TakeInAsManyAsFit(_item);
            if (_item.amount == 0)
            {
                _canBeHarvested = false;
                _indicator.SetActive(false);
            }
            if (_item.amount < 0)
            {
                throw new System.Exception("_item.amount < 0 in BerryBush.Harvest: " + _item.amount);
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

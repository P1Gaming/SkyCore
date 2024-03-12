using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryBush : MonoBehaviour
{
    [SerializeField, Tooltip("How long the bush takes to regrow berries")]
    private int _cooldown;
    [SerializeField,Tooltip("The berry game object")]
    GameObject _berry;
    private GameObject _berryToTake;
    ItemStack _item;
    private bool _canBeHarvested = false;

    [SerializeField,Tooltip("The visual indicator that the bush is ready to harvest")]
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
        _berryToTake = Instantiate(_berry,_berry.transform);
        _canBeHarvested = true;
        _indicator.SetActive(true);
    }
    public void Harvest(PlayerInteraction player)
    {
        if (_canBeHarvested)
        {
            if (player.GetComponent<Player.InventoryBase>().TryAddItem(_item))
            {
                _canBeHarvested = false;
                Destroy(_berryToTake);
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

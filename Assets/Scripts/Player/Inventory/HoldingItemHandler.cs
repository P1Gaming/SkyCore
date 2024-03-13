using Player;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UI.Inventory;
using UnityEngine;

public class HoldingItemHandler : MonoBehaviour
{

    private static InventoryScene _instance;
    public int _heldItemIndex = 0;

    [SerializeField]
    private GameObject _helditem;

    private ItemStack _item;

    private GameObject _highLightHotBar;

    // Start is called before the first frame update
    void Start()
    {
        _instance = GameObject.FindGameObjectWithTag("Player")?.GetComponent<InventoryScene>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            HeldItemHandling(0);
        }
        else if (Input.GetKeyDown("2"))
        {
            HeldItemHandling(1);
        }
        else if (Input.GetKeyDown("3"))
        {
            HeldItemHandling(2);
        }
    }

    private void HeldItemHandling(int index)
    {
        ItemStack ItemRef = InventoryUI.Instance.Hotbar.InventoryHotbar.GetItemIndex(index);

        if (ItemRef != null)
        {
            //Visually Change Model
            _helditem.GetComponent<MeshFilter>().sharedMesh = ItemRef.itemInfo.ItemPrefab.GetComponent<MeshFilter>().sharedMesh;
            //Copy Materials Over
            _helditem.GetComponent<MeshRenderer>().sharedMaterials = ItemRef.itemInfo.ItemPrefab.GetComponent<MeshRenderer>().sharedMaterials;

            _item = ItemRef;
        }
        else
        {
            _helditem.GetComponent<MeshFilter>().sharedMesh = null;
            _item = null;
        }
        //Change Index
        _heldItemIndex = index;

        //TODO: This gets the hotbar, and goes ALLLLL they way done to the grid inside the hotbar. Ideally there should be a reference for this, but for the time being it works.
        GameObject selectedGrid = InventoryUI.Instance.Hotbar.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(index).gameObject;
        InventoryUI.Instance.HotbarHighlight.transform.position = selectedGrid.transform.position;
    }

    public ItemStack GetCurrentHeldItem()
    {
        return _item;
    }

    public void UpdateHeldItem()
    {
        UnityEngine.Debug.Log("KaCAw");
        HeldItemHandling(_heldItemIndex);
    }
}

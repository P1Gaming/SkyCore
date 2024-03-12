using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    /// <summary>
    /// UI for the hot bar.
    /// </summary>
    public class HotBarUI : MonoBehaviour
    {
        [SerializeField, Tooltip("add a prefab for an inventory slot")]
        private GameObject _inventorySlotPrefab;

        [SerializeField, Tooltip("Add the GameObject representing thr hotbar that has a grid layout group component")]
        private GameObject _hotBarGrid;

        [SerializeField]
        private Transform _itemParentDuringDragAndDrop;

        private InventoryUIBase _inventoryOrHotBarUI;

        private void Awake()
        {
            Player.InventoryBase hotbar 
                = GameObject.FindGameObjectWithTag("Player").GetComponent<Player.InventoryScene>().HotBar;
            if (hotbar == null)
            {
                Debug.LogError("Hotbar on player could not be found.");
            }

            _inventoryOrHotBarUI = new InventoryUIBase(_inventorySlotPrefab, _hotBarGrid, hotbar, _itemParentDuringDragAndDrop, ItemBase.ItemSortType.None);

        }

        public void SetDragAndDrop(InventoryDragAndDrop dragAndDrop) => _inventoryOrHotBarUI.InventoryOrHotBar.Initialize(dragAndDrop);

        private void OnEnable()
        {
            _inventoryOrHotBarUI.OnEnable();
        }

        private void OnDestroy()
        {
            _inventoryOrHotBarUI.OnDestroy();
        }
    }
}
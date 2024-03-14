using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    /// <summary>
    /// Stores the hotbar and inventory for inspector settings, and decides where picked up items go first.
    /// </summary>
    public class InventoryScene : MonoBehaviour
    {
        [SerializeField]
        private InventoryBase _hotBar;
        [SerializeField]
        private InventoryBase _inventoryResource;
        [SerializeField]
        private InventoryBase _inventoryJelly;
        [SerializeField]
        private InventoryBase _inventoryTool;

        public InventoryBase HotBar => _hotBar;
        public InventoryBase InventoryResource => _inventoryResource;
        public InventoryBase InventoryJelly => _inventoryJelly;
        public InventoryBase InventoryTool => _inventoryTool;

        private static InventoryScene _instance;
        public static InventoryScene Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryScene>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            _hotBar.SetOverflowTo(new InventoryBase[] { _inventoryResource, _inventoryJelly, _inventoryTool });
        }
    }

}
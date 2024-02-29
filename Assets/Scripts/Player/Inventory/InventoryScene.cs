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
        public InventoryBase GoIntoFirst { get; private set; }

        private static InventoryScene _instance;
        public static InventoryScene Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindGameObjectWithTag("Player")?
                        .GetComponent<InventoryScene>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            DecideWhereItemsGoFirstAndOverflow(out InventoryBase first, out InventoryBase[] overflow);
            GoIntoFirst = first;
            first.SetOverflowTo(overflow);
        }

        private void DecideWhereItemsGoFirstAndOverflow(out InventoryBase first, out InventoryBase[] overflow)
        {
            first = _hotBar;
            overflow = new InventoryBase[3];
            overflow[0] = _inventoryResource;
            overflow[1] = _inventoryJelly;
            overflow[2] = _inventoryTool;
            Debug.Log(overflow.ToString());
            foreach (InventoryBase a in overflow)
            {
                Debug.Log(a.StacksCapacityResource.ToString());
            }
        }
    }

}
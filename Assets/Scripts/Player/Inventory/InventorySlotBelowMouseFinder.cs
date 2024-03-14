using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Inventory
{
    /// <summary>
    /// Finds any inventory slot below the mouse.
    /// </summary>
    public class InventorySlotBelowMouseFinder
    {
        private List<RaycastResult> _raycastResults = new List<RaycastResult>();
        private PointerEventData _eventData;
        private EventSystem _eventSystem;

        /// <returns>An inventory slot below the mouse, or null if none.</returns>
        public InventorySlotUI FindInventorySlotBelowMouse()
        {
            // source: https://forum.unity.com/threads/how-to-detect-if-mouse-is-over-ui.1025533/
            // is there a better way to do this?

            if (_eventSystem != EventSystem.current)
            {
                _eventSystem = EventSystem.current;

                if (_eventSystem == null)
                {
                    return null;
                }

                _eventData = new PointerEventData(_eventSystem);
            }

            _eventData.position = Input.mousePosition;

            _raycastResults.Clear();
            _eventSystem.RaycastAll(_eventData, _raycastResults);

            foreach (RaycastResult raycastResult in _raycastResults)
            {
                InventorySlotUI result = raycastResult.gameObject.GetComponent<InventorySlotUI>();
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    }
}
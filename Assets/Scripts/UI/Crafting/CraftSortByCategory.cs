using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftSortByCategory : MonoBehaviour
{
    [SerializeField]
    private GameObject _craftUI; // Need Access to the UI to reconstruct the list
    [SerializeField]
    private string _craftCategory;



    /// <summary>
    /// The functionality that plays out when the category button is pressed
    /// Has to go through this script so that it can recognize the category
    /// </summary>
    public void OnCategoryButtonPress()
    {
        if (_craftCategory.Equals("")) //This checks to make sure categories have been added otherwise it will error most likely
        {
            return;
        }
        _craftUI.GetComponent<CraftingUICode>().ClearList();
        _craftUI.GetComponent<CraftingUICode>().AddItemsToList(_craftCategory);
    }


}

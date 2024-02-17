using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script stores data for 
/// </summary>
public class RecipeListItemScript : CraftingRecipeData 
{
    private string _itemName;
    private int _levelRequirement;
    private string _category;
    private MaterialRequirement[] _materialRequirementsList;
    private Sprite _itemIcon;
    private string _craftedItem; // TODO: Should be substituted later for Item object

    /// <summary>
    /// The following functions are setters for the above variables.
    /// </summary>
    public void SetName(string name)
    {
        _itemName = name;
    }

    public void SetLevel(int level)
    {
        _levelRequirement = level;
    }

    public void SetCategory(string category)
    {
        _category = category;
    }

    public void SetMaterials(MaterialRequirement[] materialList)
    {
        _materialRequirementsList = new MaterialRequirement[materialList.Length];
        for (int i = 0; i < materialList.Length; i++)
        {
            _materialRequirementsList[i] = materialList[i];
        }
    }

    public void SetIcon(Sprite icon)
    {
        _itemIcon = icon;
    }

    public void SetItem(string item)
    {
        _craftedItem = item;
    }

    /// <summary>
    /// The following functions are getters for the above variables.
    /// </summary>
    public string GetName()
    {
        return _itemName;
    }

    public int GetLevel()
    {
        return _levelRequirement;
    }

    public string GetCategory()
    {
        return _category;
    }

    public MaterialRequirement[] GetMaterials()
    {
        return _materialRequirementsList;
    }

    public Sprite GetIcon()
    {
        return _itemIcon;
    }

    public string GetItem()
    {
        return _craftedItem;
    }


    /// <summary>
    /// Output for debugging materials and quantity.
    /// </summary>
    /// <returns>String of materials.</returns>
    /*public string GetMaterialString()
    {
        string output = "";
        for (int i = 0; i < _materialRequirementsList.Length; i++)
        {
            output += _materialRequirementsList[i].GetQuantity() + " " + _materialRequirementsList[i].GetMaterial() + ", "; 
        }

        return output;
    }*/

}

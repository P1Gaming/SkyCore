using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipeData : MonoBehaviour
{
    /// <summary>
    /// The purpose of this script is to house ALL of the data for the craftable item recipes
    /// This is for the crafting system UI to pull data from the list and know what materials
    /// are needed for the crafting system.
    /// 
    /// This script should ONLY contain GET functions and NEVER SET anything!
    /// Given this is a database, all of the data needs to remain so it can be
    /// used again anywhere.
    /// 
    /// If data needs to be set, use the game object "CraftItemData"
    /// and add data through the Inspector
    /// That game object can be referenced and can then pull data from this script
    /// </summary>

    /// <summary>
    /// The Material Requirements class is an object that stores
    /// information about a material such as the name of the material that should be used in the inventory
    /// and how much of that material to be used in the crafting process
    /// 
    /// Note: the type of Material may need to change to match how it is used
    /// in the inventory system. The material is also accessed throughout this script
    /// and would also need to match those changes made.
    /// </summary>
    [System.Serializable]
    public class MaterialRequirement
    {
        [SerializeField]
        private string _material;
        [SerializeField]
        private int _quantity;
        [SerializeField]
        private Sprite _icon;

        public string GetMaterial() { return _material; }
        public int GetQuantity() { return _quantity; }
        public Sprite GetIcon() { return _icon; }
    }

    /// <summary>
    /// The Craftable Item class is an object to store information for a singular
    /// crafting recipe. This will store the name of the item to be crafted,
    /// the items level requirement, the category the item belongs in,
    /// the list of the materials needed to craft said item (uses the above material requirement object)
    /// the items icon
    /// and what item is outputed to the players inventory when crafted.
    /// 
    /// Note: the output crafted item may need to change the type to match how it is
    /// used in the inventory system. This item is accessed throughout the script
    /// and would need to match those changes.
    /// </summary>
    [System.Serializable]
    public class CraftableItem
    {
        [SerializeField]
        private string _itemName;
        [SerializeField]
        private int _levelRequirement;
        [SerializeField]
        private string _category;
        [SerializeField]
        private MaterialRequirement[] _materialRequirementsList;
        [SerializeField]
        private Sprite _itemIcon;
        [SerializeField]
        private string _craftedItem; // Should be substituted later for Item object


        public string GetItemName() { return _itemName;  }
        public int GetLevelRequirement() { return _levelRequirement; }
        public string GetCategory() { return _category; }
        public MaterialRequirement[] GetMaterialRequirementsList() { return _materialRequirementsList; }
        public string GetMaterialRequirement(int index) { return _materialRequirementsList[index].GetMaterial(); }
        public int GetMaterialRequirementQuantity(int index) { return _materialRequirementsList[index].GetQuantity(); }
        public Sprite GetMaterialRequirementIcon(int index) { return _materialRequirementsList[index].GetIcon(); }
        public Sprite GetItemIcon() { return _itemIcon; }
        public string GetCraftedItem() { return _craftedItem;  }

    }
    /// <summary>
    /// This is the list of craftable items that will show up in the Unity Inspector.
    /// Paired with the prefab "CraftItemData" allows anyone to add into the database
    /// to then be referenced later to access the data.
    /// </summary>
    [SerializeField]
    private CraftableItem[] _craftableItemsList;

    private void Start() 
    {
        SortTheList();
    }

    /*
    //A temporary function that prints the data to the console
    //Useful for testing purposes only
    private void TempPrintList() 
    {
        for (int i= 0; i < CraftableItemsList.Length; i++)
        {
            Debug.Log(CraftableItemsList[i].GetItemName());
        }
    }
    */
    /// <summary>
    /// The sort list function performs a Bubble Sort to sort with a unique condition on the entire list.
    /// Why Bubble Sort? Although we can attempt to use something like Merge Sort that has
    /// O(nlogn) for its average and worst cases, it has a memory consumption of n, which isn't
    /// very good if our size gets too large. Merge sort also isn't the best for small sizes.
    /// If our size is small, bubble sort is very efficient and has a O(1) for memory consumption. 
    /// We also have a fairly complex comparison to make that has 3 variables to compare.
    /// Bubble sort makes that easier.
    /// The condition priority for sorting is Level -> Category -> Alphabetical
    /// Having the list sorted in this method allows for it to be loaded in via level through a for each loop
    /// </summary>
    private void SortTheList() 
    {
        for (int i= 0; i < _craftableItemsList.Length - 1; i++) 
        {
            bool havePerformedASwap = false;

            for (int j= 0; j < _craftableItemsList.Length - i - 1; j++) 
            {
                //Check based on Level -> Category -> Alphabetical
                //Case 1: Levels are different
                //Case 2: Levels are same, categories are different
                //Case 3: Levels are same, categories are same, alphabetically different
                if ((_craftableItemsList[j].GetLevelRequirement() > _craftableItemsList[j + 1].GetLevelRequirement()) //Case 1
                    //Case 2
                    || (_craftableItemsList[j].GetLevelRequirement() == _craftableItemsList[j + 1].GetLevelRequirement() &&
                    _craftableItemsList[j].GetCategory().CompareTo(_craftableItemsList[j + 1].GetCategory()) == 1)
                    //Case 3
                    || (_craftableItemsList[j].GetLevelRequirement() == _craftableItemsList[j + 1].GetLevelRequirement() &&
                    _craftableItemsList[j].GetCategory().CompareTo(_craftableItemsList[j + 1].GetCategory()) == 0 &&
                    _craftableItemsList[j].GetItemName().CompareTo(_craftableItemsList[j + 1].GetItemName()) == 1)
                    )
                {
                    //We now swap the two positions
                    CraftableItem temp = _craftableItemsList[j];
                    _craftableItemsList[j] = _craftableItemsList[j + 1];
                    _craftableItemsList[j + 1] = temp;
                    havePerformedASwap = true;
                }
            }

            //If no elements were swapped in the list, we should break
            if (!havePerformedASwap) 
            {
                break;
            }
        }
    }



    /// <summary>
    /// The next set of functions act as getters to the above data
    /// </summary>
    public string GetItemName(int index)
    {
        return _craftableItemsList[index].GetItemName();
    }

    public int GetLevelRequirement(int index) 
    {
        return _craftableItemsList[index].GetLevelRequirement();
    }

    public string GetCategory(int index) 
    { 
        return _craftableItemsList[index].GetCategory();
    }

    public MaterialRequirement[] GetMaterialRequirementsList(int index)
    {
        return _craftableItemsList[index].GetMaterialRequirementsList();
    }

    public string GetMaterialRequirement(int index, int materialIndex)
    {
        return _craftableItemsList[index].GetMaterialRequirement(materialIndex);
    }

    public int GetMaterialRequirementQuantity(int index, int materialIndex)
    {
        return _craftableItemsList[index].GetMaterialRequirementQuantity(materialIndex);
    }

    public Sprite GetMaterialRequirementIcon(int index, int materialIndex)
    {
        return _craftableItemsList[index].GetMaterialRequirementIcon(materialIndex);
    }

    public Sprite GetItemIcon(int index)
    {
        return _craftableItemsList[index].GetItemIcon();
    }

    public string GetCraftedItem(int index)
    {

        return _craftableItemsList[index].GetCraftedItem();
    }

    public int GetCraftableListLength()
    {
        return _craftableItemsList.Length;
    }

}

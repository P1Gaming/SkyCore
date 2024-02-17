using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingUICode : MonoBehaviour
{
    [SerializeField]
    private GameObject _craftItemDataGameObject; // Database for storing recipe data
    private CraftingRecipeData _craftItemDataScript; // Reference to script for recipe database
    [SerializeField]
    private GameObject _recipeListItemPrefab; // Prefab for recipes
    private CraftingRecipeData.MaterialRequirement[] _emptyArray = new CraftingRecipeData.MaterialRequirement[] { };
    private int[] _inventoryMaterials = new int[] { 0, 0, 0, 0 };
    private CraftingRecipeData.MaterialRequirement[] _currentMaterialsDisplayed;
    private int _materialAmountMultiplier; // Increase multiplier for each item being crafted
    private ScrollRect _scrollObject;
    private TextMeshProUGUI _quantityAmount;
 
    //Wwise 
    bool _craftingSuccess = false;
    //[Header("Crafting UI Sounds")]
    //public AK.Wwise.Event UpgradeButtonClick;
    //public AK.Wwise.Event CraftSuccessButtonClick;
    //public AK.Wwise.Event CraftNotSuccessButtonClick;
    //public AK.Wwise.Event SortButtonClick;
    //public AK.Wwise.Event LeftArrowClick;
    //public AK.Wwise.Event RightArrowClick;
    //public AK.Wwise.Event ListOfItemsSort;


    void Start()
    {
        _currentMaterialsDisplayed = _emptyArray;
        _craftItemDataScript = _craftItemDataGameObject.GetComponent<CraftingRecipeData>();
        _scrollObject = transform.Find("Scroll View").GetComponent<ScrollRect>();
        _scrollObject.normalizedPosition = new Vector2(0, 1);
        _quantityAmount = transform.Find("Page switch").Find("page number text").GetComponent<TextMeshProUGUI>();
        ResetMaterialMultiplier();
        AddItemsToList("All");
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// This will iterate through the CrafItemData prefab's database, instantiate a recipe prefab for each 
    /// craftable item with an icon and name, then add it to ScrollView in the crafting menu. It also 
    /// updates the data in the RecipeListItemScript with data from the database for storage in each 
    /// recipe prefab. 
    /// This will take in a string for the category and only add matching category items
    /// </summary>
    public void AddItemsToList(string category)
    {
        Transform ScrollViewContent = null;

        // Iterate through DB of stored recipes.
        for (int i = 0; i < _craftItemDataScript.GetCraftableListLength(); i++)
        {
            //If the level of the recipe trying to add is greater than the current crafting level, should break out
            if (_craftItemDataScript.GetLevelRequirement(i) > _craftItemDataGameObject.GetComponent<CraftingLevelManager>().CraftingLevel)
            {
                break;
            }
            //This filters for the category
            if (!category.Equals("All") && !category.Equals(_craftItemDataScript.GetCategory(i)))
            {
                continue;
            }
            GameObject InstancedRecipePrefab = Instantiate(_recipeListItemPrefab);

            ScrollViewContent = transform.Find("Scroll View").Find("Viewport").Find("Content");
            InstancedRecipePrefab.transform.SetParent(ScrollViewContent);

            // Access script of instantiated prefab
            RecipeListItemScript InstancedPrefabScript = InstancedRecipePrefab
                .GetComponent<RecipeListItemScript>();

            // Recipe prefab button
            Button button = InstancedRecipePrefab.GetComponent<Button>();

            // Add a listener to the button's onClick event
            button.onClick.AddListener(() => OnCraftItemClick(InstancedRecipePrefab
                .GetComponent<RecipeListItemScript>()));

            // Resize recipe to the correct aspect ratio
            // RectTransform buttonTransform = InstancedRecipePrefab.GetComponent<RectTransform>();
            // buttonTransform.anchorMin = new Vector2(0f, 0.75f);
            // buttonTransform.anchorMax = new Vector2(0.5f, 1f);

            if (InstancedPrefabScript != null)
            {
                // The following stores item data in RecipeListItem prefab instances
                InstancedPrefabScript.SetName(_craftItemDataScript.GetItemName(i));
                InstancedPrefabScript.SetLevel(_craftItemDataScript.GetLevelRequirement(i));
                InstancedPrefabScript.SetCategory(_craftItemDataScript.GetCategory(i));
                InstancedPrefabScript.SetMaterials(_craftItemDataScript.GetMaterialRequirementsList(i));
                InstancedPrefabScript.SetIcon(_craftItemDataScript.GetItemIcon(i));
                InstancedPrefabScript.SetItem(_craftItemDataScript.GetCraftedItem(i));

                // set image in ScrollView
                Image RecipeIcon = InstancedRecipePrefab.transform.Find("Icon").GetComponent<Image>();
                RecipeIcon.sprite = InstancedPrefabScript.GetIcon();

                //set name in ScrollView
                GameObject RecipeName = InstancedRecipePrefab.transform.Find("Name").gameObject;
                RecipeName.GetComponent<TextMeshProUGUI>().text = InstancedPrefabScript.GetName();

                InstancedPrefabScript.transform.localScale = Vector3.one;

            }
            else
            {
                Debug.LogWarning("PrefabData component not found on instantiated prefab");
            }

            //Update initial recipe UI
            if (i == 0)
            {
                //_initialRecipe = InstancedPrefabScript;
                //UpdateRightUI(_initialRecipe.GetName(), _initialRecipe.GetIcon(),
                //    _initialRecipe.GetMaterials(), _inventoryMaterials, _materialAmountMultiplier);
                UpdateRightUI("", null, _emptyArray, _inventoryMaterials);
            }
        }
    }

    /// <summary>
    /// This is needed to clear the current list being displayed
    /// With the list cleared, we can then reconstruct a new list
    /// </summary>
    public void ClearList()
    {
        Transform ScrollViewContent = transform.Find("Scroll View").Find("Viewport").Find("Content");
        foreach (Transform child in ScrollViewContent)
        {
            Destroy(child.gameObject);
        }

        ResetMaterialMultiplier(); 
    }

    public void OnUpgradeButtonClick()
    {
        // Handle the click event for the upgrade button
        Debug.Log("Upgrade button clicked!");
        //UpgradeButtonClick.Post(gameObject);
        _craftItemDataGameObject.GetComponent<CraftingLevelManager>().CraftingLevel++;
        ClearList();
        AddItemsToList("All");
    }
    public void OnCraftButtonClick()
    {
        // Handle the click event for the craft button
        Debug.Log("Craft button clicked!");
        _craftingSuccess = UnityEngine.Random.value > 0.5f;

        if (_craftingSuccess == true)
        {
            //CraftSuccessButtonClick.Post(gameObject);
        }
        else
        {
           // CraftNotSuccessButtonClick.Post(gameObject);
        }
    }

    public void OnListOfItemsSort()
    {
        // Handle the list event for sorting list of items
        Debug.Log("List Of Items Sort!");
        //ListOfItemsSort.Post(gameObject);
    }
    public void OnQuantityUpButtonClick()
    {
        // Handle the click event for the right arrow button
        Debug.Log("Right/Increase Arrowclicked!");
        //RightArrowClick.Post(gameObject);

        AdjustMaterialMultiplier(1);
    }

    public void OnQuantityDownButtonClick()
    {
        // Handle the click event for the left arrow button
        Debug.Log("Left/Decrease Arrow clicked!");
        //LeftArrowClick.Post(gameObject);
        
        AdjustMaterialMultiplier(-1);
    }

    /// <summary>
    /// Increment or decrement the material multiplier.
    /// Update the quantity amount text with multiplier.
    /// Iterate through displayed materials and display initial amount multiplied by quantity.
    /// </summary>
    /// <param name="num">1 for incrementing, -1 for decrementing multiplier.</param>
    public void AdjustMaterialMultiplier(int num)
    {
        TextMeshProUGUI tempMaterialText;
        if (_materialAmountMultiplier + num > 0 && _materialAmountMultiplier + num < 100)
        {
            _materialAmountMultiplier += num;

            //update material amount text
            _quantityAmount.text = "" + _materialAmountMultiplier;
            for (int i = 0; i < _currentMaterialsDisplayed.Length; i++)
            {
                tempMaterialText = (TextMeshProUGUI)GetMaterialComponent(i, "Amount");
                tempMaterialText.text = "" + _inventoryMaterials[i] + "/" +
                    (_currentMaterialsDisplayed[i].GetQuantity() * _materialAmountMultiplier);
            }
            tempMaterialText = null;
        }
    }

    public void ResetMaterialMultiplier()
    {
        _materialAmountMultiplier = 1;
        _quantityAmount.text = "" + _materialAmountMultiplier;
    }

    /// <summary>
    /// When a recipe is clicked, it updates the UI on the right-hand side with the following:
    /// The crafted item name, the crafted item icon, the material icons, the material quantities, 
    /// and material names. If the recipe requires less than 4 different types of materials, the 
    /// remaining material boxes are hidden.
    /// </summary>
    /// <param name="recipePrefabScript">An instanced prefab for the recipe button in the ScrollView.</param>
    public void OnCraftItemClick(RecipeListItemScript recipePrefabScript)
    {
        // Handle the click event for the craftable item
        Debug.Log("Craftable item clicked!");

        ResetMaterialMultiplier();
        UpdateRightUI(recipePrefabScript.GetName(), recipePrefabScript.GetIcon(),
            recipePrefabScript.GetMaterials(), _inventoryMaterials);
        _currentMaterialsDisplayed = recipePrefabScript.GetMaterials();
    }

    /// <summary>
    /// Returns the Component of the crafting UI's right hand side for a material's image, amount
    /// required,  or name for editting. Requires casting to its type after the method is called 
    /// [Image, TextMeshProUGUI, TextMeshProUGUI].
    /// </summary>
    /// <param name="materialIndex">Index for the material of the recipe for which this is attached 
    ///     [1, 2, 3, 4].</param>
    /// <param name="compName">String of the Component being retrieved ["Image", "Amount", "Name"].</param>
    /// <returns></returns>
    private Component GetMaterialComponent(int materialIndex, string compName)
    {
        Component matComponent = null;

        Transform materialTemp = transform.Find("materials images").Find("Image mat " +
            (materialIndex + 1)).Find("Material Display");

        if (compName == "Image")
        {
            matComponent = materialTemp.Find("Image").GetComponent<Image>();
        }
        else if (compName == "Amount")
        {
            matComponent = materialTemp.Find("Amount").GetComponent<TextMeshProUGUI>();
        }
        else if (compName == "Name")
        {
            matComponent = materialTemp.Find("Name").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            matComponent = materialTemp;
        }
        return matComponent;
    }

    /// <summary>
    /// Updates the right side of the crafting UI with data passed in.
    /// </summary>
    /// <param name="craftedName">Name of item being crafted.</param>
    /// <param name="craftedIcon">Icon of item being crafted.</param>
    /// <param name="craftedMaterials">List of materials needed for crafting.</param>
    /// <param name="inventoryMaterialAmount">List of specific inventory materials.</param>
    /// <param name="itemAmountMultiplier">Amount of item being crafted.</param>
    private void UpdateRightUI(string craftedName, Sprite craftedIcon, CraftingRecipeData
        .MaterialRequirement[] craftedMaterials, int[] inventoryMaterialAmount)
    {
        Component materialImageTemp = null;
        Component materialAmountTemp = null;
        Component materialNameTemp = null;
        Image materialImageComp;
        TextMeshProUGUI materialAmountComp;
        TextMeshProUGUI materialNameComp;

        TextMeshProUGUI itemName = transform.Find("materials images").Find("Item craft image")
            .Find("Name").GetComponent<TextMeshProUGUI>();

        Image itemImage = transform.Find("materials images").Find("Item craft image")
            .Find("Item Crafted").GetComponent<Image>();

        itemName.text = craftedName;
        itemImage.sprite = craftedIcon;

        for (int i = 0; i < 4; i++)
        {
            // Next 3 lines retrieve Components required for changing material display
            materialImageTemp = GetMaterialComponent(i, "Image");
            materialAmountTemp = GetMaterialComponent(i, "Amount");
            materialNameTemp = GetMaterialComponent(i, "Name");


            // if the recipe has more materials to display, update UI with data
            if (i < craftedMaterials.Length)
            {
                // if valid
                if (materialImageTemp is Image && materialAmountTemp is TextMeshProUGUI
                    && materialNameTemp is TextMeshProUGUI)
                {
                    // Show material
                    GetMaterialComponent(i, "").gameObject.SetActive(true);

                    // Next 3 lines are for casting to appropriate Component type
                    materialImageComp = (Image)materialImageTemp;
                    materialAmountComp = (TextMeshProUGUI)materialAmountTemp;
                    materialNameComp = (TextMeshProUGUI)materialNameTemp;

                    //change material icon
                    materialImageComp.sprite = craftedMaterials[i].GetIcon();

                    //change material requirement amount
                    materialAmountComp.text = "" + inventoryMaterialAmount[i] + "/" +
                        (craftedMaterials[i].GetQuantity() * _materialAmountMultiplier);

                    //change material name
                    materialNameComp.text = craftedMaterials[i].GetMaterial();

                }
                else
                {
                    Debug.LogError("Retrieved component is not of correct type.");
                }

            }
            else
            {
                // Hide material
                GetMaterialComponent(i, "").gameObject.SetActive(false);
            }

        }
    }

    /// <summary>
    /// When crafting UI is hidden, set the right display to first recipe.
    /// </summary>
    public void OnClose()
    {
        UpdateRightUI("", null, _emptyArray, _inventoryMaterials);
        _scrollObject.normalizedPosition = new Vector2(0, 1);
        ResetMaterialMultiplier();
        _currentMaterialsDisplayed = _emptyArray;
    }

    
}

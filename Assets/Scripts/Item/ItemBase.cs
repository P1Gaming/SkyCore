using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the base for an item, an abstract scriptable object which should be used to create other 
/// scriptable object item types. Objects should not be created from this.
/// </summary>
public abstract class ItemBase:ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private int _maxStack;
    [SerializeField] private bool _isDroppable;
    [SerializeField] private bool _isStackable;
    
    [SerializeField] private ItemSortType _sortType;

    [SerializeField, Tooltip("Saturation value to be added to jelly after feeding")] 
    private int _saturationValue = 5;
    [field: SerializeField]
    public List<ItemAttribute> Attributes { get; private set; }

    public enum ItemSortType
    { 
        None,
        Tool,
        JellyItem,
        Resource
    }

    public int ID { get => _id; private set => _id = value; }
    public string Name { get => _name; private set => _name = value; }
    public Sprite Icon { get => _icon; private set => _icon = value; }
    public GameObject ItemPrefab { get => _itemPrefab; private set => _itemPrefab = value; }
    public int MaxStack { get => _maxStack; private set => _maxStack = value; }
    public bool IsDroppable { get => _isDroppable; private set => _isDroppable = value; }
    public bool IsStackable { get => _isStackable; private set => _isStackable = value; }
    public ItemSortType SortType { get => _sortType; private set => _sortType = value; }
    public int SaturationValue { get => _saturationValue; private set => _saturationValue = value; }

    public virtual void OnItemEquipped() { }
    public virtual void OnItemUnequip() { }
    public virtual void OnItemPickup() { }
    public virtual void OnItemDrop() { }
    public virtual void OnItemLeftClick() { }
    public virtual void OnItemRightClick() { }
    public virtual void OnItemRightClick(GameObject gameObject) { }
}

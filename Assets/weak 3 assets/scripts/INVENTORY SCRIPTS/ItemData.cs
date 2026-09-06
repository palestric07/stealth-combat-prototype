using UnityEngine;

public enum ItemType
{
    Battery,
    CopperWire,
    EMPCore,
    EMPGrenade,
    SignalJammer,
    Medkit,
    FuelCanister, 
    FoodPack 
}

[CreateAssetMenu(fileName = "NewItemData", menuName = "Survival System/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    [Tooltip("Unique ID used for saving, loading, and database references.")]
    public string itemID;
    public string itemName;
    public ItemType itemType;
    public Sprite icon;

    [Header("Settings")]
    [Min(1)] 
    public int maxStackSize = 99;

    [TextArea(3, 5)]
    public string description;

    // Quick helper check for inventory logic
    public bool IsStackable => maxStackSize > 1;
}
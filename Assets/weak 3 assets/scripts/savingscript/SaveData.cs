using System;
using System.Collections.Generic;

/// <summary>
/// Serializable data structure representing a single saved inventory item and its quantity.
/// </summary>
[Serializable]
public class InventoryItemSaveData
{
    public string itemName;
    public int amount;
}

/// <summary>
/// Serializable data structure holding player position, health, and inventory save states.
/// </summary>
[Serializable]
public class SaveData
{
    public float positionX;
    public float positionY;
    public float positionZ;
    public float currentHealth;
    public List<InventoryItemSaveData> inventoryItems = new List<InventoryItemSaveData>();
}
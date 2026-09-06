using System;

/// <summary>
/// Represents an individual inventory slot holding an item and its quantity.
/// </summary>
[Serializable]
public class InventorySlot
{
    public ItemData item;
    public int quantity;

    /// <summary>
    /// Initializes a new instance of the InventorySlot class with an item and quantity.
    /// </summary>
    /// <param name="newItem">The item assigned to this slot.</param>
    /// <param name="initialQuantity">The initial stack amount.</param>
    public InventorySlot(ItemData newItem, int initialQuantity)
    {
        item = newItem;
        quantity = initialQuantity;
    }
}
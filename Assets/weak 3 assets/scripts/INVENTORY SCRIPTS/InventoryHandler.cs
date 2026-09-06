using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player inventory slots, adding items, removing items, and checking quantities.
/// </summary>
public class InventoryHandler : MonoBehaviour
{
    public int maxSlots = 12;
    public List<InventorySlot> slots = new List<InventorySlot>();

    public event Action OnInventoryChanged;

    /// <summary>
    /// Adds specified quantity of an item to inventory slots.
    /// </summary>
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        foreach (var slot in slots)
        {
            if (slot.item == item && slot.quantity < item.maxStackSize)
            {
                int space = item.maxStackSize - slot.quantity;
                int addAmount = Mathf.Min(amount, space);
                slot.quantity += addAmount;
                amount -= addAmount;

                if (amount <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        while (amount > 0 && slots.Count < maxSlots)
        {
            int addAmount = Mathf.Min(amount, item.maxStackSize);
            slots.Add(new InventorySlot(item, addAmount));
            amount -= addAmount;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Checks if inventory contains required amount of an item.
    /// </summary>
    public bool HasItem(ItemData item, int amount)
    {
        int count = 0;
        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                count += slot.quantity;
                if (count >= amount) return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Removes specified amount of an item from inventory.
    /// </summary>
    public void RemoveItem(ItemData item, int amount)
    {
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].item == item)
            {
                if (slots[i].quantity > amount)
                {
                    slots[i].quantity -= amount;
                    OnInventoryChanged?.Invoke();
                    return;
                }
                else
                {
                    amount -= slots[i].quantity;
                    slots.RemoveAt(i);
                    if (amount <= 0) break;
                }
            }
        }
        OnInventoryChanged?.Invoke();
    }
}
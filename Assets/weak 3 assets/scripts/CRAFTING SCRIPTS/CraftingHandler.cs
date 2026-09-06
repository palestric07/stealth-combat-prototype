using UnityEngine;

/// <summary>
/// Handles checking crafting requirements and processing item creation using inventory items.
/// </summary>
public class CraftingHandler : MonoBehaviour
{
    public InventoryHandler inventory;

    /// <summary>
    /// Checks if the player has all necessary ingredients in their inventory to craft a recipe.
    /// </summary>
    /// <param name="recipe">The crafting recipe to check.</param>
    /// <returns>True if all ingredients are present, false otherwise.</returns>
    public bool CanCraft(CraftingRecipe recipe)
    {
        if (recipe == null || inventory == null) return false;

        foreach (var ingredient in recipe.ingredients)
        {
            if (!inventory.HasItem(ingredient.item, ingredient.amount))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Crafts an item by removing required ingredients from inventory and adding the resulting item.
    /// </summary>
    /// <param name="recipe">The crafting recipe to process.</param>
    /// <returns>True if crafting was successful, false otherwise.</returns>
    public bool CraftItem(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe)) return false;

        foreach (var ingredient in recipe.ingredients)
        {
            inventory.RemoveItem(ingredient.item, ingredient.amount);
        }

        inventory.AddItem(recipe.resultItem, recipe.resultAmount);
        return true;
    }
}
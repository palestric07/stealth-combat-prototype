using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a single required ingredient item and its quantity for crafting.
/// </summary>
[Serializable]
public class ResourceIngredient
{
    public ItemData item;
    public int amount = 1;
}

/// <summary>
/// ScriptableObject defining crafting recipes, required ingredients, and output items.
/// </summary>
[CreateAssetMenu(fileName = "NewCraftingRecipe", menuName = "Survival System/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    public string recipeName;
    public ItemData resultItem;
    public int resultAmount = 1;
    public List<ResourceIngredient> ingredients = new List<ResourceIngredient>();
}
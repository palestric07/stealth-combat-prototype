using UnityEngine;

/// <summary>
/// Helper component for testing crafting recipes using keyboard shortcuts.
/// </summary>
public class CraftingTester : MonoBehaviour
{
    public CraftingHandler craftingHandler;

    public CraftingRecipe empGrenadeRecipe;
    public CraftingRecipe signalJammerRecipe;
    public CraftingRecipe medkitRecipe;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryCraft(empGrenadeRecipe);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TryCraft(signalJammerRecipe);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TryCraft(medkitRecipe);
        }
    }

    /// <summary>
    /// Attempts to craft the specified recipe through the CraftingHandler.
    /// </summary>
    /// <param name="recipe">The crafting recipe to evaluate and process.</param>
    private void TryCraft(CraftingRecipe recipe)
    {
        if (craftingHandler == null || recipe == null) return;

        if (craftingHandler.CanCraft(recipe))
        {
            craftingHandler.CraftItem(recipe);
            Debug.Log("Successfully crafted: " + recipe.recipeName);
        }
        else
        {
            Debug.LogWarning("Cannot craft " + recipe.recipeName + "! Missing required ingredients.");
        }
    }
}
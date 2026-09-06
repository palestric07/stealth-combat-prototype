using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls an individual UI inventory slot, updating or clearing its icon and quantity display.
/// </summary>
public class UI_InventorySlot : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI quantityText;

    /// <summary>
    /// Updates the slot UI with the specified item icon sprite and stack quantity.
    /// </summary>
    /// <param name="icon">The item sprite icon to display.</param>
    /// <param name="quantity">The stack size of the item.</param>
    public void UpdateSlot(Sprite icon, int quantity)
    {
        if (icon != null)
        {
            iconImage.sprite = icon;
            iconImage.enabled = true;
            quantityText.text = quantity > 1 ? quantity.ToString() : "";
        }
        else
        {
            ClearSlot();
        }
    }

    /// <summary>
    /// Clears the slot icon display and resets the quantity text display.
    /// </summary>
    public void ClearSlot()
    {
        iconImage.sprite = null;
        iconImage.enabled = false;
        quantityText.text = "";
    }
}
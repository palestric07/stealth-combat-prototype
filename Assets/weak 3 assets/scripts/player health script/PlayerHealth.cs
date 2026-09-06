using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player health, taking damage, healing via medkits, and updating the health UI slider.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public Slider healthSlider;
    public InventoryHandler inventory;
    public ItemData medkitData;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
        }
        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            UseMedkit();
        }
    }

    /// <summary>
    /// Reduces player health by the specified amount and updates the health UI.
    /// </summary>
    /// <param name="amount">The amount of damage to inflict.</param>
    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateUI();
    }

    /// <summary>
    /// Consumes a medkit item from the player inventory to restore health if needed.
    /// </summary>
    public void UseMedkit()
    {
        if (inventory == null || medkitData == null) return;

        if (inventory.HasItem(medkitData, 1))
        {
            if (currentHealth < maxHealth)
            {
                inventory.RemoveItem(medkitData, 1);
                Heal(30f);
            }
        }
    }

    /// <summary>
    /// Restores player health by the specified amount and updates the health UI.
    /// </summary>
    /// <param name="amount">The amount of health to restore.</param>
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }
    }
}
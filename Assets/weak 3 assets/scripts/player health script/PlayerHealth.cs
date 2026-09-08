using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public Slider healthSlider;
    public InventoryHandler inventory;
    public ItemData medkitData;

    // IDamageable Properties
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0f;

    public event Action OnDeath;

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

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateUI();

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        OnDeath?.Invoke();
    }

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
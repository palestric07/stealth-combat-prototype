using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 500f;
    [SerializeField] private float currentHealth;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private GameObject healthBarCanvas;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0f;

    public event Action OnDeath;
    public event Action OnHalfHealth;

    private bool hasTriggeredHalfHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        if (healthSlider != null) healthSlider.value = currentHealth;
        if (!hasTriggeredHalfHealth && currentHealth <= maxHealth * 0.5f && currentHealth > 0f)
        {
            hasTriggeredHalfHealth = true;
            OnHalfHealth?.Invoke();
        }
        if (currentHealth <= 0f) Die();
    }

    private void Die()
    {
        if (healthBarCanvas != null) healthBarCanvas.SetActive(false);
        OnDeath?.Invoke();
    }
}
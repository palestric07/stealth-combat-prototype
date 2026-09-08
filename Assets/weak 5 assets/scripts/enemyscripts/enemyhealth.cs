using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0f;

    public event Action OnDeath;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);
        Debug.Log($"{gameObject.name} took {amount} damage! Current HP: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
}
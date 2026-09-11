using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 500f;
    [SerializeField] private float currentHealth;

    [Header("UI Setup")]
    [Tooltip("Enemy ke canvas ke andar jo slider hai wo yahan lagao")]
    [SerializeField] private Slider healthSlider;
    [Tooltip("Enemy ka poora health canvas yahan drag karo taake death par hide ho sake")]
    [SerializeField] private GameObject healthBarCanvas;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0f;

    public event Action OnDeath;
    public event Action OnHalfHealth; // 50% threshold event

    private bool hasTriggeredHalfHealth = false;

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

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Health UI Slider Update
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        Debug.Log($"Enemy took {amount} damage! Current HP: {currentHealth}/{maxHealth}");

        // 50% health check (sirf aik baar trigger hoga)
        if (!hasTriggeredHalfHealth && currentHealth <= (maxHealth * 0.5f) && currentHealth > 0f)
        {
            hasTriggeredHalfHealth = true;
            OnHalfHealth?.Invoke();
        }

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy has died!");

        // Marne par sar ke upar se health bar gayab kar do
        if (healthBarCanvas != null)
        {
            healthBarCanvas.SetActive(false);
        }

        OnDeath?.Invoke();
    }
}
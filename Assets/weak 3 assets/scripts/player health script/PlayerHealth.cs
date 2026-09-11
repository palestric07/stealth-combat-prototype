using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Hit Reaction Settings")]
    [Tooltip("Dard ki animation chalne ka chance (0.4 = 40% chance)")]
    [Range(0f, 1f)]
    [SerializeField] private float hitReactionChance = 0.4f;

    [Header("UI References")]
    public Slider healthSlider;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => currentHealth <= 0f;

    public event Action OnDeath;

    private Animator animator;
    private CharacterController characterController;
    private Collider playerCollider;
    private PlayerCombat playerCombat;
    private PlayerAudioController playerAudio;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        playerCollider = GetComponent<Collider>();
        playerCombat = GetComponent<PlayerCombat>();
        playerAudio = GetComponent<PlayerAudioController>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
        }
        UpdateUI();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        UpdateUI();

        // Damage Sound Play Karna
        if (playerAudio != null)
        {
            playerAudio.PlayDamageSound();
        }

        // Agar health zero ho jaye to Death
        if (currentHealth <= 0f)
        {
            Die();
            return;
        }

        // Kabhi kabhi hit animation chalana (agar player us waqt attack na kar raha ho)
        bool isAttacking = (playerCombat != null && playerCombat.IsAttacking);
        if (!isAttacking && UnityEngine.Random.value < hitReactionChance)
        {
            if (animator != null)
            {
                animator.SetTrigger("Hit");
            }
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        OnDeath?.Invoke();

        // 1. Death Animation Play
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 2. Combat Disable
        if (playerCombat != null)
        {
            playerCombat.enabled = false;
        }

        // 3. Movement Scripts Disable
        MonoBehaviour movementScript = GetComponent("PlayerMovement") as MonoBehaviour;
        if (movementScript != null)
        {
            movementScript.enabled = false;
        }

        // 4. CharacterController & Colliders Disable (taake dead body se takkar na ho)
        if (characterController != null)
        {
            characterController.enabled = false;
        }

        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
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
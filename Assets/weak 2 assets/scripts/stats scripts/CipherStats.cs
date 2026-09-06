using UnityEngine;

[CreateAssetMenu(fileName = "CipherStats", menuName = "Cipher/Player Stats")]
public class CipherStats : ScriptableObject
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float jumpHeight = 1.2f;

    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float staminaDrainRate = 15f;
    [SerializeField] private float staminaRegenRate = 10f;

    [SerializeField] private float stealthRating = 1.0f;

    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;
    public float CrouchSpeed => crouchSpeed;
    public float Gravity => gravity;
    public float JumpHeight => jumpHeight;
    public float MaxHealth => maxHealth;
    public float MaxStamina => maxStamina;
    public float StaminaDrainRate => staminaDrainRate;
    public float StaminaRegenRate => staminaRegenRate;
    public float StealthRating => stealthRating;
}
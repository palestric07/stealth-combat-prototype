using UnityEngine;

public class PlayerStatsAndState : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public float maxStamina = 100f;
    private float currentStamina;
    public float staminaDrainRate = 35f;
    public float staminaRegenRate = 15f;

    public float idleNoise = 0f;
    public float walkNoise = 30f;
    public float sprintNoise = 85f;
    private float currentNoise;

    private void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;

        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHealth(currentHealth, maxHealth);
            HUDManager.Instance.UpdateStamina(currentStamina, maxStamina);
        }
    }

    private void Update()
    {
        HandleStaminaAndNoise();

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(15f);
        }
    }

    private void HandleStaminaAndNoise()
    {
        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f;
        bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftShift) && currentStamina > 0;

        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentNoise = sprintNoise;
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
            }

            currentNoise = isMoving ? walkNoise : idleNoise;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateStamina(currentStamina, maxStamina);
            HUDManager.Instance.UpdateNoiseLevel(currentNoise);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth - amount, 0f, maxHealth);

        if (HUDManager.Instance != null)
        {
            HUDManager.Instance.UpdateHealth(currentHealth, maxHealth);
        }
    }
}
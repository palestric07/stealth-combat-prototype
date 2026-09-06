using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private PlayerMotor motor;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (motor == null)
        {
            motor = GetComponent<PlayerMotor>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (animator == null || motor == null) return;

        float speed = motor.currentSpeedPercent;
        animator.SetFloat("Speed", speed);
        animator.SetBool("IsCrouching", motor.isCrouching);

        if (Input.GetButtonDown("Jump") && motor.isGrounded && !motor.isCrouching)
        {
            animator.SetTrigger("Jump");
        }
    }
}
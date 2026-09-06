using System.Collections;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [SerializeField] private CipherStats stats;
    [SerializeField] private Transform cameraRoot;

    [SerializeField] private LayerMask vaultLayer;
    [SerializeField] private float vaultRayDistance = 1.2f;
    [SerializeField] private float vaultHeight = 1.2f;
    [SerializeField] private float rotationSpeed = 15f;

    private CharacterController controller;
    private Vector3 velocity;

    public bool isGrounded;
    public bool isCrouching;
    public bool isVaulting;
    public float currentSpeedPercent;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraRoot == null && Camera.main != null)
        {
            cameraRoot = Camera.main.transform;
        }
    }

    private void Update()
    {
        if (isVaulting) return;

        CheckGrounded();
        HandleMovement();
        HandleJump();
        HandleCrouch();
        HandleVault();
    }

    private void CheckGrounded()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    private void HandleMovement()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(x, 0f, z).normalized;

        if (dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg + cameraRoot.eulerAngles.y;
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, Time.deltaTime * rotationSpeed);

            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            float speed = stats.WalkSpeed;
            currentSpeedPercent = 0.5f;

            if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
            {
                speed = stats.SprintSpeed;
                currentSpeedPercent = 1f;
            }
            else if (isCrouching)
            {
                speed = stats.CrouchSpeed;
                currentSpeedPercent = 0.5f;
            }

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);
        }
        else
        {
            currentSpeedPercent = 0f;
        }

        velocity.y += stats.Gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(stats.JumpHeight * -2f * stats.Gravity);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
        }
    }

    private void HandleVault()
    {
        if (Input.GetKeyDown(KeyCode.E) && isGrounded && !isCrouching)
        {
            Vector3 lowerOrigin = transform.position + Vector3.up * 0.2f;
            Vector3 upperOrigin = transform.position + Vector3.up * vaultHeight;

            if (Physics.Raycast(lowerOrigin, transform.forward, vaultRayDistance, vaultLayer))
            {
                if (!Physics.Raycast(upperOrigin, transform.forward, vaultRayDistance, vaultLayer))
                {
                    StartCoroutine(VaultRoutine());
                }
            }
        }
    }

    private IEnumerator VaultRoutine()
    {
        isVaulting = true;
        Vector3 targetPos = transform.position + transform.forward * 1.5f + Vector3.up * 1f;
        Vector3 startPos = transform.position;

        float timer = 0f;
        float duration = 0.4f;

        while (timer < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;
        isVaulting = false;
    }
}
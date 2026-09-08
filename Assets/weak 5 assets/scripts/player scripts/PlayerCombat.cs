using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Configuration")]
    [Tooltip("Pancho ScriptableObjects (Attack_1 to Attack_5) sequence mein yahan assign karein")]
    [SerializeField] private List<AttackData> comboList = new List<AttackData>();
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;

    [Header("Combo Timing")]
    [SerializeField] private float comboResetTime = 1.0f;

    [Header("Audio Source (Optional)")]
    [SerializeField] private AudioSource audioSource;

    private Animator animator;
    private int currentComboIndex = 0;
    private bool isAttacking = false;
    private bool comboBuffered = false;
    private float lastAttackTime = 0f;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        HandleInput();
        CheckComboReset();
    }

    private void HandleInput()
    {
        // Left Mouse Button Attack
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                ExecuteAttack();
            }
            else
            {
                // Attack chal raha ho to agla punch buffer kar lo
                comboBuffered = true;
            }
        }
    }

    private void ExecuteAttack()
    {
        if (comboList.Count == 0) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        AttackData currentAttack = comboList[currentComboIndex];

        // Direct crossfade to state
        animator.CrossFade(currentAttack.animationStateName, currentAttack.transitionDuration);

        // Optional sound
        if (audioSource != null && currentAttack.attackSwingSound != null)
        {
            audioSource.PlayOneShot(currentAttack.attackSwingSound);
        }

        // Combo Index ko aage barhana
        currentComboIndex++;
        if (currentComboIndex >= comboList.Count)
        {
            currentComboIndex = 0; // Finisher ke baad wapis reset
        }
    }

    private void CheckComboReset()
    {
        if (!isAttacking && Time.time - lastAttackTime > comboResetTime)
        {
            currentComboIndex = 0;
        }
    }

    // --- ANIMATION EVENTS ---

    // 1. Combo window khulne par call hota hai
    public void OnComboWindow()
    {
        if (comboBuffered)
        {
            comboBuffered = false;
            ExecuteAttack();
        }
    }

    // 2. Punch/Kick ke exact hit frame par call hota hai
    public void OnHitCheck()
    {
        int attackIndex = (currentComboIndex == 0) ? comboList.Count - 1 : currentComboIndex - 1;
        AttackData currentAttack = comboList[attackIndex];

        // Aakhri attack finisher kick hai
        bool isFinisher = (attackIndex == comboList.Count - 1);

        Transform point = attackPoint != null ? attackPoint : transform;
        Collider[] hitEnemies = Physics.OverlapSphere(point.position, currentAttack.attackRange, enemyLayers);

        foreach (Collider hitCollider in hitEnemies)
        {
            // Health damage apply karna
            IDamageable target = hitCollider.GetComponentInParent<IDamageable>();
            if (target != null && !target.IsDead)
            {
                target.TakeDamage(currentAttack.damage);

                // Knockdown / flinch reaction trigger karna
                EnemyAI enemyAI = hitCollider.GetComponentInParent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.ReceiveHit(isFinisher);
                }
            }
        }
    }

    // 3. Attack animation end hone par call hota hai
    public void OnAttackEnd()
    {
        isAttacking = false;
        comboBuffered = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, 1.5f);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Configuration")]
    [Tooltip("Pancho ScriptableObjects (Attack_1 to Attack_5) sequence mein yahan assign karein")]
    [SerializeField] private List<AttackData> comboList = new List<AttackData>();
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;

    [Header("VFX Prefabs (Only Spawns on Hit)")]
    [Tooltip("Punch impact / hit spark effect")]
    [SerializeField] private GameObject hitVFXPrefab;
    [Tooltip("Blood splash / splatter effect")]
    [SerializeField] private GameObject bloodVFXPrefab;

    [Header("Attack Speed & Feel")]
    [Range(0.8f, 3f)]
    [SerializeField] private float attackSpeedMultiplier = 1.4f;
    [Tooltip("Punch lagte waqt micro-freeze duration")]
    [SerializeField] private float hitStopDuration = 0.07f;

    [Header("Combo Timing")]
    [SerializeField] private float comboResetTime = 1.0f;

    [Header("Audio Source & Volume")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] [Range(0f, 1f)] private float swingVolume = 0.4f;
    [SerializeField] [Range(0f, 1f)] private float hitVolume = 1.0f;

    private Animator animator;
    private int currentComboIndex = 0;
    private bool isAttacking = false;
    private bool comboBuffered = false;
    private float lastAttackTime = 0f;
    private Coroutine hitStopCoroutine;

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

        // Safety fallback agar animation event miss ho jaye
        if (isAttacking && Time.time - lastAttackTime > 1.2f)
        {
            ResetCombatState();
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isAttacking)
            {
                ExecuteAttack();
            }
            else
            {
                comboBuffered = true;
            }
        }
    }

    private void ExecuteAttack()
    {
        if (comboList.Count == 0) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        animator.speed = attackSpeedMultiplier;

        AttackData currentAttack = comboList[currentComboIndex];

        // Swing sound
        if (audioSource != null && currentAttack.attackSwingSound != null)
        {
            audioSource.PlayOneShot(currentAttack.attackSwingSound, swingVolume);
        }

        animator.CrossFade(currentAttack.animationStateName, currentAttack.transitionDuration);

        currentComboIndex++;
        if (currentComboIndex >= comboList.Count)
        {
            currentComboIndex = 0;
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

    public void OnComboWindow()
    {
        if (comboBuffered)
        {
            comboBuffered = false;
            ExecuteAttack();
        }
    }

    public void OnHitCheck()
    {
        int attackIndex = (currentComboIndex == 0) ? comboList.Count - 1 : currentComboIndex - 1;
        AttackData currentAttack = comboList[attackIndex];

        bool isFinisher = (attackIndex == comboList.Count - 1);

        Transform point = attackPoint != null ? attackPoint : transform;
        Collider[] hitEnemies = Physics.OverlapSphere(point.position, currentAttack.attackRange, enemyLayers);

        Collider bestTarget = null;
        float closestDistance = float.MaxValue;

        // Sirf samne aur sab se qareeb aik dushman dhoondo
        foreach (Collider hitCollider in hitEnemies)
        {
            Vector3 directionToEnemy = (hitCollider.transform.position - transform.position).normalized;
            directionToEnemy.y = 0;

            float dotProduct = Vector3.Dot(transform.forward, directionToEnemy);

            if (dotProduct > 0.4f)
            {
                IDamageable damageable = hitCollider.GetComponentInParent<IDamageable>();
                if (damageable != null && !damageable.IsDead)
                {
                    float dist = Vector3.Distance(transform.position, hitCollider.transform.position);
                    if (dist < closestDistance)
                    {
                        closestDistance = dist;
                        bestTarget = hitCollider;
                    }
                }
            }
        }

        // Agar dushman ko hit laga to hi sab chalega (Damage, Sound, VFX, Blood)
        if (bestTarget != null)
        {
            IDamageable target = bestTarget.GetComponentInParent<IDamageable>();
            if (target != null && !target.IsDead)
            {
                target.TakeDamage(currentAttack.damage);

                // Exact impact position (dushman ke body par jahan punch takraya)
                Vector3 impactPoint = bestTarget.ClosestPoint(point.position);
                // Thora chest level par elevate karne ke liye safety offset
                if (impactPoint == Vector3.zero) impactPoint = bestTarget.bounds.center;

                // 1. Hit VFX Spawn (Agar assigned hai)
                if (hitVFXPrefab != null)
                {
                    GameObject hitVFX = Instantiate(hitVFXPrefab, impactPoint, Quaternion.identity);
                    Destroy(hitVFX, 1.5f); // 1.5 sec baad clean up
                }

                // 2. Blood VFX Spawn (Agar assigned hai)
                if (bloodVFXPrefab != null)
                {
                    // Blood ko punch ki direction mein aage ki taraf spray karne ke liye rotation
                    Quaternion bloodRotation = Quaternion.LookRotation(transform.forward);
                    GameObject bloodVFX = Instantiate(bloodVFXPrefab, impactPoint, bloodRotation);
                    Destroy(bloodVFX, 2.0f); // 2 sec baad clean up
                }

                // 3. Audio Playback
                if (audioSource != null && currentAttack.hitImpactSound != null)
                {
                    audioSource.PlayOneShot(currentAttack.hitImpactSound, hitVolume);
                }

                // 4. Hit-Stop
                if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
                hitStopCoroutine = StartCoroutine(HitStopRoutine(hitStopDuration));

                // 5. Enemy AI Reaction
                EnemyAI enemyAI = bestTarget.GetComponentInParent<EnemyAI>();
                if (enemyAI != null)
                {
                    enemyAI.ReceiveHit(isFinisher);
                }
            }
        }
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        animator.speed = 0.05f;
        yield return new WaitForSeconds(duration);
        animator.speed = attackSpeedMultiplier;
    }

    public void OnAttackEnd()
    {
        ResetCombatState();
    }

    private void ResetCombatState()
    {
        isAttacking = false;
        comboBuffered = false;
        if (animator != null) animator.speed = 1f;
    }

    private void OnDisable()
    {
        ResetCombatState();
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, 1.5f);
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private List<AttackData> comboList = new List<AttackData>();
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private GameObject hitVFXPrefab;
    [SerializeField] private GameObject bloodVFXPrefab;
    [Range(0.8f, 3f)] [SerializeField] private float attackSpeedMultiplier = 1.4f;
    [SerializeField] private float hitStopDuration = 0.07f;
    [SerializeField] private float comboResetTime = 1.0f;
    [SerializeField] private AudioSource audioSource;
    [Range(0f, 1f)] [SerializeField] private float swingVolume = 0.4f;
    [Range(0f, 1f)] [SerializeField] private float hitVolume = 1.0f;

    private Animator animator;
    private int currentComboIndex;
    private bool isAttacking;
    private bool comboBuffered;
    private float lastAttackTime;
    private Coroutine hitStopCoroutine;

    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        HandleInput();
        CheckComboReset();
        if (isAttacking && Time.time - lastAttackTime > 1.2f) ResetCombatState();
    }

    private void HandleInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (!isAttacking) ExecuteAttack();
        else comboBuffered = true;
    }

    private void ExecuteAttack()
    {
        if (comboList.Count == 0) return;
        isAttacking = true;
        lastAttackTime = Time.time;
        animator.speed = attackSpeedMultiplier;
        AttackData currentAttack = comboList[currentComboIndex];
        if (audioSource != null && currentAttack.attackSwingSound != null)
            audioSource.PlayOneShot(currentAttack.attackSwingSound, swingVolume);
        animator.CrossFade(currentAttack.animationStateName, currentAttack.transitionDuration);
        currentComboIndex = (currentComboIndex + 1) % comboList.Count;
    }

    private void CheckComboReset()
    {
        if (!isAttacking && Time.time - lastAttackTime > comboResetTime)
            currentComboIndex = 0;
    }

    public void OnComboWindow()
    {
        if (!comboBuffered) return;
        comboBuffered = false;
        ExecuteAttack();
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

        foreach (Collider hitCollider in hitEnemies)
        {
            Vector3 directionToEnemy = (hitCollider.transform.position - transform.position).normalized;
            directionToEnemy.y = 0;
            if (Vector3.Dot(transform.forward, directionToEnemy) > 0.4f)
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

        if (bestTarget == null) return;
        IDamageable target = bestTarget.GetComponentInParent<IDamageable>();
        if (target == null || target.IsDead) return;

        target.TakeDamage(currentAttack.damage);
        Vector3 impactPoint = bestTarget.ClosestPoint(point.position);
        if (impactPoint == Vector3.zero) impactPoint = bestTarget.bounds.center;

        if (hitVFXPrefab != null)
        {
            GameObject hitVFX = Instantiate(hitVFXPrefab, impactPoint, Quaternion.identity);
            Destroy(hitVFX, 1.5f);
        }
        if (bloodVFXPrefab != null)
        {
            GameObject bloodVFX = Instantiate(bloodVFXPrefab, impactPoint, Quaternion.LookRotation(transform.forward));
            Destroy(bloodVFX, 2.0f);
        }
        if (audioSource != null && currentAttack.hitImpactSound != null)
            audioSource.PlayOneShot(currentAttack.hitImpactSound, hitVolume);

        if (hitStopCoroutine != null) StopCoroutine(hitStopCoroutine);
        hitStopCoroutine = StartCoroutine(HitStopRoutine(hitStopDuration));

        EnemyAI enemyAI = bestTarget.GetComponentInParent<EnemyAI>();
        if (enemyAI != null) enemyAI.ReceiveHit(isFinisher);
    }

    private IEnumerator HitStopRoutine(float duration)
    {
        animator.speed = 0.05f;
        yield return new WaitForSeconds(duration);
        animator.speed = attackSpeedMultiplier;
    }

    public void OnAttackEnd() => ResetCombatState();

    private void ResetCombatState()
    {
        isAttacking = false;
        comboBuffered = false;
        if (animator != null) animator.speed = 1f;
    }

    private void OnDisable() => ResetCombatState();

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, 1.5f);
    }
}
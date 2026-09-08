using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    [Header("Animation Settings")]
    [Tooltip("Animator state ka exact naam jo trigger hoga")]
    public string animationStateName;
    
    [Tooltip("Previous state se is attack par transition ka time")]
    public float transitionDuration = 0.1f;

    [Header("Damage Settings")]
    [Tooltip("Is attack se kitna damage hoga")]
    public float damage = 20f;

    [Tooltip("Is attack ki reach / range")]
    public float attackRange = 1.5f;

    [Header("Audio Feedback")]
    public AudioClip attackSwingSound;
    public AudioClip hitImpactSound;
}
using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    public string animationStateName;
    public float transitionDuration = 0.1f;
    public float damage = 20f;
    public float attackRange = 1.5f;
    public AudioClip attackSwingSound;
    public AudioClip hitImpactSound;
}
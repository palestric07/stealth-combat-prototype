using UnityEngine;

public class PlayerEMPThrower : MonoBehaviour
{
    public GameObject empGrenadePrefab;
    public Transform throwPoint;
    public KeyCode throwKey = KeyCode.G;
    public Animator playerAnimator;
    public string throwAnimTrigger = "Throw";
    public float throwDelay = 0.3f;

    private void Update()
    {
        if (Input.GetKeyDown(throwKey))
        {
            TriggerThrow();
        }
    }

    private void TriggerThrow()
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetTrigger(throwAnimTrigger);
        }

        CancelInvoke(nameof(SpawnAndThrowGrenade));
        Invoke(nameof(SpawnAndThrowGrenade), throwDelay);
    }

    private void SpawnAndThrowGrenade()
    {
        if (empGrenadePrefab == null) return;

        Vector3 spawnPos = throwPoint != null 
            ? throwPoint.position 
            : (transform.position + transform.forward * 1.2f + Vector3.up * 1.2f);

        Quaternion spawnRot = throwPoint != null ? throwPoint.rotation : transform.rotation;

        GameObject grenadeObj = Instantiate(empGrenadePrefab, spawnPos, spawnRot);

        Collider[] playerColliders = GetComponentsInChildren<Collider>();
        Collider grenadeCollider = grenadeObj.GetComponent<Collider>();

        if (grenadeCollider != null)
        {
            foreach (var pCol in playerColliders)
            {
                Physics.IgnoreCollision(pCol, grenadeCollider, true);
            }
        }

        if (grenadeObj.TryGetComponent<EMPGrenade>(out var grenade))
        {
            Vector3 throwDirection = (transform.forward + Vector3.up * 0.25f).normalized;
            grenade.Launch(throwDirection);
        }
    }
}
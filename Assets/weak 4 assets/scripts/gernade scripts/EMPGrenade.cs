using UnityEngine;

public class EMPGrenade : MonoBehaviour
{
    public float blastRadius = 10f;
    public LayerMask droneLayer;
    public float throwForce = 12f;
    public GameObject empVfxPrefab;

    private Rigidbody rb;
    private Vector3 lastPosition;
    private float spawnTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        spawnTime = Time.time;
    }

    public void Launch(Vector3 direction)
    {
        if (rb != null)
        {
            rb.AddForce(direction * throwForce, ForceMode.Impulse);
        }
        Invoke(nameof(Detonate), 3f);
    }

    private void Update()
    {
        if (Time.time - spawnTime < 0.15f)
        {
            lastPosition = transform.position;
            return;
        }

        Vector3 movement = transform.position - lastPosition;
        float distance = movement.magnitude;

        if (distance > 0.001f)
        {
            if (Physics.Raycast(lastPosition, movement.normalized, out RaycastHit hit, distance))
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    transform.position = hit.point;
                    CancelInvoke(nameof(Detonate));
                    Detonate();
                    return;
                }
            }
        }

        lastPosition = transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || Time.time - spawnTime < 0.15f) return;

        CancelInvoke(nameof(Detonate));
        Detonate();
    }

    public void Detonate()
    {
        if (empVfxPrefab != null)
        {
            Instantiate(empVfxPrefab, transform.position, Quaternion.identity);
        }

        Collider[] targets = Physics.OverlapSphere(transform.position, blastRadius, droneLayer);
        foreach (var hit in targets)
        {
            if (hit.TryGetComponent<DroneAIController>(out var drone))
            {
                drone.ApplyEMPHit();
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
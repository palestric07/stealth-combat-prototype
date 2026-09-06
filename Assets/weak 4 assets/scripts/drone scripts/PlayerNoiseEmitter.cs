using UnityEngine;

public class PlayerNoiseEmitter : MonoBehaviour
{
    public float runNoiseRadius = 12f;
    public float craftNoiseRadius = 20f;
    public LayerMask droneLayer;

    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode craftKey = KeyCode.C;

    private void Update()
    {
        if (Input.GetKey(runKey) && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            EmitNoise(runNoiseRadius);
        }

        if (Input.GetKeyDown(craftKey))
        {
            EmitNoise(craftNoiseRadius);
        }
    }

    public void EmitNoise(float radius)
    {
        Collider[] hitDrones = Physics.OverlapSphere(transform.position, radius, droneLayer);
        
        foreach (var col in hitDrones)
        {
            if (col.TryGetComponent<DroneAIController>(out var drone))
            {
                drone.OnNoiseDetected(transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, runNoiseRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, craftNoiseRadius);
    }
}
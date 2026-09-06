using UnityEngine;

public class AISensor : MonoBehaviour
{
    [SerializeField] private string targetTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.TriggerAlert(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            if (HUDManager.Instance != null)
            {
                HUDManager.Instance.TriggerAlert(false);
            }
        }
    }
}
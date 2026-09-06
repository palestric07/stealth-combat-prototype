using UnityEngine;

/// <summary>
/// Handles player interaction logic by detecting and triggering nearby interactable objects.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 3.5f;
    public LayerMask interactableLayer = ~0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    /// <summary>
    /// Searches for nearby objects implementing IInteractable within range and triggers their interaction.
    /// </summary>
    private void TryInteract()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactRange, interactableLayer);
        bool foundInteractable = false;

        foreach (var col in colliders)
        {
            IInteractable interactable = col.GetComponent<IInteractable>();

            if (interactable == null)
            {
                interactable = col.GetComponentInParent<IInteractable>();
            }

            if (interactable != null)
            {
                foundInteractable = true;
                Debug.Log("Interacting with: " + col.gameObject.name);
                interactable.Interact();
                break;
            }
        }

        if (!foundInteractable)
        {
            Debug.LogWarning("No interactable object found nearby!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactRange);
    }
}
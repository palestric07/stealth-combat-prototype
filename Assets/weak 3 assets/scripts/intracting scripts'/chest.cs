using UnityEngine;

/// <summary>
/// Handles player interaction with a chest object to open and close its lid.
/// </summary>
public class ChestInteract : MonoBehaviour, IInteractable
{
    public Transform chestLid;
    public float openAngle = -80f;
    
    private bool isOpen = false;

    /// <summary>
    /// Returns the prompt string displayed to the player based on the chest state.
    /// </summary>
    /// <returns>Interaction prompt message.</returns>
    public string GetInteractPrompt()
    {
        return isOpen ? "Press E to Close Chest" : "Press E to Open Chest";
    }

    /// <summary>
    /// Toggles the chest between open and closed states by rotating the lid.
    /// </summary>
    public void Interact()
    {
        isOpen = !isOpen;

        float angleToRotate = isOpen ? openAngle : -openAngle;

        if (chestLid != null)
        {
            chestLid.Rotate(angleToRotate, 0, 0, Space.Self);
        }
        else
        {
            transform.Rotate(angleToRotate, 0, 0, Space.Self);
        }

        Debug.Log(isOpen ? "Chest Opened!" : "Chest Closed!");
    }
}
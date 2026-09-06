using UnityEngine;

/// <summary>
/// Handles teleporting the player to a target destination door upon interaction.
/// </summary>
public class TeleportDoor : MonoBehaviour, IInteractable
{
    public Transform destinationDoor;
    public Vector3 spawnOffset = new Vector3(0, 0, 1.5f);

    /// <summary>
    /// Returns the prompt string displayed to the player for entering the door.
    /// </summary>
    /// <returns>Interaction prompt message.</returns>
    public string GetInteractPrompt()
    {
        return "Press E to Enter Door";
    }

    /// <summary>
    /// Teleports the player to the assigned destination door position with an offset.
    /// </summary>
    public void Interact()
    {
        if (destinationDoor == null)
        {
            Debug.LogWarning("Destination Door is not assigned in the Inspector!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            CharacterController cc = player.GetComponent<CharacterController>();

            if (cc != null) cc.enabled = false;

            player.transform.position = destinationDoor.position + destinationDoor.TransformDirection(spawnOffset);
            player.transform.rotation = destinationDoor.rotation;

            if (cc != null) cc.enabled = true;

            Debug.Log("Teleported to: " + destinationDoor.name);
        }
    }
}
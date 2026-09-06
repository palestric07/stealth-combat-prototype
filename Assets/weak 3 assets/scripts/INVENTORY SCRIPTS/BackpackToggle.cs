using UnityEngine;

/// <summary>
/// Toggles the visibility of the backpack UI canvas.
/// </summary>
public class BackpackToggle : MonoBehaviour
{
    public GameObject backpackCanvas;
    public KeyCode toggleKey = KeyCode.B;

    private void Start()
    {
        if (backpackCanvas != null)
        {
            backpackCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleBackpack();
        }
    }

    /// <summary>
    /// Toggles the active state of the backpack UI canvas.
    /// </summary>
    public void ToggleBackpack()
    {
        if (backpackCanvas != null)
        {
            bool currentState = backpackCanvas.activeSelf;
            backpackCanvas.SetActive(!currentState);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

public class UI_InventoryManager : MonoBehaviour
{
    [Header("References")]
    public InventoryHandler playerInventory;
    public Transform slotContainer;
    public GameObject slotPrefab;

    private readonly List<UI_InventorySlot> uiSlots = new List<UI_InventorySlot>();

    private void Start()
    {
        if (playerInventory == null)
        {
            Debug.LogError($"[UI_InventoryManager] playerInventory reference is missing on {gameObject.name}.", this);
            return;
        }

        playerInventory.OnInventoryChanged += RefreshUI;
        InitializeSlots();
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnInventoryChanged -= RefreshUI;
        }
    }

    private void InitializeSlots()
    {
        if (slotContainer == null || slotPrefab == null)
        {
            Debug.LogError($"[UI_InventoryManager] slotContainer or slotPrefab is unassigned on {gameObject.name}.", this);
            return;
        }

        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
        uiSlots.Clear();

        for (int i = 0; i < playerInventory.maxSlots; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, slotContainer);
            
            if (newSlot.TryGetComponent<UI_InventorySlot>(out var slotScript))
            {
                uiSlots.Add(slotScript);
            }
            else
            {
                Debug.LogError($"[UI_InventoryManager] slotPrefab is missing the UI_InventorySlot component!", this);
            }
        }
    }

    public void RefreshUI()
    {
        for (int i = 0; i < uiSlots.Count; i++)
        {
            // Ensures both the slot entry and the assigned item exist
            if (i < playerInventory.slots.Count && 
                playerInventory.slots[i] != null && 
                playerInventory.slots[i].item != null)
            {
                var slotData = playerInventory.slots[i];
                uiSlots[i].UpdateSlot(slotData.item.icon, slotData.quantity);
            }
            else
            {
                uiSlots[i].ClearSlot();
            }
        }
    }
}
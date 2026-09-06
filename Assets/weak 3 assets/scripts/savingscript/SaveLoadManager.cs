using System.IO;
using UnityEngine;

/// <summary>
/// Manages saving and loading player position, health, and inventory data to a JSON file.
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    public Transform playerTransform;
    public PlayerHealth playerHealth;
    public InventoryHandler inventoryHandler;
    public ItemData[] allPossibleItems;

    private string saveFilePath;

    private void Awake()
    {
        saveFilePath = Path.Combine(Application.persistentDataPath, "savegame.json");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadGame();
        }
    }

    /// <summary>
    /// Saves current player stats, position, and inventory contents to a JSON save file.
    /// </summary>
    public void SaveGame()
    {
        SaveData data = new SaveData();

        if (playerTransform != null)
        {
            data.positionX = playerTransform.position.x;
            data.positionY = playerTransform.position.y;
            data.positionZ = playerTransform.position.z;
        }

        if (playerHealth != null)
        {
            data.currentHealth = playerHealth.currentHealth;
        }

        if (inventoryHandler != null)
        {
            for (int i = 0; i < inventoryHandler.slots.Count; i++)
            {
                var slot = inventoryHandler.slots[i];
                if (slot != null && slot.item != null)
                {
                    data.inventoryItems.Add(new InventoryItemSaveData
                    {
                        itemName = slot.item.itemName,
                        amount = GetSlotQuantity(slot)
                    });
                }
            }
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Game Saved to: " + saveFilePath);
    }

    /// <summary>
    /// Loads saved player stats, position, and inventory items from the JSON save file.
    /// </summary>
    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("Save file not found!");
            return;
        }

        string json = File.ReadAllText(saveFilePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        if (playerTransform != null)
        {
            CharacterController cc = playerTransform.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;

            playerTransform.position = new Vector3(data.positionX, data.positionY, data.positionZ);

            if (cc != null) cc.enabled = true;
        }

        if (playerHealth != null)
        {
            playerHealth.currentHealth = data.currentHealth;
            playerHealth.Heal(0f);
        }

        if (inventoryHandler != null)
        {
            for (int i = inventoryHandler.slots.Count - 1; i >= 0; i--)
            {
                var slot = inventoryHandler.slots[i];
                if (slot != null && slot.item != null)
                {
                    inventoryHandler.RemoveItem(slot.item, GetSlotQuantity(slot));
                }
            }

            foreach (var savedItem in data.inventoryItems)
            {
                ItemData itemAsset = FindItemDataByName(savedItem.itemName);
                if (itemAsset != null)
                {
                    inventoryHandler.AddItem(itemAsset, savedItem.amount);
                }
            }
        }

        Debug.Log("Game Loaded successfully!");
    }

    private int GetSlotQuantity(InventorySlot slot)
    {
        var type = slot.GetType();
        var field = type.GetField("quantity") ?? type.GetField("count") ?? type.GetField("itemCount") ?? type.GetField("amount");
        
        if (field != null)
        {
            return (int)field.GetValue(slot);
        }

        return 1;
    }

    private ItemData FindItemDataByName(string name)
    {
        if (allPossibleItems == null) return null;
        foreach (var item in allPossibleItems)
        {
            if (item != null && item.itemName == name)
            {
                return item;
            }
        }
        return null;
    }
}
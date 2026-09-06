using UnityEngine;

/// <summary>
/// Handles item pickup logic for pooled game objects and returns them to ObjectPoolManager upon collection.
/// </summary>
public class PooledItemPickup : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;
    public string poolKey;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryHandler inventory = other.GetComponent<InventoryHandler>();
            if (inventory != null && itemData != null)
            {
                inventory.AddItem(itemData, amount);

                if (ObjectPoolManager.Instance != null && !string.IsNullOrEmpty(poolKey))
                {
                    ObjectPoolManager.Instance.ReturnToPool(poolKey, gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}
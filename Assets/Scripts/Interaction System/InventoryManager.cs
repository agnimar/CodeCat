using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxCapacity = 8;
    // We can store references to the actual GameObjects or to a simple ItemData class/ScriptableObject
    private List<GameObject> items = new List<GameObject>();

    // A reference for spawn/drop position (like near the player)
    [SerializeField] private Transform dropPosition;

    public bool AddItem(GameObject item)
    {
        if (items.Count >= maxCapacity)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        // If not full, add the item to the list
        items.Add(item);

        // Deactivate or hide the actual item in the world so we don't see it
        item.SetActive(false);

        Debug.Log($"{item.name} added to inventory.");

        Debug.Log($"Populating inventory UI with {items.Count} items.");
        foreach (var i in items)
        {
            Debug.Log($"Item in inventory: {i?.name ?? "Null Item"}");
        }

        return true;
    }

    public bool RemoveItem(string itemName)
    {
        // Find item in the list by name (or other ID if you prefer)
        int index = items.FindIndex(i => i.name == itemName);
        if (index == -1)
        {
            Debug.Log($"Item {itemName} not in inventory.");
            return false;
        }

        // Remove the item from the inventory
        items.RemoveAt(index);
        Debug.Log($"{itemName} removed from inventory.");
        return true;
    }

    public bool HasItem(string itemName)
    {
        return items.Exists(i => i.name == itemName);
    }

    // This version removes the actual GameObject and returns it (for placing on pillars, etc.)
    public GameObject GetAndRemoveItem(string itemName)
    {
        int index = items.FindIndex(i => i.name == itemName);
        if (index == -1) return null;

        GameObject item = items[index];
        items.RemoveAt(index);
        return item;
    }

    // Drop the currently selected item or any item from the inventory
    public void DropItem(string itemName)
    {
        GameObject itemToDrop = GetAndRemoveItem(itemName);
        if (itemToDrop == null) return;

        // Activate the item and place it in front of the player
        itemToDrop.SetActive(true);
        itemToDrop.transform.position = dropPosition.position;
        itemToDrop.transform.rotation = dropPosition.rotation;

        Debug.Log($"Dropped {itemToDrop.name} from inventory.");
    }
    public List<GameObject> GetItems()
    {
        // Remove null or destroyed items from the list
        items.RemoveAll(item => item == null);
        return items;
    }


}

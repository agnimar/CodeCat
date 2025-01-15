using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxCapacity = 8;
    [SerializeField] private Transform dropPosition; // Position to drop items
    private List<GameObject> items = new List<GameObject>();
    public bool IsOpenedByPlayer { get; set; }


    public bool AddItem(GameObject item)
    {
        if (items.Count >= maxCapacity)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        items.Add(item);
        item.SetActive(false); // Hide item in the world
        
        return true;
    }

    public void DropItem(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning("Invalid inventory slot index.");
            return;
        }

        GameObject item = items[index];
        items.RemoveAt(index); // Remove the item from the inventory

        // Enable the item
        item.SetActive(true);

        // Perform a raycast to find the ground below the drop position
        Vector3 dropPoint = dropPosition.position;
        if (Physics.Raycast(dropPosition.position, Vector3.down, out RaycastHit hitInfo, 10f))
        {
            dropPoint = hitInfo.point; // Adjust the position to the ground level
        }

        item.transform.position = dropPoint;
        //item.transform.rotation = dropPosition.rotation;

        //Debug.Log($"{item.name} dropped from inventory at {dropPoint}.");
    }


    public List<GameObject> GetItems()
    {
        items.RemoveAll(item => item == null); // Remove destroyed or null items
        return items;
    }
    public GameObject GetAndRemoveItem(string itemName)
    {
        var item = items.Find(i => i.GetComponent<InteractableBase>()?.ItemName == itemName);
        if (item != null)
        {
            items.Remove(item);
            return item;
        }
        return null;
    }

}
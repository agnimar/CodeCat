using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxCapacity = 8;
    [SerializeField] private Transform dropPosition;
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
        item.SetActive(false);
        SoundManager.PlaySound(SoundType.PICK_UP_OBJECT);

        return true;
    }

    public void DropItem(int index)
    {
        if (index < 0 || index >= items.Count)
        {
            Debug.LogWarning("Invalid inventory slot index.");
            return;
        }
        SoundManager.PlaySound(SoundType.DROP);
        GameObject item = items[index];
        items.RemoveAt(index);

        item.SetActive(true);

        Vector3 dropPoint = dropPosition.position;
        if (Physics.Raycast(dropPosition.position, Vector3.down, out RaycastHit hitInfo, 10f))
        {
            dropPoint = hitInfo.point;
        }
        item.transform.position = dropPoint;
    }


    public List<GameObject> GetItems()
    {
        items.RemoveAll(item => item == null);
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
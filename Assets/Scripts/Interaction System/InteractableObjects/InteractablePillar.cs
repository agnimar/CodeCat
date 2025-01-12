using UnityEngine;

public class InteractablePillar : InteractableBase
{
    private bool isOccupied = false;
    private GameObject interactingPlayer;

    public bool IsOccupied => isOccupied;

    public override void OnInteractionStart(InteractionData data)
    {
        if (isOccupied)
        {
            Debug.Log("Pillar is already occupied.");
            return;
        }

        interactingPlayer = data.Interactor;

        // Open inventory for item selection
        UIManager.Instance.ShowInventoryForSelection(OnItemSelected);
    }

    private void OnItemSelected(GameObject selectedItem)
    {
        if (selectedItem == null)
        {
            Debug.Log("No item selected.");
            return;
        }

        var inventory = interactingPlayer.GetComponent<InventoryManager>();
        if (inventory == null)
        {
            Debug.LogError("Interactor missing InventoryManager!");
            return;
        }

        if (ValidateItem(selectedItem))
        {
            PlaceItemOnPillar(selectedItem);
            inventory.GetAndRemoveItem(selectedItem.GetComponent<InteractableBase>().ItemName);
        }
        else
        {
            Debug.Log("Incorrect item placed.");
            Debug.Log("The item doesn't fit this pillar.");
        }
    }

    private bool ValidateItem(GameObject item)
    {
        var interactableBase = item.GetComponent<InteractableBase>();
        return interactableBase != null; // Customize validation if needed
    }

    private void PlaceItemOnPillar(GameObject itemObject)
    {
        isOccupied = true;

        // Position and attach the item to the pillar
        itemObject.SetActive(true);
        itemObject.transform.position = transform.position + Vector3.up * 1.0f; // Adjust height
        itemObject.transform.SetParent(transform);

        Debug.Log($"{itemObject.name} placed on the pillar!");
        PuzzleManager.Instance?.CheckPuzzleState();
    }
}
